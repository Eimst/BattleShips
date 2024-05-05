using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class KeyBind : MonoBehaviour
{
    public TextMeshProUGUI buttonLabel1;
    public TextMeshProUGUI buttonLabel2;
    public TextMeshProUGUI buttonLabel3;
    /// <summary>
    /// gets keybinds for abilities from player prefs or puts "no key" if player prefs are empty
    /// </summary>
    void Start()
    {
        buttonLabel1.text = PlayerPrefs.GetString("AbilityKey1");
        buttonLabel2.text = PlayerPrefs.GetString("AbilityKey2");
        buttonLabel3.text = PlayerPrefs.GetString("AbilityKey3");
        if (PlayerPrefs.GetString("AbilityKey1").Equals(""))
            buttonLabel1.text = "No key";
        if (PlayerPrefs.GetString("AbilityKey2").Equals(""))
            buttonLabel2.text = "No key";
        if (PlayerPrefs.GetString("AbilityKey3").Equals(""))
            buttonLabel3.text = "No key";
    }
    /// <summary>
    /// sets user preferred keybind to ability buttons and saves them to player prefs
    /// </summary>
    void Update()
    {
        if (buttonLabel1.text.Equals("Press key"))
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(key))
                {
                    buttonLabel1.text = key.ToString();
                    if (buttonLabel2.text.Equals(key.ToString()))
                    {
                        buttonLabel2.text = "No key";
                        PlayerPrefs.SetString("AbilityKey2", "");
                        PlayerPrefs.Save();
                    }
                    if (buttonLabel3.text.Equals(key.ToString()))
                    {
                        buttonLabel3.text = "No key";
                        PlayerPrefs.SetString("AbilityKey3", "");
                        PlayerPrefs.Save();
                    }
                    PlayerPrefs.SetString("AbilityKey1", key.ToString());
                    PlayerPrefs.Save();
                }
            }
        }
        else if (buttonLabel2.text.Equals("Press key"))
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(key))
                {
                    buttonLabel2.text = key.ToString();
                    if (buttonLabel1.text.Equals(key.ToString()))
                    {
                        buttonLabel1.text = "No key";
                        PlayerPrefs.SetString("AbilityKey1", "");
                        PlayerPrefs.Save();
                    }
                    if (buttonLabel3.text.Equals(key.ToString()))
                    {
                        buttonLabel3.text = "No key";
                        PlayerPrefs.SetString("AbilityKey3", "");
                        PlayerPrefs.Save();
                    }
                    PlayerPrefs.SetString("AbilityKey2", key.ToString());
                    PlayerPrefs.Save();
                }
            }
        }
        else if (buttonLabel3.text.Equals("Press key"))
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(key))
                {
                    buttonLabel3.text = key.ToString();
                    if (buttonLabel2.text.Equals(key.ToString()))
                    {
                        buttonLabel2.text = "No key";
                        PlayerPrefs.SetString("AbilityKey2", "");
                        PlayerPrefs.Save();
                    }
                    if (buttonLabel1.text.Equals(key.ToString()))
                    {
                        buttonLabel1.text = "No key";
                        PlayerPrefs.SetString("AbilityKey1", "");
                        PlayerPrefs.Save();
                    }
                    PlayerPrefs.SetString("AbilityKey3", key.ToString());
                    PlayerPrefs.Save();
                }
            }
        }
        if (DisableLeftClickKey(ref buttonLabel1))
        {
            PlayerPrefs.SetString("AbilityKey1", "");
            PlayerPrefs.Save();
        }
        if (DisableLeftClickKey(ref buttonLabel2))
        {
            PlayerPrefs.SetString("AbilityKey2", "");
            PlayerPrefs.Save();
        }
        if (DisableLeftClickKey(ref buttonLabel3))
        {
            PlayerPrefs.SetString("AbilityKey3", "");
            PlayerPrefs.Save();
        }
    }
    /// <summary>
    /// disables mouse left key if possible and returns true 
    /// </summary>
    /// <param name="label">label of keybind button</param>
    /// <returns>true if mouse left key was disabled</returns>
    public bool DisableLeftClickKey(ref TextMeshProUGUI label)
    {
        if (label.text.Equals("Mouse0"))
        {
            label.text = "No key";
            return true;
        }
        return false;
    }
    /// <summary>
    /// prepares ability1 button to change key
    /// </summary>
    public void ChangeKey1()
    {
        buttonLabel1.text = "Press key";
    }
    /// <summary>
    /// prepares ability1 button to change key
    /// </summary>
    public void ChangeKey2()
    {
        buttonLabel2.text = "Press key";
    }
    /// <summary>
    /// prepares ability1 button to change key
    /// </summary>
    public void ChangeKey3()
    {
        buttonLabel3.text = "Press key";
    }
    /// <summary>
    /// resets all abilities' keybinds to "no key"
    /// </summary>
    public void ResetBinds()
    {
        buttonLabel1.text = "No key";
        buttonLabel2.text = "No key";
        buttonLabel3.text = "No key";
        PlayerPrefs.SetString("AbilityKey1", "");
        PlayerPrefs.SetString("AbilityKey2", "");
        PlayerPrefs.SetString("AbilityKey3", "");
        PlayerPrefs.Save();
    }
}
