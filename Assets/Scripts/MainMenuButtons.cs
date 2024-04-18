using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{

    public bool modeSpecial { get; set; } 

    public void PlayGame()
    {
        PlayerPrefs.SetInt("Mode", modeSpecial ? 1 : 0);

        StartCoroutine(EnableStartButtonAfterDelay(1.5f));
    }


    IEnumerator EnableStartButtonAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Enable the start button
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
