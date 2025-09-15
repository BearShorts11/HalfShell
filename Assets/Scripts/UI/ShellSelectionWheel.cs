using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShellSelectionWheel : MonoBehaviour
{
    public int ID;
    private Animator anim;
    public string itemName;
    public TextMeshPro itemText;
    public Image selectedItem;
    private bool selected = false;
    public Sprite icon;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            selectedItem.sprite = icon;
            itemText.text = itemName;

        }
    }

    public void Selected() => selected = true;
    public void Deselected() => selected = false;
    public void HoverEnter()
    {
        anim.SetBool("Hover", true);
        itemText.text = itemName;
    }
    public void HoverExit()
    {
        anim.SetBool("Hover", false);
        itemText.text = "";
    }


    public static void OpenShellWheel(GameObject shellSelectionMenu)
    {
        Debug.Log("opened shell selection menu");
        



    }
}
