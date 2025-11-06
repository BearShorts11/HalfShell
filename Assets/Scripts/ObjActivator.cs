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
    }
    public void ObjSwap(ShellBase.ShellType shellType)
    {
        if (shellType == targetShell)
        {
            IntialObj.SetActive(false);
            SwapToObj.SetActive(true);
        }
    }
}
