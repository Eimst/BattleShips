
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShowScrollBarValues : MonoBehaviour
{

    public TextMeshProUGUI x3T;

    public Scrollbar x3;
    
    public TextMeshProUGUI hozVerT;

    public Scrollbar hozVer;
    
    public TextMeshProUGUI sonarT;

    public Scrollbar sonar;

    public GameObject panel;

    private void Start()
    {
        if (PlayerPrefs.GetInt("Mode") == 1 )
        {
            panel.SetActive(true);
            ShowScrollbarValue();
        }
    }

    public void ShowScrollbarValue()
    {
        x3T.SetText((Mathf.RoundToInt(x3.value * 10) == 0 ? "OFF" : Mathf.RoundToInt(x3.value * 10).ToString()));
        hozVerT.SetText(Mathf.RoundToInt(hozVer.value * 10) == 0 ? "OFF" : Mathf.RoundToInt(hozVer.value * 10).ToString());
        sonarT.SetText(Mathf.RoundToInt(sonar.value * 10) == 0 ? "OFF" : Mathf.RoundToInt(sonar.value * 10).ToString());

    }


    public void SendValues()
    {
        FindObjectOfType<GameManager>().SetPowersRep(Mathf.RoundToInt(x3.value * 10), Mathf.RoundToInt(hozVer.value * 10), 
            Mathf.RoundToInt(sonar.value * 10));
    }
}
