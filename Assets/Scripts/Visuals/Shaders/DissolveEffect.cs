using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect  : MonoBehaviour
{
    private static readonly int DissolveValueID = Shader.PropertyToID("_Dissolve_Value");
    private static readonly int SeedValueID = Shader.PropertyToID("_Seed");
    private static readonly int ObjectColorID = Shader.PropertyToID("_Object_Color");
    private Material _dissolveMaterial;
    
    [SerializeField] private GameObject additionalDisable;
    [SerializeField] private float dissolveDuration = 2f;
    [SerializeField] private float dissolveValue;

    private void Awake()
    {
        _dissolveMaterial = GetComponent<MeshRenderer>().material;
    }
    
    public IEnumerator StartDissolve()
    {
        float elapsedTime = 0f;
        
        var randomSeed = UnityEngine.Random.Range(0f, 1f);
        _dissolveMaterial.SetFloat(SeedValueID, randomSeed);
        
        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;
            dissolveValue = Mathf.Lerp(0f, 1f, elapsedTime / dissolveDuration);
            _dissolveMaterial.SetFloat(DissolveValueID, dissolveValue);

            if (additionalDisable?.activeSelf == true && dissolveValue >= 0.2f)
            {
                additionalDisable.SetActive(false);
            }
            yield return null;
        }
    }

    public void Dissolve()
    {
        CoroutineHelper.Start(StartDissolve());
    }

    public void ChangeColor(Color color)
    {
        _dissolveMaterial.SetColor(ObjectColorID, color);

    }
}
