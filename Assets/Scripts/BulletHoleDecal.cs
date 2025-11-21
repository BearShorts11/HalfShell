using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BulletHoleDecal : MonoBehaviour
{
    Transform parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(this.gameObject, 30f);
        parent = transform.parent;

    }

    // Update is called once per frame
    void Update()
    {
        //kill if parent dies
        if (parent is null) Destroy(this.gameObject);
    }


    //https://discussions.unity.com/t/how-to-fade-out-decals-projector-based-on-time/946801/2 

    [SerializeField]
    private float VisibleDuration = 28f;
    [SerializeField]
    private float FadeDuration = 2f;

    private DecalProjector _decalProjector;

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
