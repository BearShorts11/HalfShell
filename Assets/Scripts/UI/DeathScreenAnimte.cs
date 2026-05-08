using UnityEngine;

public class DeathScreenAnimte : MonoBehaviour
{
    RectTransform rectTransform;
    [SerializeField] float minSlashMax = 440;
    [SerializeField] float swapSpeed = 1.5f;
    float nextTimeToSwap;
    int index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        nextTimeToSwap = Time.unscaledTime + swapSpeed;
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("updating");
        if (nextTimeToSwap <= Time.unscaledTime)
        {
            switch (index)
            {
                case 0:
                    rectTransform.localPosition = new Vector3(0, minSlashMax, 0);
                    break;
                case 1:
                    rectTransform.localPosition = Vector3.zero;
                    break;
                case 2:
                    rectTransform.localPosition = new Vector3(0, -minSlashMax, 0);
                    break;
            }
            index++;
            if (index > 2) index = 0;
            nextTimeToSwap = Time.unscaledTime + swapSpeed;
        }
    }
}
