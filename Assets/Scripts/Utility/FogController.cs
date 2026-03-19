using UnityEngine;
using System.Collections;

public class FogController : MonoBehaviour
{
    public float ChangeTime = 0;
    Coroutine FogChange;

    public void FogEnable(bool check)
    { 
        RenderSettings.fog = check;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
    }
    public void FogColor(string hexCode)
    {
        ColorUtility.TryParseHtmlString(hexCode, out Color hexColor);
        if (ChangeTime > 0) { ActivateLerp(hexColor, RenderSettings.fogDensity); }
        else { RenderSettings.fogColor = hexColor; }
    }
    public void FogDensity(float density)
    {
        if (ChangeTime > 0) { ActivateLerp(RenderSettings.fogColor, density); }
        else { RenderSettings.fogDensity = density; }
    }
    public void SetChangeTime(float time) => ChangeTime = time; 

    public void ActivateLerp(Color color, float density)
    {
        if (FogChange != null) { StopCoroutine(FogChange); }
        FogChange = StartCoroutine(LerpFog(color, density));
    }

    private IEnumerator LerpFog(Color color, float density)
    {
        Color startColor = RenderSettings.fogColor;
        float startDensity = RenderSettings.fogDensity;

        float elapsedTime = 0f;

        while (elapsedTime < ChangeTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / ChangeTime);

            RenderSettings.fogColor = Color.Lerp(startColor, color, t);
            RenderSettings.fogDensity = Mathf.Lerp(startDensity, density, t);

            yield return null;
        }

        ChangeTime = 0f;
        RenderSettings.fogColor = color;
        RenderSettings.fogDensity = density;
    }
}
