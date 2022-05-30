using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private int requiredSoulsforWin;


    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (gameManager.soulCounter >= requiredSoulsforWin)
            {
                uiManager.WinContainer.SetActive(true);
            }
            else return;
        }
    }


}
