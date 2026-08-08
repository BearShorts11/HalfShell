using UnityEngine;

/// <summary>
/// Behavior that makes a hint show up when the player has low hp.
/// Controls Canvas2DSpriteBehavior
/// </summary>
public class LowHealthHintBehavior : MonoBehaviour
{
    [SerializeField] Canvas2DSpriteBehavior hint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (hint == null)
        {
            if (gameObject.TryGetComponent<Canvas2DSpriteBehavior>(out Canvas2DSpriteBehavior component))
            {
                hint = component;
            }
            else
            {
                component = gameObject.GetComponentInChildren<Canvas2DSpriteBehavior>();
                if (component != null)
                    hint = component;
                else
                    Debug.LogError("Error, Canvas2DSpriteBehavior component not found!");
            }
        }
        FindFirstObjectByType<PlayerBehavior>().UpdateHintList(this);
    }

    public void ShowHints(bool bShow)
    {
        // Inverting this since the function in Canvas2DSprite behavior is about hiding them
        bShow = !bShow;

        if (hint != null) 
        {
            hint.HidePrompt(bShow);
        }
    }
}
