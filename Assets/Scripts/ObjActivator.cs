using UnityEngine;

public class ObjActivator : MonoBehaviour
{
    [SerializeField]
    private GameObject IntialObj;
    [SerializeField]
    private GameObject SwapToObj;
    [SerializeField]
    private ShellBase.ShellType targetShell;

    private void Awake()
    {
        IntialObj.SetActive(true);
        SwapToObj.SetActive(false);
        Debug.Log("Ready to switch");
    }

    public void ObjSwap(ShellBase.ShellType shellType)
    {
        if (shellType == targetShell)
        {
            IntialObj.SetActive(false);
            SwapToObj.SetActive(true);
            Debug.Log("Switched");
        }
    }

    public void ObjSwapNoShell()
    {
        IntialObj.SetActive(false);
        SwapToObj.SetActive(true);
        Debug.Log("Switched");
    }
}
