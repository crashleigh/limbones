using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartCollectable : MonoBehaviour
{
    [SerializeField] int HealAmount = 1;
    [SerializeField] GameObject Sprite1, Sprite2, Sprite3, Sprite4;
    [SerializeField] GameManager gameManager;

    float force = 100f;

    private void OnEnable()
    {
        StartCoroutine(OnSpawn());
        gameManager = FindObjectOfType<GameManager>();
    }
    private void Update()
    {
        ActivateAppropriateSprite();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<Health>().GetCurrentHealth != other.gameObject.GetComponent<Health>().GetMaxHealth)
            {
                other.gameObject.GetComponent<Health>().IncreaseHealth(HealAmount);
                Destroy(this.gameObject);
            }
            else
            {
                return;
            }
        }
    }

    private IEnumerator OnSpawn()
    {
        GetComponent<CircleCollider2D>().isTrigger = false;
        yield return new WaitForSeconds(5);
        GetComponent<CircleCollider2D>().isTrigger = true;
    }


    private void ActivateAppropriateSprite()
    {
        if (gameManager.healthPassthrough == 3)
        {
            Sprite1.SetActive(true);
            Sprite2.SetActive(false);
            Sprite3.SetActive(false);
            Sprite4.SetActive(false);
        }
        if (gameManager.healthPassthrough == 2)
        {
            Sprite1.SetActive(false);
            Sprite2.SetActive(true);
            Sprite3.SetActive(false);
            Sprite4.SetActive(false);
        }
        if (gameManager.healthPassthrough == 1)
        {
            Sprite1.SetActive(false);
            Sprite2.SetActive(false);
            Sprite3.SetActive(true);
            Sprite4.SetActive(false);
        }
    }

}
