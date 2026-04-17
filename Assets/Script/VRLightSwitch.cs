using UnityEngine;
using UnityEngine.Rendering;
using System.Collections; // Required for Coroutines (Delays)

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
        public Light associatedLight;
        public float delay; // How many seconds to wait?
    }

    [Header("Lighting & Sky")]
    public Light sunLight;
    public Material daySkybox;
    public Material nightSkybox;

    [Header("Original Logic: TEXT & FRAME")]
    public MeshRenderer textMesh; 
    public Material textE1_On;
    public Material textE1_Off;
    public MeshRenderer frameMesh; 
    public Material frameE0_On;
    public Material frameE0_Off;
    public Material frameE1_On;
    public Material frameE1_Off;

    [Header("--- ULTIMATE LIGHTING SPACE ---")]
    public ExtraObjectSwap[] ultimateObjects;

    // This is the function your button calls
    public void ToggleSun()
    {
        if (sunLight == null) return;

        // Start the process
        StartCoroutine(ToggleRoutine());
    }

    private IEnumerator ToggleRoutine()
    {
        // 1. Flip Sun & Sky immediately
        sunLight.enabled = !sunLight.enabled;
        bool isNight = !sunLight.enabled;
        RenderSettings.skybox = isNight ? nightSkybox : daySkybox;

        // 2. Update the original Sign immediately
        UpdateOriginalSign(isNight);

        // 3. Loop through Ultimate Objects and handle their specific delays
        foreach (ExtraObjectSwap swap in ultimateObjects)
        {
            // Start a separate mini-routine for each object so they don't block each other
            StartCoroutine(ApplyDelayedSwap(swap, isNight));
        }

        // Refresh environment
        DynamicGI.UpdateEnvironment();
        yield return null;
    }

    private IEnumerator ApplyDelayedSwap(ExtraObjectSwap swap, bool isNight)
    {
        // Wait for the specific delay time set in the Inspector
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

        if (swap.associatedLight != null)
        {
            swap.associatedLight.enabled = isNight;
        }
    }

    private void UpdateOriginalSign(bool isNight)
    {
        if (textMesh != null)
        {
            Material[] textMats = textMesh.materials;
            if (textMats.Length > 1)
            {
                textMats[1] = isNight ? textE1_On : textE1_Off;
                textMesh.materials = textMats;
            }
        }

        if (frameMesh != null)
        {
            Material[] frameMats = frameMesh.materials;
            if (frameMats.Length > 0) frameMats[0] = isNight ? frameE0_On : frameE0_Off;
            if (frameMats.Length > 1) frameMats[1] = isNight ? frameE1_On : frameE1_Off;
            frameMesh.materials = frameMats;
        }
    }
}