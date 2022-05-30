using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;




    public void PauseGame()
    {
        if (GetComponent<GameManager>().gameIsPaused == false)
        {
            Time.timeScale = 0;
            GetComponent<GameManager>().gameIsPaused = true;
            uiManager.PauseContainer.SetActive(true);
        }
        else if (GetComponent<GameManager>().gameIsPaused == true)
        {
            Time.timeScale = 1;
            GetComponent<GameManager>().gameIsPaused = false;
            uiManager.PauseContainer.SetActive(false);
        }
    }




}
