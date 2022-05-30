using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] private PlayerController pController;
    [SerializeField] GameObject spriteState1, spriteState2, spriteState3, spriteState4;

    private void Awake()
    {
        pController = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        UpdateActiveSprite();
    }

    private void FixedUpdate()
    {
        transform.position = player.transform.position;
    }


    void UpdateActiveSprite()
    {
        if (!pController.CurrentlyDying)
        {
            spriteState1.SetActive(false);
            spriteState2.SetActive(false);
            spriteState3.SetActive(false);
            spriteState4.SetActive(false);
        }

        if (pController.playerState1 && pController.CurrentlyDying)
        {
            spriteState1.SetActive(true);
            spriteState2.SetActive(false);
            spriteState3.SetActive(false);
            spriteState4.SetActive(false);
        }
        if (pController.playerState2 && pController.CurrentlyDying)
        {
            spriteState1.SetActive(false);
            spriteState2.SetActive(true);
            spriteState3.SetActive(false);
            spriteState4.SetActive(false);
        }
        if (pController.playerState3 && pController.CurrentlyDying)
        {
            spriteState1.SetActive(false);
            spriteState2.SetActive(false);
            spriteState3.SetActive(true);
            spriteState4.SetActive(false);
        }
        if (pController.playerState4 && pController.CurrentlyDying)
        {
            spriteState1.SetActive(false);
            spriteState2.SetActive(false);
            spriteState3.SetActive(false);
            spriteState4.SetActive(true);
        }

    }
}
