using UnityEngine;

public class DeathDetector : MonoBehaviour
{
    [SerializeField][Tooltip("Killable Enemy(s) that enable or disable an object.")]
    private Enemy TargetEnemy;
    private GameObject toggledObject;

    private bool enemyKilled = false;
    private bool enableObject = false;
    private bool finished = false;

    private void Start()
    {
        finished = false;
        enemyKilled = false;
    }
    void Update()
    {
        if(finished) Destroy(this);

        if (EnemyCheck() && enableObject == false)
        {
            toggledObject.gameObject.SetActive(false);
            finished = true;

        }
        else if (EnemyCheck() && enableObject == true)
        {
            toggledObject.gameObject.SetActive(true);
            finished = true;
        }
    }

    private bool EnemyCheck()
    {
        if (TargetEnemy) enemyKilled = false;
        else if (!TargetEnemy) enemyKilled = true;
        return enemyKilled;
    }
}
