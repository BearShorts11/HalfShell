using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private PlayerBehavior player;

    public GameObject hurtOverlay;
    public Animator hurtOverlayAnim;
    public bool lowHealth = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        // TESTING INPUTS 
        if (Input.GetKeyDown(KeyCode.Z))
        {
            player.Damage(10f);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            player.Health += 10;
            CheckHealth();
        }
    }

    //THIS IS BAD DON'T LOOK AT THIS I FIX LATER -N
    public static GameObject MakeUIShell(Image parent, ShellBase shell)
    {
        GameObject UIshell = new GameObject();
        Image display = UIshell.AddComponent<Image>();
        RectTransform UIShellRectTransform = UIshell.GetComponent<RectTransform>();
        UIShellRectTransform.SetParent(parent.transform, false);

        //put this somewhere better dumbass -N
        float shellWidth = 30f;
        float shellHeight = 70f;
        float halfShellHeight = 35f;

        Color color = shell.DisplayColor;
        UIshell.GetComponent<Image>().color = color;

        //set size based on full or half shell
        switch (shell.Type)
        {
            case ShellBase.ShellType.HalfShell:
                UIShellRectTransform.sizeDelta = new Vector2(shellWidth, halfShellHeight);
                break;
            default:
                UIShellRectTransform.sizeDelta = new Vector2(shellWidth, shellHeight);
                break;
        }

        return UIshell;
    }


    public void Hurt()
    {
        CheckHealth();
        hurtOverlayAnim.Play(Animator.StringToHash("Base Layer.Hurt Overlay Enter"));

        //if (lowHealth)
        //{
        //    //play anim, keep overlay up and pulsate (???)
        //    hurtOverlayAnim.Play(Animator.StringToHash("Base Layer.Hurt Overlay Enter"));
        //}
        //else
        //{
        //    hurtOverlayAnim.Play(Animator.StringToHash("Base Layer.Hurt Overlay Enter"));
        //}
    }

    public void CheckHealth()
    {
        // TO-DO: Change hard-coded values to check for percentage of HP
        if (player.Health <= 25)
        {
            lowHealth = true;
            hurtOverlayAnim.SetBool("lowHealth", true);
        }
        else
        {
            lowHealth = false;
            hurtOverlayAnim.SetBool("lowHealth", false);
        }
    }

}
