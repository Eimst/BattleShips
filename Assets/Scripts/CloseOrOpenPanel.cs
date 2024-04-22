using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseOrOpenPanel : MonoBehaviour
{

    public GameObject panel;

    public Button button;

    public Sprite imageExit;

    public Sprite imageOpen;

    public bool isOpen { get; set; }


    private void Start()
    {
        isOpen = false;
    }


    public void ManagePanel()
    {
        if(isOpen)
        {
            panel.SetActive(false);
            isOpen= false;
            button.image.sprite = imageOpen;
        }
        else
        {
            panel.SetActive(true);
            isOpen= true;
            button.image.sprite = imageExit;
        }
        
    }


    void Update()
    {
        if (isOpen && Input.GetMouseButtonDown(0))
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(panel.GetComponent<RectTransform>(), Input.mousePosition, null))
            {
                ManagePanel();
            }
        }
    }

}
