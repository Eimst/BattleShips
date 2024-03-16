using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mouse : MonoBehaviour
{
    // Main  camera
    public Camera Camera;

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
    }

    private bool CheckToPutShip(int x, int y, int spriteIndex, bool isRotated)
    {
        int size = (spriteIndex + 1) / 2 - 1;
        Field field = FindObjectOfType<Field>();
        bool done = false;
        if (isRotated)
        {
            field.EnterShip((int)x, (int)y, (int)x, (int)y + size, ref done);
        }
        else
        {
            field.EnterShip((int)x, (int)y, (int)x + size, (int)y, ref done);
        }
        return done;
    }

    private bool Validate(ref int x, ref int y, int spriteIndex)
    {
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
        Debug.Log(size);
    }

    //Changes mouse coordinates to world coordinates
    private Vector3 ScreenCordsToWorldCords()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 15));
        return worldPosition;
    }
}
