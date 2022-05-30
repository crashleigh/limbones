using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
	public TMP_Text timerText;   // formatted value to display to the player
	public bool timerRunning;   // whether the timer is running (hook for pause function)

	float _time;    // stores the representation of in-game time

    // ! START
    void Start()
    {
        TimerReset();
        timerRunning = true;
    }

    // ! UPDATE
    void Update()
    {
        if (timerRunning)
        {
            _time += Time.deltaTime;
        }

        double x = _time;
        int t = (int)x;

        //int minutes = t / 60;

        int minutes = t / 60;
        int seconds = t % 60;

        //double t = System.Math.Round(_time, 2);

        //timerText.text = t.ToString();

        timerText.text = string.Format("TIME: {0:00}:{1:00}", minutes, seconds);
    }

    // * PAUSE TIMER
    void TimerPause()
    {
        timerRunning = false;
    }

    // * RESET TIMER TO ZERO
    void TimerReset()
    {
        _time = 0;
    }
}

