using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartsIndicator : MonoBehaviour
{
    [SerializeField] GameObject Part01;
    [SerializeField] GameObject Part02;
    [SerializeField] GameObject Part03;
    [SerializeField] GameObject Part04;
    [SerializeField] public int healthPassthrough;

    // public GameObject Part01 => Part01;
    // public GameObject Part02 => Part02;
    // public GameObject Part03 => Part03;
    // public GameObject Part04 => Part04;

    private void Update()
    {
        healthPassthrough = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().GetCurrentHealth;

        if (healthPassthrough == 1)
        {
            Part01.SetActive(false);
            Part02.SetActive(false);
            Part03.SetActive(false);
            Part04.SetActive(true);
        }

        if (healthPassthrough == 2)
        {
            Part01.SetActive(true);
            Part02.SetActive(false);
            Part03.SetActive(false);
            Part04.SetActive(true);
        }

        if (healthPassthrough == 3)
        {
            Part01.SetActive(true);
            Part02.SetActive(true);
            Part03.SetActive(false);
            Part04.SetActive(true);
        }

        if (healthPassthrough == 4)
        {
            Part01.SetActive(true);
            Part02.SetActive(true);
            Part03.SetActive(true);
            Part04.SetActive(true);
        }
    }
}
