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

    private void CallShootingManagerSetAbility()
    {
        GameObject.FindObjectOfType<ShootingManager>().SetAbility(abilityNumber);
    }
}
