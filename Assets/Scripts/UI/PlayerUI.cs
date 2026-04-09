using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerBehavior player;

    // The parent object for all static UI elements (Chamber/Load, HP Bar, etc)
    // Exclude crosshairs from this because it gets real disorienting
    public GameObject HUD;
    public Animator UIRattleAnim;

    public Slider healthBar;
    public TextMeshProUGUI HPtext;

    public GameObject armorParent;
    public Slider armorBar;
    public TextMeshProUGUI armorText;

    public GameObject hurtOverlay;
    public Animator hurtOverlayAnim;
    public bool lowHealth = false;

    public TextMeshProUGUI currentCapacityText;
    public Image ChamberUI;
    public Image SingleShotCrosshair;
    public Image MultiShotCrosshair;

    //gets around needing a static reference for a static method (Make UI Shell)
    public Sprite HalfShellSprite;
    public Sprite SlugSprite;
    private static Sprite _halfShellSprite;
    private static Sprite _slugSprite;

    //UI shell size vals
    static float shellHeight = 30f;
    static float shellWidth = 70f;
    static float halfShellWidth = 35f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerBehavior>();

        PlayerShooting gun = FindFirstObjectByType<PlayerShooting>();
        if (gun.Chamber is not null)
        {
            ChamberUIOn(gun.Chamber);
        }

        _halfShellSprite = HalfShellSprite;
        _slugSprite = SlugSprite;
    }

    // Update is called once per frame
    void Update()
    {
    }


    public static GameObject MakeUIShell(Image parent, ShellBase shell, bool forMagazine)
    {
        GameObject UIshell = new GameObject();
        UIshell.AddComponent<Image>();
        RectTransform UIShellRectTransform = UIshell.GetComponent<RectTransform>();
        UIShellRectTransform.SetParent(parent.transform, false);

        //set size based on full or half shell
        switch (shell.Type)
        {
            case ShellBase.ShellType.HalfShell:
                UIshell.GetComponent<Image>().sprite = _halfShellSprite;
                UIShellRectTransform.sizeDelta = new Vector2(halfShellWidth, shellHeight);
                break;
            default:
                UIshell.GetComponent<Image>().sprite = _slugSprite;
                UIShellRectTransform.sizeDelta = new Vector2(shellWidth, shellHeight);
                break;
        }

        if (forMagazine) { UIShellRectTransform.localScale = new Vector3(0.8f, 0.8f, 1); }

        return UIshell;
    }

    public void ChamberUIOff()
    {
        Destroy(ChamberUI.transform.GetChild(1).gameObject);
    }

    public void ChamberUIOn(ShellBase shell)
    {
        GameObject UIshell = MakeUIShell(ChamberUI, shell, false);
        UIshell.SetActive(true);
    }


    public void SwitchCrosshairUI(ShellBase chamber, float magCount)
    {
        if (chamber is not null)
        {
            ShellBase top = chamber;
            switch (top.Type)
            {
                case ShellBase.ShellType.Slug:
                    //case ShellBase.ShellType. some other shot type that also is a single fire
                    SingleShotCrosshair.gameObject.SetActive(true);
                    MultiShotCrosshair.gameObject.SetActive(false);
                    break;
                case ShellBase.ShellType.Buckshot:
                case ShellBase.ShellType.Incindiary:
                case ShellBase.ShellType.HalfShell:
                    SingleShotCrosshair.gameObject.SetActive(false);
                    MultiShotCrosshair.gameObject.SetActive(true);
                    break;
            }
        }

        //defaults to small crosshair for visibility
        if (chamber is null && magCount <= 0)
        {
            SingleShotCrosshair.gameObject.SetActive(true);
            MultiShotCrosshair.gameObject.SetActive(false);
        }
    }

    public void Hurt()
    {
        UIRattle(1);
        UpdateHP(player.Health, player.maxHealth);
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
            armorParent.SetActive(true);
            armorText.enabled = true;
            armorText.text = $"ARM: {Mathf.Round(Arm)}";
        }
        else { armorText.enabled = false; armorParent.SetActive(false); }
    }

    public void UpdateMaxArmor(float Arm, float Max)
    {
        armorBar.maxValue = Max;
        armorText.text = $"ARM: {Mathf.Round(Arm)}";
    }
}
