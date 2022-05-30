using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDealDamage : MonoBehaviour
{
    [SerializeField, Header("DAMAGE VARIABLES")] private int damageToApply = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        if (other.gameObject.tag == "Enemy")
        {
            damageable.TakeDamage(damageToApply);
        }
    }

}
