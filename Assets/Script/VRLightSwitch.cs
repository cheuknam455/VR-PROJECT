using UnityEngine;
using UnityEngine.Rendering;

public class VRLightSwitch : MonoBehaviour
{
    [Header("Lighting & Sky")]
    public Light sunLight;
    public Material daySkybox;
    public Material nightSkybox;

    [Header("TEXT (Changes Element 1 Only)")]
    public MeshRenderer textMesh; 
    public Material textE1_On;
    public Material textE1_Off;

    [Header("FRAME (Changes Element 0 & 1)")]
    public MeshRenderer frameMesh; 
    public Material frameE0_On;
    public Material frameE0_Off;
    public Material frameE1_On;
    public Material frameE1_Off;

    public void ToggleSun()
    {
        // 1. Flip the sun state
        sunLight.enabled = !sunLight.enabled;

        // 2. Update Skybox
        RenderSettings.skybox = sunLight.enabled ? daySkybox : nightSkybox;

        // 3. Update TEXT (Element 1 Only)
        if (textMesh != null)
        {
            Material[] textMats = textMesh.materials;
            if (textMats.Length > 1) // Ensure it actually has an Element 1
            {
                textMats[1] = sunLight.enabled ? textE1_Off : textE1_On;
                textMesh.materials = textMats;
            }
        }

        // 4. Update FRAME (Element 0 and 1)
        if (frameMesh != null)
        {
            Material[] frameMats = frameMesh.materials;
            
            if (sunLight.enabled) // MORNING
            {
                if (frameMats.Length > 0) frameMats[0] = frameE0_Off;
                if (frameMats.Length > 1) frameMats[1] = frameE1_Off;
            }
            else // NIGHT
            {
                if (frameMats.Length > 0) frameMats[0] = frameE0_On;
                if (frameMats.Length > 1) frameMats[1] = frameE1_On;
            }
            frameMesh.materials = frameMats;
        }

        // 5. Update environment lighting
        DynamicGI.UpdateEnvironment();
    }
}