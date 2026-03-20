using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamageFeedbackPlayer : MonoBehaviour
{
    [Header("Vignette Settings")]
    [SerializeField] private float maxVignetteIntensity = 0.65f;
    [SerializeField] private float minVignetteIntensity = 0.2f;
    [SerializeField] private Color damageColor;

    [Header("References")]
    [SerializeField] private EntityStats stats;
    [SerializeField] private Volume volume;
    private Vignette vignette;

    void Start()
    {
        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.active = true;
            vignette.color.overrideState = true;
            vignette.color.value = damageColor;
            vignette.intensity.overrideState = true;
        }
        else
        {
            Debug.LogError("Vignette effect not found in the Volume");
        }
    }

    void Update()
    {
        if (stats == null || vignette == null) return;

        //Calculate health percentage (1.0 = full health, 0.0 = dead)
        float healthPercent = stats.CurrentHp / stats.MaxHp;

        float targetIntensity;

        //Logic to decide the intensity based on current health
        if (healthPercent >= 1f)
        {
            // If health is full, remove the red vignette
            targetIntensity = 0.2f;
        }
        else
        {
            //Calculate how much damage was taken (0.0 = none, 1.0 = near death)
            float intensityPercent = 1.0f - healthPercent;

            //Map the damage to a range between Min and Max intensity
            targetIntensity = Mathf.Lerp(minVignetteIntensity, maxVignetteIntensity, intensityPercent);
        }

        //Smoothly transition the current intensity to the target value
        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetIntensity, Time.deltaTime * 5f);
    }
}
