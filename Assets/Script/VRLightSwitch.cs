using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class VRLightSwitch : MonoBehaviour
{
    [System.Serializable]
    public struct ExtraObjectSwap
    {
        public string label;          
        public MeshRenderer mesh;    
        public int materialIndex;    
        public Material dayMat;      
        public Material nightMat;    
        public float delay;          
    }

    [System.Serializable]
    public struct UltimateLight
    {
        public string label;          
        public Light lightSource;    
        public float delay;          
    }

    [Header("Lighting & Sky")]
    public Light sunLight;
    public Material daySkybox;
    public Material nightSkybox;

    [Header("--- AUDIO SETTINGS ---")]
    public AudioSource dayAudio;   // Drag your Day ambient sound here
    public AudioSource nightAudio; // Drag your Night ambient sound here
    public float audioFadeSpeed = 1.0f; // Seconds to fade in/out

    [Header("--- ULTIMATE OBJECTS (Materials) ---")]
    public ExtraObjectSwap[] ultimateObjects;

    [Header("--- ULTIMATE LIGHTS (Real Lighting) ---")]
    public UltimateLight[] ultimateLights;

    void Start()
    {
        // Set initial audio volumes
        if (dayAudio) dayAudio.volume = sunLight.enabled ? 1 : 0;
        if (nightAudio) nightAudio.volume = sunLight.enabled ? 0 : 1;
        
        if (dayAudio && !dayAudio.isPlaying) dayAudio.Play();
        if (nightAudio && !nightAudio.isPlaying) nightAudio.Play();
    }

    public void ToggleSun()
    {
        if (sunLight == null) return;
        StartCoroutine(ToggleRoutine());
    }

    private IEnumerator ToggleRoutine()
    {
        // 1. Immediate Environment Changes
        sunLight.enabled = !sunLight.enabled;
        bool isNight = !sunLight.enabled;
        RenderSettings.skybox = isNight ? nightSkybox : daySkybox;

        // 2. Audio Transition
        StartCoroutine(FadeAudio(isNight));

        // 3. Handle Mesh Swaps
        foreach (ExtraObjectSwap swap in ultimateObjects)
        {
            StartCoroutine(ApplyDelayedSwap(swap, isNight));
        }

        // 4. Handle Ultimate Lights
        foreach (UltimateLight uLight in ultimateLights)
        {
            StartCoroutine(ApplyDelayedLight(uLight, isNight));
        }

        DynamicGI.UpdateEnvironment();
        yield return null;
    }

    private IEnumerator FadeAudio(bool isNight)
    {
        float timer = 0;
        float startDayVol = dayAudio ? dayAudio.volume : 0;
        float startNightVol = nightAudio ? nightAudio.volume : 0;

        float targetDayVol = isNight ? 0 : 1;
        float targetNightVol = isNight ? 1 : 0;

        while (timer < audioFadeSpeed)
        {
            timer += Time.deltaTime;
            float percent = timer / audioFadeSpeed;

            if (dayAudio) dayAudio.volume = Mathf.Lerp(startDayVol, targetDayVol, percent);
            if (nightAudio) nightAudio.volume = Mathf.Lerp(startNightVol, targetNightVol, percent);
            
            yield return null;
        }
    }

    private IEnumerator ApplyDelayedSwap(ExtraObjectSwap swap, bool isNight)
    {
        yield return new WaitForSeconds(swap.delay);
        if (swap.mesh != null)
        {
            Material[] mats = swap.mesh.materials;
            if (swap.materialIndex < mats.Length)
            {
                mats[swap.materialIndex] = isNight ? swap.nightMat : swap.dayMat;
                swap.mesh.materials = mats;
            }
        }
    }

    private IEnumerator ApplyDelayedLight(UltimateLight uLight, bool isNight)
    {
        yield return new WaitForSeconds(uLight.delay);
        if (uLight.lightSource != null)
        {
            uLight.lightSource.enabled = isNight;
        }
    }
}