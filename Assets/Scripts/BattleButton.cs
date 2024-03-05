using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleButton : MonoBehaviour
{
    
    public void GoToBattle()
    {
        /*bool isAllUsed = true;
        for (int i = 0; i < 4; i++)
        if (field.GetComponent<Field>().shipsCount[i] > 0)
            isAllUsed = false;
        if (isAllUsed)*/
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}