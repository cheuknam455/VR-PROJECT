using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class VRLightSwitch : MonoBehaviour
{
    [System.Serializable]
    public struct ExtraObjectSwap
    {
        public string label;          // Name for organization
        public MeshRenderer mesh;    // The object (e.g., typeMesh1)
        public int materialIndex;    // Slot index (0, 1, 2...)
        public Material dayMat;      // Off material
        public Material nightMat;    // On/Glow material
        public float delay;          // Time before swapping
    }

    [System.Serializable]
    public struct UltimateLight
    {
        public string label;          // Name for organization
        public Light lightSource;    // The Point/Spot Light component
        public float delay;          // Time before turning on/off
    }

    [Header("Lighting & Sky")]
    public Light sunLight;
    public Material daySkybox;
    public Material nightSkybox;

    [Header("--- ULTIMATE OBJECTS (Materials) ---")]
    public ExtraObjectSwap[] ultimateObjects;

    [Header("--- ULTIMATE LIGHTS (Real Lighting) ---")]
    public UltimateLight[] ultimateLights;

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

        // 2. Handle Mesh Swaps with their specific delays
        foreach (ExtraObjectSwap swap in ultimateObjects)
        {
            StartCoroutine(ApplyDelayedSwap(swap, isNight));
        }

        // 3. Handle Ultimate Lights with their specific delays
        foreach (UltimateLight uLight in ultimateLights)
        {
            StartCoroutine(ApplyDelayedLight(uLight, isNight));
        }

        // 4. Force global lighting refresh
        DynamicGI.UpdateEnvironment();
        yield return null;
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