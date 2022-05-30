using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPickup : MonoBehaviour
{
    private GameManager gameManager;

    private void OnEnable()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
                gameManager.soulCounter++;
                Destroy(this.gameObject);
        }
    }
}
