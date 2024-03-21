using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChanger : MonoBehaviour
{
    private static CursorChanger instance;

    public Texture2D cursorArrow;

    public Texture2D cursorAtack;

    void Awake()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(gameObject); 
            instance = this;
        }
        else if(instance != this) 
        {
            Destroy(gameObject);
        }
       
    }


    void Start()
    {
        Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
    }


    public void ChangeCursor(bool attack)
    {
        Cursor.SetCursor(attack ? cursorAtack : cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
    }

}
