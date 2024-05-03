using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public Button myButton; // Assign this in the Inspector
    public int abilityNumber; // Assign the name of the ability this button corresponds to

    private GameManager _gameManager;
    
    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        myButton.onClick.AddListener(CallShootingManagerSetAbility);
    }
    private void Update()
    {
        if(!_gameManager.isKeyBindPressed)
            CheckForKeybinds();
    }


    private void CheckForKeybinds()
    {
        if (!PlayerPrefs.GetString("AbilityKey1").Equals("") && abilityNumber == 1) // 3x3
        {
            if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("AbilityKey1"))))
            {
                myButton.onClick.Invoke();
                _gameManager.PrepareBoardForKeyBindActivation();
            }
        }
        else if (!PlayerPrefs.GetString("AbilityKey2").Equals("") && abilityNumber == 2) // ver/hor
        {
            if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("AbilityKey2"))))
            {
                myButton.onClick.Invoke();
                _gameManager.PrepareBoardForKeyBindActivation();
            }
        }
        else if (!PlayerPrefs.GetString("AbilityKey3").Equals("") && abilityNumber == 4) // sonar
        {
            if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("AbilityKey3"))))
            {
                myButton.onClick.Invoke();
                _gameManager.PrepareBoardForKeyBindActivation();
            }
        }
    }
    
    
    private void CallShootingManagerSetAbility()
    {
        if(!_gameManager.isKeyBindPressed)
        {
            _gameManager.isKeyBindPressed = true;
            GameObject.FindObjectOfType<ShootingManager>().SetAbility(abilityNumber);
        }
    }
}
