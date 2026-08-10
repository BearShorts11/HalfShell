using UnityEngine;

// World Canvas Billboard Sprite Behavior
public enum FadeMode
{
    None = 0,
    FadeIn = 1,
    FadeOut = 2
}
public class Canvas2DSpriteBehavior : MonoBehaviour
{
    public bool billboard = true;
    [SerializeField] private bool hidden = false;
    public bool bHidden { 
        private set { hidden = value; } 
        get { return hidden; } 
    }
    [Range(0f,5f)]public float fadeSpeed = 1f;
    private float fadeTime
    {
        get {  return fadeSpeed * Time.fixedDeltaTime; }
    }

    [SerializeField] protected Canvas canvas;
    [SerializeField] protected CanvasGroup canvasGroup;
    Transform cam;
    Vector3 initScale = Vector3.one;
    private FadeMode transition = FadeMode.None;
    private float time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        cam = Camera.main.transform;
        canvas = canvas != null ? canvas : gameObject.GetComponent<Canvas>();
        canvasGroup = canvasGroup != null ? canvasGroup : gameObject.GetComponent<CanvasGroup>();
        initScale = canvas.transform.localScale;

        if (canvasGroup != null)
            canvasGroup.alpha = bHidden ? 0 : 1;

        HidePrompt(bHidden);
    }

    public void SetFadeSpeed(float newFadeSpeed)
    {
        fadeSpeed = Mathf.Clamp(newFadeSpeed, 0f, 5f);
    }

    /// <summary>
    /// To show or hide the sprite/prompt object smoothly by fading it in or out.
    /// Sets transition mode instead of controlling the Canvas Group alpha directly.
    /// </summary>
    /// <param name="bHide">True: Fade the object out, False: Fade the object in</param>
    public void HidePrompt(bool bHide)
    {
        bHidden = bHide;

        //if (canvas != null)
        //    canvas.enabled = bShow;

        if (canvasGroup == null) return;

        if (!bHide)
        {
            transition = FadeMode.FadeIn;
        }
        else
        {
            transition = FadeMode.FadeOut;
        }
    }

    /// <summary>
    /// Hide the sprite/prompt instantly with no fade out. Directly controls Canvas Group Alpha
    /// </summary>
    /// <param name="bHide"></param>
    public void HidePromptInstant(bool bHide)
    {
        bHidden = bHide;

        if (canvasGroup == null) return;

        canvasGroup.alpha = bHide ? 0 : 1;

        transition = FadeMode.None;
    }

    public void FadeInPrompt()
    {
        if (canvasGroup.alpha != 1)
        {
            canvasGroup.alpha += fadeTime;
            canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha);
        }
        else
            transition = FadeMode.None;
    }

    public void FadeOutPrompt()
    {
        if (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= fadeTime;
            canvasGroup.alpha = Mathf.Clamp01(canvasGroup.alpha);
        }
        else
            transition = FadeMode.None;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if ((!bHidden && canvasGroup.alpha != 0) && billboard)
        {
            // Billboard effect
            // I looked up how to billboard and the tutorials pretty much go to the LookAt method
            //  but the lookAt method can rotate the thing in the wrong direction if the player is
            //  directly above or below it.
            // Setting the transform.forward to the player camera is much better but the problem
            //  remains the same.
            // Ideally, this billboard effect should be disabled for when there is a proper UI art
            //  and it has the shader material/sprite to set up the billboard effect.
            {
                //canvas.transform.LookAt(cam, Vector3.up);
                canvas.transform.rotation = cam.rotation;
                //canvas.transform.Rotate(0, 180, 0);
            }
            //canvas.transform.forward = -cam.forward;

            // Scale the prompt element according to the player's distance to keep it noticeable AND readable
            canvas.transform.localScale = initScale * Mathf.Clamp((Vector3.Distance(this.transform.position, Camera.main.transform.position)) * 0.2f, 0.05f, 2);
        }
        if (transition != FadeMode.None)
        {
            time += fadeTime;
            if (time >= fadeTime)
            {
                switch (transition)
                {
                    case FadeMode.FadeIn:
                        FadeInPrompt(); 
                        break;
                    case FadeMode.FadeOut:
                        FadeOutPrompt();
                        break;
                    default:
                        break;
                }
                time = 0;
            }
        }
    }
}
