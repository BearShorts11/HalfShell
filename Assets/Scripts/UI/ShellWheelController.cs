using UnityEngine;
using UnityEngine.UI;

public class ShellWheelController : MonoBehaviour
{
    public Animator shellWheelAnim;
    public Animator slowOverlayAnim;
    public static bool shellWheelSelected = false;
    public Image selectedItem;
    public Sprite noImage;
    public static int shellID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.paused != true)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                PlayerBehavior.UnlockCursor();
                PlayerBehavior.SlowMoActive = true;
                shellWheelSelected = true;
            }

            if (Input.GetKeyUp(KeyCode.Tab))
            {
                PlayerBehavior.LockCursor();
                PlayerBehavior.SlowMoActive = false;
                shellWheelSelected = false;
            }
        }

        if (shellWheelSelected)
        {
            shellWheelAnim.SetBool("OpenShellWheel", true);
            slowOverlayAnim.SetBool("OpenShellWheel", true);
        }

        else
        {
            shellWheelAnim.SetBool("OpenShellWheel", false);
            slowOverlayAnim.SetBool("OpenShellWheel", false);
        }

        switch (shellID)
        {
            // For future reference: This where we call associated unique animations, functions, sounds, etc. for all shell types -

            case 0: // Nothing is slected
                break;
            case 1: // Buckshot
                Debug.Log("Buckshot loaded");
                break;
            case 2: // Slug
                Debug.Log("Slug loaded");
                break;
        }
    }
}
