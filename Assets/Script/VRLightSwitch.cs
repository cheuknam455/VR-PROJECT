using UnityEngine;
using UnityEngine.Rendering;

public class VRLightSwitch : MonoBehaviour
{
    public Light sunLight;
    public Material daySkybox;
    public Material nightSkybox;

    public void ToggleSun()
    {
        // 1. Flip the light switch first
        sunLight.enabled = !sunLight.enabled;

        // 2. Check the NEW state to decide the skybox
        if (sunLight.enabled == true) 
        {
            RenderSettings.skybox = daySkybox;
            Debug.Log("Switched to Morning");
        }
        else 
        {
            RenderSettings.skybox = nightSkybox;
            Debug.Log("Switched to Night");
        }

        // 3. Force Unity to redraw the sky
        DynamicGI.UpdateEnvironment();
    }
}