using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DecalFadeOut : MonoBehaviour
{
    Transform parent;
    public bool needsParent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(this.gameObject, 30f);
        parent = transform.parent;
        if (Decals.Length > 0 )
        {
            _decalProjector.material = Decals[Random.Range(0, Decals.Length)];
        }
    }

    // Update is called once per frame
    void Update()
    {
        //kill if parent dies- bullet holes only
        if (needsParent && parent is null)
        { 
            Destroy(this.gameObject);
            Debug.Log("destroyed decal- no parent");
        }
    }


    //https://discussions.unity.com/t/how-to-fade-out-decals-projector-based-on-time/946801/2 

    [SerializeField]
    private float VisibleDuration = 28f;
    [SerializeField]
    private float FadeDuration = 2f;

    private DecalProjector _decalProjector;

    [SerializeField] public Material[] Decals;

    private void OnEnable()
    {
        _decalProjector = GetComponent<DecalProjector>();
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(VisibleDuration);

        float elapsed = 0;
        float initialFactor = _decalProjector.fadeFactor;

        while (elapsed < 1)
        {
            _decalProjector.fadeFactor = Mathf.Lerp(initialFactor, 0, elapsed);
            elapsed += Time.deltaTime / FadeDuration;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
