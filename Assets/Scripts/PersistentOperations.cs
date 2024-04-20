using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentOperations : MonoBehaviour
{
    public static PersistentOperations instance { get; set; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ReturnToMenu()
    {

        StartCoroutine(LoadSceneWithDelay("MainMenu", true));
    }

    public void Restart()
    {
        StartCoroutine(LoadSceneWithDelay("ShipSelectionScene"));
    }

    private IEnumerator LoadSceneWithDelay(string sceneName, bool destroy = false)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        if (destroy && GameManager.instance != null)
        {
            Destroy(GameManager.instance.gameObject);
            GameManager.instance = null;
        }
        FindObjectOfType<CursorChanger>().SendMessage("ChangeToAttack", false);
    }
}
