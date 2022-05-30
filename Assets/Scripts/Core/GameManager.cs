using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] public bool gameIsPaused;
    [SerializeField] public GameObject bodyPartCollectable;

    [SerializeField] public int healthPassthrough;

    [SerializeField] public int soulCounter = 0;

    private void Start()
    {
        soulCounter = 0;
    }
    private void Awake()
    {
        gameIsPaused = false;
        
    }

    private void Update()
    {
        Pause();
        healthPassthrough = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().GetCurrentHealth;

    }


    private void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GetComponent<Pause>().PauseGame();
        }
    }
}
