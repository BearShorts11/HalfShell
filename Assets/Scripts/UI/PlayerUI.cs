using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerBehavior player;
    public PlayerShooting gun;

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

    public UI_Message messageText;

    public TextMeshProUGUI currentCapacityText;
    public Image ChamberUI;
    public Image SingleShotCrosshair;
    public Image MultiShotCrosshair;

    //gets around needing a static reference for a static method (Make UI Shell) but needing to assign in the inspector
    public Sprite HalfShellSprite;
    public Sprite SlugSprite;
    public Sprite FireShellSprite;
    private static Sprite _halfShellSprite;
    private static Sprite _slugSprite;
    private static Sprite _fireShellSprite;

    //UI shell size vals
    static float shellHeight = 30f;
    static float shellWidth = 70f;
    static float halfShellWidth = 35f;
    float shellUIstart;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerBehavior>();

        gun = FindFirstObjectByType<PlayerShooting>();
        if (gun.Chamber is not null)
        {
            ChamberUIOn(gun.Chamber);
        }

        _halfShellSprite = HalfShellSprite;
        _slugSprite = SlugSprite;
        _fireShellSprite = FireShellSprite;
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
            case ShellBase.ShellType.Incindiary:
                UIshell.GetComponent<Image>().sprite = _fireShellSprite;
                UIShellRectTransform.sizeDelta = new Vector2(shellWidth, shellHeight);
                UIshell.GetComponent <Image>().color = shell.DisplayColor;
                break;
            default:
                UIshell.GetComponent<Image>().sprite = _slugSprite;
                UIShellRectTransform.sizeDelta = new Vector2(shellWidth, shellHeight);
                break;
        }

        if (forMagazine) { UIShellRectTransform.localScale = new Vector3(0.8f, 0.8f, 1); }

        return UIshell;
    }

    public void MagazineUILoss(ShellBase shell)
    {
        // Transform out of bound error fix (5 + 1 in the chamber) -V
        if (gun.currentCapacity < gun.totalCapacity)
            Destroy(gun.magazineUI.transform.GetChild(gun.magUI.Count -1).gameObject);

        //shift position forward of all children
        for (int i = 0; i < gun.Magazine.Count; i++)
        {
            Vector3 adjustPosition = Vector3.zero;
            if (shell.Type == ShellBase.ShellType.HalfShell) adjustPosition.x = -30;
            else adjustPosition.x = -60;

            gun.magazineUI.transform.GetChild(i).GetComponent<RectTransform>().localPosition += adjustPosition;
        }
    }

    public void LoadMagUI(ShellBase shell)
    {
        LoadMagUI2(shell);
        return;


        GameObject UIshell = MakeUIShell(gun.magazineUI, shell, true);

        //set position based on capacity, shell size, & buffer
        float size = shell.Size;
        float y = 0;
        y = -122;
        if (shell.Type == ShellBase.ShellType.HalfShell) y -= 14f; //half of halfshell width //should be 14 for adjusted size
        shellUIstart = y;

        for (int i = 1; i < gun.magUI.Count; i++)
        {
            if (gun.magUI[i - 1].Type == ShellBase.ShellType.HalfShell) y += 30;
            else y += 60;
        }

        UIshell.GetComponent<RectTransform>().localPosition = new Vector3(y, 0, 0);
        UIshell.SetActive(true);
    }

    public void LoadMagUI2(ShellBase shell)
    {
        GameObject UIshell = MakeUIShell(gun.magazineUI, shell, true);

        //shift position forward of all children
        for (int i = 0; i < gun.Magazine.Count; i++)
        {
            Vector3 adjustPosition = Vector3.zero;
            if (shell.Type == ShellBase.ShellType.HalfShell) adjustPosition.x = 30;
            else adjustPosition.x = 60;

                gun.magazineUI.transform.GetChild(i).GetComponent<RectTransform>().localPosition += adjustPosition;
        }

        float y = -122;
        if (shell.Type == ShellBase.ShellType.HalfShell) y -= 14f;
        UIshell.GetComponent<RectTransform>().localPosition = new Vector3(y, 0, 0);
        UIshell.SetActive(true);
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

    public void UI_SetMessage(string message)
    {
        messageText.SetMessage(message);
    }

    public void UI_SetMessage(string message, float time)
    {
        messageText.SetMessage(message, time);
    }

}