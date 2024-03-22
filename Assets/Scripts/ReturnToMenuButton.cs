using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuButton : MonoBehaviour
{
    public void ReturnToMenu()
    {
        if (GameManager.instance != null)
        {
            // Destroy the instance of GameManager
            Destroy(GameManager.instance.gameObject); 
            GameManager.instance = null; 
        }
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene("ShipSelectionScene");
    }
}
