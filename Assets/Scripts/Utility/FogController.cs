using UnityEngine;

public class FogController : MonoBehaviour
{
    public void FogEnable(bool check)
    { 
        RenderSettings.fog = check;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
    }
    public void FogColor(string hexCode)
    {
        ColorUtility.TryParseHtmlString(hexCode, out Color hexColor);
        RenderSettings.fogColor = hexColor;
    }
    public void FogDensity(float density) => RenderSettings.fogDensity = density;
}
