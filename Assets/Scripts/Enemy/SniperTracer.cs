using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SniperTracer : MonoBehaviour
{
    public LineRenderer line { get; set; }
    float step { get => Time.deltaTime * fadeSpeed; }

    float lineWidth = 0f;
    [SerializeField] AnimationCurve fadeOutCurve;
    [SerializeField] float fadeSpeed = 3f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        line = GetComponent<LineRenderer>();
        lineWidth = line.startWidth;
        StartCoroutine(FadeAway());
    }

    IEnumerator FadeAway()
    {
        float time = 0;
        while (time < 1)
        {
            // This doesn't seem to effect the tracer in the game while it does in the scene view...
            // MEANWHILE, DOING IT ON THE WIDTHMULTIPLIER BREAKS IT'S VISIBILITY ENTIRELY, WHAT.
            // So I set both widthMultiplier AND start and endwidth???
            line.widthMultiplier = 1f;
            line.widthMultiplier += (step);
            lineWidth += (step);
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.startColor = new Color(1, 1, 1, fadeOutCurve.Evaluate(Mathf.Lerp(1, 0, time)));
            line.endColor = line.startColor;
            time += step;
            yield return new WaitForSeconds(step);
        }
        Destroy(this.gameObject);
    }
}
