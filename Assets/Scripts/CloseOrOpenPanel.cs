using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseOrOpenPanel : MonoBehaviour
{

    public GameObject panel;

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
        }
        else
        {
            panel.SetActive(true);
            isOpen= true;
        }
        
    }


    void Update()
    {
        if (isOpen && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            Debug.Log(mousePosition.x + " " + mousePosition.y);
            if (mousePosition.x >= 0 && mousePosition.x <= 1420 && mousePosition.y >= 0 && mousePosition.y <= 1100 || mousePosition.y < 620)
            {
                ManagePanel();    
            }
        }
    }

}
