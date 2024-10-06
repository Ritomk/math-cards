using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CoroutineHelper
{
    private class CoroutineRunner : MonoBehaviour { }
    
    private static CoroutineRunner _runner;
    
    private static Dictionary<IEnumerator, Coroutine> _coroutineMapping
     = new Dictionary<IEnumerator, Coroutine>();

    /// <summary>
    /// Singleton instance of the CoroutineRunner.
    /// </summary>
    private static CoroutineRunner Runner
    {
        get
        {
            if (_runner == null)
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
    public static void Start(IEnumerator coroutine)
    {
        Coroutine wrappedCoroutine = Runner.StartCoroutine(WrapCoroutine(coroutine));
        _coroutineMapping[coroutine] = wrappedCoroutine;
    }

    /// <summary>
    /// Stops a specific coroutine.
    /// </summary>
    /// <param name="coroutine">The Coroutine to stop.</param>
    public static void Stop(IEnumerator coroutine)
    {
        if (_runner != null && coroutine != null)
        {
            if (_coroutineMapping.TryGetValue(coroutine, out Coroutine wrappedCoroutine))
            {
                Runner.StopCoroutine(wrappedCoroutine);
                _coroutineMapping.Remove(coroutine);
            }
        }
    }

    /// <summary>
    /// Stops all running coroutines managed by the CoroutineHelper.
    /// </summary>
    public static void StopAll()
    {
        Runner.StopAllCoroutines();
        _coroutineMapping.Clear();
    }

    /// <summary>
    /// Pauses all coroutines managed by the CoroutineHelper.
    /// </summary>
    public static void Pause()
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
    public static void Resume()
    {
        if (IsPaused)
        {
            IsPaused = false;
            Debug.Log("All Coroutines have been resumed.");
        }
    }

    private static IEnumerator WrapCoroutine(IEnumerator coroutine)
    {
        while (IsPaused)
        {
            yield return null;
        }

        while (true)
        {
            if (!coroutine.MoveNext())
            {
                _coroutineMapping.Remove(coroutine);
                yield break;
            }
            
            object yielded = coroutine.Current;

            while (IsPaused)
            {
                yield return null;
            }
            
            yield return yielded;
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