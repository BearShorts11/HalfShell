using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
    private GameObject shellObject;
    private Transform shellEjectionTransform;

    private void Start()
    {
        shellObject = GameObject.Find("Shell");
        shellEjectionTransform = GameObject.Find("Shell_Eject").transform;
        //shellObject.SetActive(false);
    }
    //if this isn't used in the next few weeks can I get rid of this? -N
    // You may. -V
    public void PlaySound(string path)
    {
        RuntimeManager.PlayOneShotAttached(path, this.gameObject);
    }


    #region Shotgun related animation event methods
    
    // will these work in the animation event once we have modular shells set up? -V
    private void HideShellModel()
    {
        if (shellObject.activeSelf)
            shellObject.SetActive(false);
        //Debug.Log("Shell Invisible!");
    }
    private void ShowShellModel()
    {
        if (!shellObject.activeSelf)
            shellObject.SetActive(true);
        //Debug.Log("Shell Visible!");
    }

    public void EjectShell()
    {
        // TODO: Find a way to link this to the player shooting script and know what shell to eject based on what the shotgun just shot or what shell is sitting in the chamber
        GameObject playerObject = GameObject.Find("Player");
        PlayerShooting shooting = playerObject.GetComponent<PlayerShooting>();
        if (shooting.ShellInChamber() == false) return;

        EjectShell(Resources.Load<GameObject>("Shell_Ejection/Shell_Generic"));
    }
    private void EjectShell(GameObject shell)
    {
        shell = Instantiate(shell, shellEjectionTransform.position, Quaternion.LookRotation(shellEjectionTransform.right));
        shell.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.right * Random.Range(150f, 300f));
        shell.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.up * Random.Range(75f, 210f));
        shell.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-15f, 15f), Random.Range(30f, 50f), Random.Range(-5f,-5f)));
    }
    #endregion
}
