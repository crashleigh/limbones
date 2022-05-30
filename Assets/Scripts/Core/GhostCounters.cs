using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GhostCounters : MonoBehaviour
{
    private GameManager gameManager;
    public TMP_Text counterText;   // formatted value to display to the player

    private void OnEnable()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        counterText.text = string.Format("x {0}", gameManager.soulCounter.ToString());
    }
}
