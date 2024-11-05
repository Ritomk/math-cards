using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

public static class CoroutineHelper
{
    private class CoroutineRunner : MonoBehaviour { }
    
    private static CoroutineRunner _runner;
    
    private struct CoroutineInfo
    {
        public Coroutine Coroutine;
        public string Name;
    }
    
    private static int _nextId = 0;

    private static HashSet<int> _pausedCoroutines = new HashSet<int>();
    
    private static Dictionary<int, CoroutineInfo> _coroutineMapping
        = new Dictionary<int, CoroutineInfo>();

    private static Dictionary<int, CoroutineInfo> _stateCoroutineMapping
        = new Dictionary<int, CoroutineInfo>();
    
    /// <summary>
    /// Singleton instance of the CoroutineRunner.
    /// </summary>
    private static CoroutineRunner Runner
    {
        get
        {
            if (_runner is null)
            {
                GameObject obj = new GameObject("CoroutineHelper");
                _runner = obj.AddComponent<CoroutineRunner>();
                Object.DontDestroyOnLoad(obj);
            }
            return _runner;
        }
    }
    
    /// <summary>
    /// Indicates whether all coroutines are currently paused.
    /// </summary>
    public static bool IsPaused { get; private set; } = false;

    /// <summary>
    /// Starts a coroutine.
    /// </summary>
    /// <param name="coroutine">The IEnumerator to run as a coroutine.</param>
    /// <param name="name">Optional name for the coroutine</param>
    public static int Start(IEnumerator coroutine, 
        [CallerMemberName] string name = "Unnamed Coroutine")
    {
        int id = _nextId++;
        IEnumerator wrappedCoroutine = WrapCoroutine(coroutine, id, name);
        Coroutine unityCoroutine = Runner.StartCoroutine(wrappedCoroutine);
        _coroutineMapping[id] = new CoroutineInfo
        {
            Coroutine = unityCoroutine, 
            Name = name
        };
        return id;
    }

    /// <summary>
    /// Starts a coroutine and waits until it finishes.
    /// </summary>
    /// <param name="coroutine">The IEnumerator to run as a coroutine.</param>
    /// <param name="name">Optional name for the coroutine</param>
    public static IEnumerator StartAndWait(IEnumerator coroutine, 
        [CallerMemberName] string name = "Unnamed Coroutine")
    {
        int id = Start(coroutine, name);
        
        while (_coroutineMapping.ContainsKey(id))
        {
            yield return null;
        }
    }

    /// <summary>
    /// Stops a specific coroutine.
    /// </summary>
    /// <param name="coroutine">The Coroutine to stop.</param>
    public static void Stop(int id)
    {
        if (_runner != null)
        {
            if (_coroutineMapping.TryGetValue(id, out CoroutineInfo coroutineInfo))
            {
                Runner.StopCoroutine(coroutineInfo.Coroutine);
                _coroutineMapping.Remove(id);
                
                if(_pausedCoroutines.Contains(id))
                    _pausedCoroutines.Remove(id);
            }
        }
    }

    /// <summary>
    /// Pauses specific coroutine.
    /// </summary>
    /// <param name="id">The ID of the coroutine to pause.</param>
    public static void Pause(int id)
    {
        if (_coroutineMapping.ContainsKey(id))
        {
            _pausedCoroutines.Add(id);
        }
    }

    /// <summary>
    /// Resumes specific coroutine.
    /// </summary>
    /// <param name="id">The ID of the coroutine to resume.</param>
    public static void Resume(int id)
    {
        if (_pausedCoroutines.Contains(id))
        {
            _pausedCoroutines.Remove(id);
        }
    }

    /// <summary>
    /// Stops all running coroutines managed by the CoroutineHelper.
    /// </summary>
    public static void StopAll()
    {
        Runner.StopAllCoroutines();
        _coroutineMapping.Clear();
        _pausedCoroutines.Clear();
    }

    /// <summary>
    /// Pauses all coroutines managed by the CoroutineHelper.
    /// </summary>
    public static void PauseAll()
    {
        if (!IsPaused)
        {
            IsPaused = true;
            Debug.Log("All Coroutines have been paused.");
        }
    }

    /// <summary>
    /// Resumes all coroutines managed by the CoroutineHelper.
    /// </summary>
    public static void ResumeAll()
    {
        if (IsPaused)
        {
            IsPaused = false;
            Debug.Log("All Coroutines have been resumed.");
        }
    }

    public static int GetRunningCoroutinesCount()
    {
        return _coroutineMapping.Count;
    }

    public enum DebugType
    {
        Both,
        Normal,
        State,
    }
    public static void DebugRunningCoroutines(DebugType debugType)
    {
        if (_coroutineMapping.Count == 0)
        {
            Debug.Log("No coroutines are currently running.");
            return;
        }

        if (debugType is DebugType.Both or DebugType.Normal)
        {
            Debug.Log("Currently running coroutines:");
            foreach (var kvp in _coroutineMapping)
            {
                int id = kvp.Key;
                string name = kvp.Value.Name;
                Debug.Log($"ID: {id}, Name: {name}");
            }  
        }

        if (debugType is DebugType.Both or DebugType.State)
        {
            Debug.Log("Currently running state coroutines:");
            foreach (var kvp in _stateCoroutineMapping)
            {
                int id = kvp.Key;
                string name = kvp.Value.Name;
                Debug.Log($"State Coroutine ID: {id}, Name: {name}");
            }
        }
    }

    public static int StartState(IEnumerator coroutine, string name = "Unnamed Coroutine")
    {
        int id = _nextId++;
        Coroutine unityCoroutine = Runner.StartCoroutine(coroutine);
        _stateCoroutineMapping[id] = new CoroutineInfo
        {
            Coroutine = unityCoroutine,
            Name = name
        };
        return id;
    }

    public static void StopState(int id)
    {
        if (_coroutineMapping.TryGetValue(id, out CoroutineInfo coroutineInfo))
        {
            Runner.StopCoroutine(coroutineInfo.Coroutine);
            _stateCoroutineMapping.Remove(id);
        }
    }

    /// <summary>
    /// Waits for all managed coroutines to finish, with an optional exit time.
    /// </summary>
    /// <param name="exitTime">
    /// The maximum time to wait in seconds. Set to -1 to wait indefinitely.
    /// </param>
    public static IEnumerator WaitForAllCoroutines(float exitTime = -1f)
    {
        float elapsedTime = 0f;
        bool hasTimeout = exitTime >= -1f;

        while (_coroutineMapping.Count > 0)
        {
            DebugRunningCoroutines(DebugType.Normal);
            if (hasTimeout)
            {
                if (elapsedTime >= exitTime)
                {
                    yield break;
                }
                elapsedTime += Time.deltaTime;
            }
            
            yield return null;
        }
    }

    private static IEnumerator WrapCoroutine(IEnumerator coroutine, int id, string name = "TempName")
    {
        yield return null;
        
        while (true)
        {
            while (IsPaused || _pausedCoroutines.Contains(id))
            {
                yield return null;
            }


            if (!coroutine.MoveNext())
            {
                _coroutineMapping.Remove(id); 
                _pausedCoroutines.Remove(id);
                yield break;
            }

            yield return coroutine.Current;
        }
    }
}

public class WaitForSecondsPauseable : IEnumerator
{
    private float _timeRemaining;

    public WaitForSecondsPauseable(float time)
    {
        _timeRemaining = time;
    }
    
    public object Current => null;
    
    public bool MoveNext()
    {
        if (!CoroutineHelper.IsPaused)
        {
            _timeRemaining -= Time.deltaTime;
        }
        return _timeRemaining > 0;
    }

    public void Reset() { }
}