using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[SerializeField] public GameObject MenuScreen;
	[SerializeField] public GameObject CreditsScreen;

	void Start()
	{
		DefaultState();
	}

	public void DefaultState()
	{
		MenuScreen.SetActive(true);
		CreditsScreen.SetActive(false);
	}

	public void StartButton()
	{
		SceneManager.LoadScene(1);
	}

	
	public void DebugButton()
	{
		SceneManager.LoadScene(2);
	}

	public void CreditsButton()
	{
		MenuScreen.SetActive(false);
		CreditsScreen.SetActive(true);
	}


	public void QuitButton()
	{
		Application.Quit();
	}
}