using UnityEngine;
using UnityEngine.UI;

public class ShellWheelController : MonoBehaviour
{
    public Animator anim;
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
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PlayerBehavior.UnlockCursor();
            shellWheelSelected = true;
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            PlayerBehavior.LockCursor();
            shellWheelSelected = false;
        }

        if (shellWheelSelected)
        {
            anim.SetBool("OpenShellWheel", true);
        }

        else
        {
            anim.SetBool("OpenShellWheel", false);
        }

        switch (shellID)
        {
            // For future reference: This where we call associated unique animations, functions, sounds, etc. for all shell types -

            case 0: // Nothing is slected
                selectedItem.sprite = noImage;
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
