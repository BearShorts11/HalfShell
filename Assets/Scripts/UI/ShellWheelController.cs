using UnityEngine;
using UnityEngine.UI;

public class ShellWheelController : MonoBehaviour
{
    public Animator shellWheelAnim;
    public Animator slowOverlayAnim;
    public static bool shellWheelSelected = false;
    public static bool shellWheelDisabled = false;
    public Image selectedItem;
    public Sprite noImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shellWheelDisabled) return;

        if (PauseMenu.paused != true)
        {
            if (Input.GetKeyDown(KeyCode.Tab) | Input.GetKeyDown(KeyCode.LeftControl))
            {
                PlayerBehavior.UnlockCursor();
                PlayerBehavior.SlowMoActive = true;
                shellWheelSelected = true;
            }

            if (Input.GetKeyUp(KeyCode.Tab) | Input.GetKeyUp(KeyCode.LeftControl))
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
    }
}
