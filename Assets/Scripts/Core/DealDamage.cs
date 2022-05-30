using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    [SerializeField, Header("DAMAGE VARIABLES")] private int damageToApply = 1;
    [SerializeField] private float rejectionForce = 1000;

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        if (other.gameObject.tag == "Player")
        {
            if (other.GetComponent<PlayerController>().running) return;
            var dir = other.transform.position - transform.position;
            other.GetComponent<IPlayerCOntroller>().AddForce(dir * rejectionForce, PlayerForce.Decay);
            other.GetComponent<BodyPartSpawner>().SpawnBodyCollectable();
            
            damageable.TakeDamage(damageToApply);
        }
    }

}
