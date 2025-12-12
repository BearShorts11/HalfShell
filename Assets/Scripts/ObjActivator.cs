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
        if (IntialObj != null) { IntialObj.SetActive(true); }
        if (SwapToObj != null) { SwapToObj.SetActive(false); }
    }

    public void ObjSwap(ShellBase.ShellType shellType)
    {
        if (shellType == targetShell)
        {
            if (IntialObj != null) { IntialObj.SetActive(false); }
            if (SwapToObj != null) { SwapToObj.SetActive(true); }
        }
    }
}
