using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpriteController : MonoBehaviour
{
    [SerializeField] GameObject spriteState1, spriteState2, spriteState3, spriteState4;


    void Update()
    {
        UpdateActiveSprite();
    }


    void UpdateActiveSprite()
    {
        if (GetComponent<PlayerController>().CurrentlyDying)
        {
            spriteState1.SetActive(false);
            spriteState2.SetActive(false);
            spriteState3.SetActive(false);
            spriteState4.SetActive(false);
        }
        if (GetComponent<PlayerController>().playerState1)
        {
            spriteState1.SetActive(true);
            spriteState2.SetActive(false);
            spriteState3.SetActive(false);
            spriteState4.SetActive(false);
        }
        if (GetComponent<PlayerController>().playerState2)
        {
            spriteState1.SetActive(false);
            spriteState2.SetActive(true);
            spriteState3.SetActive(false);
            spriteState4.SetActive(false);
        }
        if (GetComponent<PlayerController>().playerState3)
        {
            spriteState1.SetActive(false);
            spriteState2.SetActive(false);
            spriteState3.SetActive(true);
            spriteState4.SetActive(false);
        }
        if (GetComponent<PlayerController>().playerState4)
        {
            spriteState1.SetActive(false);
            spriteState2.SetActive(false);
            spriteState3.SetActive(false);
            spriteState4.SetActive(true);
        }

    }
}
