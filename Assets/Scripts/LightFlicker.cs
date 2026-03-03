using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Intensity Settings")]
    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;

    [Header("Speed")]
    [SerializeField] private float minWaitTime;
    [SerializeField] private float maxWaitTime;

    private Light[] lightSources;

    void Start()
    {
        lightSources = GetComponentsInChildren<Light>();

        if (lightSources.Length == 0)
        {
            Debug.LogWarning($"No light found on lamppost: {gameObject.name}");
            return;
        }

        StartCoroutine(FlickerRoutine());
    }

    IEnumerator FlickerRoutine()
    {
        while (true)
        {
            //Random intensity value for current flicker cycle
            float targetIntensitySpot = Random.Range(minIntensity, maxIntensity);

            //Chance for the light to turn off completely
            if (Random.value < 0.01f)
            {
                targetIntensitySpot = 0;
            }

            float targetIntensityPoint = targetIntensitySpot / maxIntensity;

            //Applies the same intensity for all lights on the lamppost simultaneously
            foreach (Light light in lightSources)
            {
                if (light.type == LightType.Spot)
                {
                    light.intensity = targetIntensitySpot;
                }
                else if (light.type == LightType.Point)
                {
                    light.intensity = targetIntensityPoint;
                }
            }

            float waitTime;
            if (targetIntensitySpot == 0f) waitTime = 5f;
            else waitTime = Random.Range(minWaitTime, maxWaitTime);

            yield return new WaitForSeconds(waitTime);
        }
    }
}
