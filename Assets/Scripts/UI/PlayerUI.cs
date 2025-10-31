using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private PlayerBehavior player;

    // The parent object for all static UI elements (Chamber/Load, HP Bar, etc)
    // Exclude crosshairs from this because it gets real disorienting
    public GameObject HUD;
    public Animator UIRattleAnim;

    public Slider healthBar;
    public TextMeshProUGUI HPtext;

    public Slider armorBar;
    public TextMeshProUGUI armorText;

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
            UpdateHP(player.Health, player.MaxHP);
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
        UIRattle(1);
        UpdateHP(player.Health, player.MaxHP);
        CheckHealth();
        hurtOverlayAnim.Play(Animator.StringToHash("Base Layer.Hurt Overlay Enter"));
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

    public void UIRattle(int type)
    {
        UIRattleAnim.SetInteger("Type", type);
        UIRattleAnim.Play(Animator.StringToHash("Base Layer.UI Rattle"));
        UIRattleAnim.SetInteger("Type", 0);
    }

    public void UpdateHP(float HP, float Max)
    {
        healthBar.value = HP; 
        HPtext.text = $"HP: {Mathf.Round(HP)}";
    }
    public void UpdateMaxHP(float HP, float Max)
    {
        healthBar.maxValue = Max;
        HPtext.text = $"HP: {Mathf.Round(HP)}";
    }

    public void UpdateArmor(float Arm, float Max)
    {
        armorBar.value = Arm;
        if (armorBar.value > 0)
        {
            armorText.enabled = true;
            armorText.text = $"ARM: {Mathf.Round(Arm)}";
        }
        else { armorText.enabled = false; }
    }
    public void UpdateMaxArmor(float Arm, float Max)
    {
        armorBar.maxValue = Max;
        armorText.text = $"ARM: {Mathf.Round(Arm)}";
    }
}
