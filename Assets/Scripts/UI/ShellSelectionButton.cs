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
            itemText.text = itemName;
        }
    }

    public void Click()
    {
        selected = true;
        ShellWheelController.shellID = ID;
        selected = false;
        ShellWheelController.shellID = 0;
    }

    public void HoverEnter()
    {
        anim.SetBool("Hovered", true);
        itemText.text = itemName;
    }
    public void HoverExit()
    {
        anim.SetBool("Hovered", false);
        itemText.text = "";
    }


    //public static void OpenShellWheel(GameObject shellSelectionMenu)
    //{
    //    Debug.Log("opened shell selection menu");
        



    //}
}
