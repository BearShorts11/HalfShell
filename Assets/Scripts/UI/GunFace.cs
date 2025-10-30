using UnityEngine;

public class GunFace : MonoBehaviour
{
    [SerializeField] private Animator gunFaceAnimator;
    private float animationSpeed = 1;
    public bool debugging = true;
    [Tooltip("isTalking - Is only used for the scene view with the inspector while play mode is on. Sets the drawn face on the shotgun to a talking animation state on true")]
    public bool isTalking = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gunFaceAnimator == null)
        {
            gunFaceAnimator = GameObject.Find("Face").GetComponent<Animator>();
            Debug.LogError("Error! gunFaceAnimator field not set! Finding Game Object named \"Face\"...");
            if (gunFaceAnimator == null)
                Debug.Log("No GameObject named \"Face\"!!!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (debugging && Input.GetKeyDown(KeyCode.Period))
            if (isTalking)
                StopTalking();
            else
                Talk();
    }

    void FixedUpdate()
    {
        gunFaceAnimator.speed = Time.timeScale < 1 ? 1 / Time.timeScale : 1;
    }

    public float AnimationSpeed
    {
        get { return animationSpeed; }
        set 
        { 
            animationSpeed = value;
            if (gunFaceAnimator != null)
                gunFaceAnimator.speed = animationSpeed;
        }
    }

    public void Talk()
    {
        isTalking = true;
        gunFaceAnimator.Play("GunFace_Talk");
    }

    public void StopTalking()
    {
        isTalking = false;
        gunFaceAnimator.Play("GunFace_Idle");
    }

    void OnDrawGizmos()
    {
        if (gunFaceAnimator == null) return;

        if (isTalking)
        {
            Talk();
        }
        else
        {
            StopTalking();
        }
    }
}
