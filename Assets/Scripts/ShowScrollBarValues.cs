
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class ShowScrollBarValues : MonoBehaviour
{

    public TextMeshProUGUI x3T;

    public Scrollbar x3;
    
    public TextMeshProUGUI hozVerT;

    public Scrollbar hozVer;
    
    public TextMeshProUGUI sonarT;

    public Scrollbar sonar;

    public GameObject panel;

    public Toggle check;

    public GameObject checkObject;
    private void Start()
    {
        if (PlayerPrefs.GetInt("Mode") == 1)
        {
            if (check != null)
            {
                checkObject.SetActive(true);
                check.onValueChanged.AddListener(delegate { ToggleValueChanged(check); });
            }
            
        }
    }
    
    void ToggleValueChanged(Toggle change)
    {
        if (change.isOn)
        {
            panel.SetActive(true);
            ShowScrollbarValue();
        }
        else
        {
            panel.SetActive(false);
        }
    }


    public void ShowScrollbarValue()
    {
        x3T.SetText((Mathf.RoundToInt(x3.value * 10) == 0 ? "OFF" : Mathf.RoundToInt(x3.value * 10).ToString()));
        hozVerT.SetText(Mathf.RoundToInt(hozVer.value * 10) == 0 ? "OFF" : Mathf.RoundToInt(hozVer.value * 10).ToString());
        sonarT.SetText(Mathf.RoundToInt(sonar.value * 8) == 0 ? "OFF" : Mathf.RoundToInt(sonar.value * 8 + 2).ToString());

    }


    public void SendValues()
    {
        if(check.isOn)
        {
            FindObjectOfType<GameManager>().SetPowersRep(Mathf.RoundToInt(x3.value * 10),
                Mathf.RoundToInt(hozVer.value * 10),
                Mathf.RoundToInt(sonar.value * 8 + 2));
        }
        else
        {
            FindObjectOfType<GameManager>().SetPowersRep(Random.Range(6, 12),
                Random.Range(6, 12), Random.Range(6, 12));
        }
    }
}
