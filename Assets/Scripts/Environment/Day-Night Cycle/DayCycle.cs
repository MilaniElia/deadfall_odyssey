using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[ExecuteInEditMode]
public class DayCycle : MonoBehaviour
{
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField, Range(0, 96)] private float TimeOfDay;
    [SerializeField] private float MaxDayDuration = 96;

    private float StartOfDayPercentage = 20;
    private float EndOfDayPercentage = 75;
    private bool LightsOn;

    private bool IsNight
    {
        get
        {
            float timePercent = (TimeOfDay / MaxDayDuration) * 100;
            if (timePercent < StartOfDayPercentage + 5 || timePercent > EndOfDayPercentage - 3)
            {
                return true;
            }
            return false;
        }
    }

    private void Awake()
    {
        TurnOffLights();
    }

    private void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {
            TimeOfDay += Time.deltaTime;
            TimeOfDay %= MaxDayDuration;
            UpdateLighting(TimeOfDay / MaxDayDuration);
        }
        else
        {
            UpdateLighting(TimeOfDay / MaxDayDuration);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);
        ManageNightLights();
        if ((timePercent * 100) > EndOfDayPercentage || (timePercent * 100) < StartOfDayPercentage)
        {
            DirectionalLight.gameObject.SetActive(false);

        }
        else
        {
            if (!DirectionalLight.gameObject.activeSelf)
            {
                DirectionalLight.gameObject.SetActive(true);
            }
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
        }
    }

    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }

    private void ManageNightLights()
    {
        if (!LightsOn && IsNight)
        {
            TurnOnLights();
        }
        else if (LightsOn && !IsNight)
        {
            TurnOffLights();
        }
    }

    private void TurnOnLights()
    {
        Light[] lights = (Light[])(GameObject.FindObjectsOfType(typeof(Light), true));
        foreach (Light light in lights)
        {
            light.gameObject.SetActive(true);
        }
        LightsOn = true;
    }

    private void TurnOffLights()
    {
        Light[] lights = (Light[])(GameObject.FindObjectsOfType(typeof(Light), true));
        foreach (Light light in lights)
        {
            light.gameObject.SetActive(false);
        }
        LightsOn = false;
    }
}
