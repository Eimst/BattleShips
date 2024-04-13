using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Mouse : MonoBehaviour
{
    // Main  camera
    public Camera Camera;
    private int savedX = -1;
    private int savedY = -1;
    private bool savedPos = false;

    private void Update()
    {
        SpriteChanger script = FindObjectOfType<SpriteChanger>();
        //moves object to mouse
        transform.position = ScreenCordsToWorldCords();
        int x = 0, y = 0;
        //Checks if ship can be put if yes then puts it
        if (Input.GetMouseButtonDown(0) && script.currentSpriteIndex > 0)
        {
            if (Validate(ref x, ref y, script.currentSpriteIndex))
            {
                if (CheckToPutShip(x, y, script.currentSpriteIndex, script.isRotated))
                {
                    savedX = -1;
                    savedY = -1;
                    savedPos = false;
                    script.currentSpriteIndex = 0;
                    script.ChangeSprite(0);
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (Validate(ref x, ref y, 1))
            {
                PickUpShip(x, y);
            }
        }

        if (Validate(ref x, ref y, script.currentSpriteIndex))
        {
            if (x != savedX || y != savedY || (script.currentSpriteIndex % 2 == 0) != savedPos)
            {
                savedX = x;
                savedY = y;
                savedPos = script.currentSpriteIndex % 2 == 0;
                Indicate(x, y, script.currentSpriteIndex);
            }
        }
        else
        {
            // BUGAS!!! LOL kiek cheburek. you suck!!!
            if (-1 != savedX || -1 != savedY)
            {
                savedX = -1;
                savedY = -1;
                savedPos = false;
                Field field = FindObjectOfType<Field>();
                field.StopIndication();
            }
        }
    }

    private bool CheckToPutShip(int x, int y, int spriteIndex, bool isRotated)
    {
        int size = (spriteIndex + 1) / 2 - 1;
        Field field = FindObjectOfType<Field>();
        bool done = false;
        if (isRotated)
        {
            field.StopIndication();
            field.EnterShip((int)x, (int)y, (int)x, (int)y + size, ref done);
        }
        else
        {
            field.StopIndication();
            field.EnterShip((int)x, (int)y, (int)x + size, (int)y, ref done);
        }
        return done;
    }

    private bool Validate(ref int x, ref int y, int spriteIndex)
    {
        if (spriteIndex == 0) { return false; }
        Vector3 worldPosition = ScreenCordsToWorldCords();
        float xTem = worldPosition.x;
        float yTem = worldPosition.y;
        float xCorr = 0;
        float yCorr = 0;
        if (spriteIndex % 2 > 0)
        {
            xCorr = 0.5f * spriteIndex / 2;
        }
        else
        {
            yCorr = 0.5f * (spriteIndex / 2 - 1);
        }
        if (-4.5f < xTem - xCorr && xTem + xCorr < 5.5 && -5.5f < yTem - yCorr && yTem + yCorr < 4.5)
        {
            x = (int) Mathf.Round(xTem + 5 - xCorr) - 1;
            y = (int) Mathf.Abs(Mathf.Round(yTem + 6 + yCorr) - 10);
            return true;
        }
        return false;
    }

    private void PickUpShip(int x, int y)
    {
        Field field = FindObjectOfType<Field>();
        SpriteChanger script = FindObjectOfType<SpriteChanger>();
        int size = 0;
        bool isVert = false;
        if (field.DespawnShip(x, y, ref size, ref isVert))
        {
            size--;
            script.currentSpriteIndex = 1 + 2*size;
            if (isVert) { script.currentSpriteIndex++; script.isRotated = true; }
            else { script.isRotated = false; }
            script.ChangeSprite(script.currentSpriteIndex);
        }
        //Debug.Log(size);
    }

    private void Indicate(int x, int y, int spriteIndex)
    {
        Field field = FindObjectOfType<Field>();
        field.StopIndication();
        if (field.CheckForShips(x, y, (spriteIndex + 1) / 2, spriteIndex % 2 == 0))
        {
            field.Indicate(x, y, (spriteIndex + 1) / 2, spriteIndex % 2 == 0);
        }
    }

    public void onClick()
    {
        SpriteChanger script = FindObjectOfType<SpriteChanger>();
        script.currentSpriteIndex = 0;
        script.ChangeSprite(0);
        script.isRotated = false;
    }

    //Changes mouse coordinates to world coordinates
    private Vector3 ScreenCordsToWorldCords()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 15));
        return worldPosition;
    }
}
