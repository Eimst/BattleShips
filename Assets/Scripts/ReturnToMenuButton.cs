using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuButton : MonoBehaviour
{
    private void Start()
    {
        if (PersistentOperations.instance == null)
        {
            GameObject operations = new GameObject("PersistentOperations");
            operations.AddComponent<PersistentOperations>();
        }
    }

    public void ReturnToMenu()
    {
        FindObjectOfType<PersistentOperations>().ReturnToMenu();
    }

    public void Restart()
    {
        FindObjectOfType<PersistentOperations>().Restart();
    }

}
