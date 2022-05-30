using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject pauseContainer;
    [SerializeField] GameObject fader;
    [SerializeField] GameObject loseContainer;
    [SerializeField] GameObject winContainer;

    public GameObject PauseContainer => pauseContainer;
    public GameObject Fader => fader;
    public GameObject LoseContainer => loseContainer;
    public GameObject WinContainer => winContainer;

    private void Awake()
    {
        winContainer.SetActive(false);
        loseContainer.SetActive(false);
        pauseContainer.SetActive(false);
        fader.SetActive(false);
    }
}
