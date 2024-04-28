using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    public Button myButton; // Assign this in the Inspector
    public int abilityNumber; // Assign the name of the ability this button corresponds to

    private void Start()
    {
        myButton.onClick.AddListener(CallShootingManagerSetAbility);
    }
    private void Update()
    {
        if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("AbilityKey1"))) && abilityNumber == 1) // 3x3
            myButton.onClick.Invoke();
        else if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("AbilityKey2"))) && abilityNumber == 2) // ver/hor
            myButton.onClick.Invoke();
        else if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("AbilityKey3"))) && abilityNumber == 3) // sonar
            myButton.onClick.Invoke();
    }
    private void CallShootingManagerSetAbility()
    {
        GameObject.FindObjectOfType<ShootingManager>().SetAbility(abilityNumber);
    }
}
