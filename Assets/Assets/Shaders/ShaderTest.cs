using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTest : MonoBehaviour
{
    private static readonly int _dissolveValue = Shader.PropertyToID("_Dissolve_Value");
    private static readonly int _seedValue = Shader.PropertyToID("_Seed");
    private Material _dissolveMaterial;

    [SerializeField] private GameObject particles;
    [SerializeField] private float dissolveDuration = 2f;
    [SerializeField] private float dissolveValue;

    private void Start()
    {
        _dissolveMaterial = GetComponent<MeshRenderer>().material;
    }

    private IEnumerator Dissolver()
    {
        float elapsedTime = 0f;
        
        var randomSeed = UnityEngine.Random.Range(0f, 1f);
        _dissolveMaterial.SetFloat(_seedValue, randomSeed);
        
        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;
            dissolveValue = Mathf.Lerp(0f, 1f, elapsedTime / dissolveDuration);
            _dissolveMaterial.SetFloat(_dissolveValue, dissolveValue);

            if (particles.activeSelf == true && dissolveValue >= 0.2f)
            {
                particles.SetActive(false);
            }
            yield return null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            particles.SetActive(true);
            StartCoroutine(Dissolver());
        }
    }
}
