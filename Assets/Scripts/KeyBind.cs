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
    public bool DisableLeftClickKey(ref TextMeshProUGUI label)
    {
        if (label.text.Equals("Mouse0"))
        {
            label.text = "No key";
            return true;
        }
        return false;
    }
    public void ChangeKey1()
    {
        buttonLabel1.text = "Press key";
    }
    public void ChangeKey2()
    {
        buttonLabel2.text = "Press key";
    }
    public void ChangeKey3()
    {
        buttonLabel3.text = "Press key";
    }
    public void ResetBinds()
    {
        buttonLabel1.text = "No  key";
        buttonLabel2.text = "No  key";
        buttonLabel3.text = "No  key";
        PlayerPrefs.SetString("AbilityKey1", "");
        PlayerPrefs.SetString("AbilityKey2", "");
        PlayerPrefs.SetString("AbilityKey3", "");
        PlayerPrefs.Save();
    }
}
