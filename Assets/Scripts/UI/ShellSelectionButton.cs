using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShellSelectionButton : MonoBehaviour
{
    public int ID;
    private Animator anim;
    public string itemName;
    public TextMeshProUGUI itemText;
    private bool selected = false;

    public TextMeshProUGUI ammoText;
    private PlayerShooting player;
    private ShellBase.ShellType type;

    private Button button;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        anim = GetComponent<Animator>();
        player = FindFirstObjectByType<PlayerShooting>();
        button = GetComponent<Button>();
        SetShellType();
        UpdateAmmoCount();
    }

    // Update is called once per frame
    void Update()
    {
        if (selected) { itemText.text = itemName; }
        UpdateAmmoCount();
    }

    public void Click()
    {
        selected = true;
        UpdateAmmoCount();
        selected = false;
    }

    public void HoverEnter()
    {
        if (button.interactable)
        {
            anim.SetBool("Hovered", true);
            itemText.text = itemName;
        }
    }
    public void HoverExit()
    {
        anim.SetBool("Hovered", false);
        itemText.text = "";
    }

    private void SetShellType()
    {
        switch (ID)
        {
            case 0: // Half Shell
                type = ShellBase.ShellType.HalfShell;
                break;
            case 1: // Slug
                type = ShellBase.ShellType.Slug;
                break;
            default:
                type = ShellBase.ShellType.Buckshot;
                break;
        }
    }

    public void UpdateAmmoCount()
    {
        if (ammoText != null)
        { 
            if (type != ShellBase.ShellType.Buckshot) { ammoText.text = $"{player.AmmoCounts[type]}"; }
            else { ammoText.text = $""; }
        }
        CheckActive();
    }

    private void CheckActive()
    {
        if (player.AmmoCounts[type] > 0) 
        { 
            button.interactable = true;
            anim.SetBool("Active", true);
        }
        else 
        { 
            button.interactable = false;
            anim.SetBool("Active", false);
        }
    }
}
