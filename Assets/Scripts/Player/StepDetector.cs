using UnityEngine;

public class StepDetector : MonoBehaviour
{
    CharacterController cc;
    private float DefaultStepOffset;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cc = FindFirstObjectByType<CharacterController>();
        DefaultStepOffset = cc.stepOffset;
    }

    private void OnTriggerEnter(Collider other)
    {
        cc.stepOffset = 0.5f;
    }

    private void OnTriggerExit(Collider other)
    {
        cc.stepOffset = DefaultStepOffset;
    }
}
