using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Mouse : MonoBehaviour
{
    // Main  camera
    public Camera Camera;
    // Variabales to see if position was changed
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
                    // removes ship from mouse
                    savedX = -1;
                    savedY = -1;
                    savedPos = false;
                    script.currentSpriteIndex = 0;
                    script.ChangeSprite(0);
                }
                else
                {

                    savedX = -1;
                    savedY = -1;
                    savedPos = false;
                }
            }
            else
            {
                // removes ship from mouse if clicked of field
                if (script.isNew == false)
                {
                    script.currentSpriteIndex = 0;
                    script.ChangeSprite(0);
                }
                else
                {
                    script.isNew = false;
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            // Tries to pick up ship
            if (Validate(ref x, ref y, 1))
            {
                PickUpShip(x, y);
            }
        }

        // find if hovered ship over field
        if (Validate(ref x, ref y, script.currentSpriteIndex))
        {
            // checks if position is changed
            if (x != savedX || y != savedY || (script.currentSpriteIndex % 2 == 0) != savedPos)
            {
                // indicates hovered field tiles
                savedX = x;
                savedY = y;
                savedPos = script.currentSpriteIndex % 2 == 0;
                Indicate(x, y, script.currentSpriteIndex);
            }
        }
        else
        {
            // stops indication if ship outside field
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

    /// <summary>
    /// Method that places ship on field
    /// </summary>
    /// <param name="x"> x coordinate </param>
    /// <param name="y"> x coordinate </param>
    /// <param name="spriteIndex"> current sprite index </param>
    /// <param name="isRotated"> current ship direction </param>
    /// <returns> if ship was placed </returns>
    private bool CheckToPutShip(int x, int y, int spriteIndex, bool isRotated)
    {
        int size = (spriteIndex + 1) / 2 - 1; // calculates size of ship
        Field field = FindObjectOfType<Field>();
        bool done = false;
        // tries to put ship on filed
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
        return done; // returns if action was succesful
    }

    /// <summary>
    /// Method find if ship is hovering on field. And returns coordinates.
    /// </summary>
    /// <param name="x"> x coordinate </param>
    /// <param name="y"> y coordinate </param>
    /// <param name="spriteIndex"> current sprite index </param>
    /// <returns> if ship hovered over filed </returns>
    private bool Validate(ref int x, ref int y, int spriteIndex)
    {
        if (spriteIndex == 0) { return false; } // if mouse without ship returns
        // gets mouse coordinates
        Vector3 worldPosition = ScreenCordsToWorldCords();
        float xTem = worldPosition.x;
        float yTem = worldPosition.y;
        float xCorr = 0;
        float yCorr = 0;

        // finds corections for coordinates if ship is bigger
        if (spriteIndex % 2 > 0)
        {
            xCorr = 0.5f * spriteIndex / 2;
        }
        else
        {
            yCorr = 0.5f * (spriteIndex / 2 - 1);
        }

        //checks if ships is hovered over field
        if (-4.5f < xTem - xCorr && xTem + xCorr < 5.5 && -5.5f < yTem - yCorr && yTem + yCorr < 4.5)
        {
            x = (int) Mathf.Round(xTem + 5 - xCorr) - 1;
            y = (int) Mathf.Abs(Mathf.Round(yTem + 6 + yCorr) - 10);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Method picks up ship from field
    /// </summary>
    /// <param name="x"> x coordinate </param>
    /// <param name="y"> y coordinate </param>
    private void PickUpShip(int x, int y)
    {
        Field field = FindObjectOfType<Field>();
        SpriteChanger script = FindObjectOfType<SpriteChanger>();
        int size = 0;
        bool isVert = false;

        // checks if clicked tiles have ship. If tiles have ship removes clicked ship from field
        // and returns size - the size of that ship, isVert - direction of that ship ( true is vertical )
        if (field.DespawnShip(x, y, ref size, ref isVert)) 
        {
            // finds corresponding sprite to picked up ship
            size--;
            script.currentSpriteIndex = 1 + 2*size;
            if (isVert) { script.currentSpriteIndex++; script.isRotated = true; }
            else { script.isRotated = false; }
            // changes sprite of mouse to picked up ship
            script.ChangeSprite(script.currentSpriteIndex);
        }
        //Debug.Log(size);
    }

    /// <summary>
    /// Method that indicates tiles where will ship be placed also indicates if ship is hovered on illegal position 
    /// </summary>
    /// <param name="x"> x coordinate </param>
    /// <param name="y"> y coordinate </param>
    /// <param name="spriteIndex"> current sprite index </param>
    private void Indicate(int x, int y, int spriteIndex)
    {
        Field field = FindObjectOfType<Field>();
        field.StopIndication(); // stop previous indication
        if (field.CheckForShips(x, y, (spriteIndex + 1) / 2, spriteIndex % 2 == 0)) // checks if position is legal
        {
            field.Indicate(x, y, (spriteIndex + 1) / 2, spriteIndex % 2 == 0, false); // indicates position
        }
        else
        {
            field.Indicate(x, y, (spriteIndex + 1) / 2, spriteIndex % 2 == 0, true); // indicates illegal position
        }
    }

    //Changes mouse coordinates to world coordinates
    private Vector3 ScreenCordsToWorldCords()
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 15));
        return worldPosition;
    }
}
