using UnityEngine;

public class SimpleTeleport : MonoBehaviour
{
    [SerializeField] private GameObject subject;
    [SerializeField] private GameObject teleportPos;

    public void Teleport()
    {
        if (subject.GetComponentInChildren<CharacterController>() != null)
        {
            CharacterController controller = subject.GetComponentInChildren<CharacterController>();
            controller.enabled = false;
        }

        subject.transform.position = teleportPos.transform.position;
        subject.transform.rotation = teleportPos.transform.rotation;

        if (subject.GetComponentInChildren<CharacterController>() != null)
        {
            CharacterController controller = subject.GetComponentInChildren<CharacterController>();
            controller.enabled = true;
        }
    }
}
