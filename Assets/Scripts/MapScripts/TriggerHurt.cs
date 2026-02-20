using Assets.Scripts;
using UnityEngine;

/// <summary>
/// A script for trigger volumes (or trigger only colliders) that damages players within it's bounds
/// 
/// Right now it only checks for players but in the future we could add more to it or change it
/// </summary>
public class Trigger_Hurt : MonoBehaviour
{
    public float damage = 0;
    public float damageInterval = 1;

    private float timeToNextDamage;
    private IDamageable damageable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            damageable = other.gameObject.GetComponent<IDamageable>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Time.time > timeToNextDamage)
            {
                timeToNextDamage = Time.time + damageInterval;
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            damageable = null;
        }
    }


}
