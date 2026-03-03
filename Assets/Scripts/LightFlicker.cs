using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Intensity Settings")]
    [SerializeField] private int minIntensity;
    [SerializeField] private int maxIntensity;

    [Header("Steady")]
    [SerializeField] private float minSteadyTime;
    [SerializeField] private float maxSteadyTime;

    [Header("Flicker Settings")]
    [SerializeField] private int minFlickerCount;
    [SerializeField] private int maxFlickerCount;
    [SerializeField] private float flickerSpeed;

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
            SetLightIntensity(maxIntensity);
            float steadyTime = Random.Range(minSteadyTime, maxSteadyTime);
            yield return new WaitForSeconds(steadyTime);

            int flickers = Random.Range(minFlickerCount, maxFlickerCount);

            for (int i = 0; i < flickers; i++)
            {
                int targetIntensitySpot = Random.Range(0, maxIntensity + 1);
                SetLightIntensity(targetIntensitySpot);

                yield return new WaitForSeconds(flickerSpeed);
            }

            //Chance for the light to turn off completely
            if (Random.value < 0.01f)
            {
                SetLightIntensity(0);
                yield return new WaitForSeconds(2f);
            }
        }
    }

    void SetLightIntensity(int spotIntensity)
    {
        float targetIntensityPoint = (float)spotIntensity / 20f;

        //Applies the same intensity for all lights on the lamppost simultaneously
        foreach (Light light in lightSources)
        {
            if (light.type == LightType.Spot)
            {
                light.intensity = (float)spotIntensity;
            }
            else if (light.type == LightType.Point)
            {
                light.intensity = targetIntensityPoint;
            }
        }
    }
}
