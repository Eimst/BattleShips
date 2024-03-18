using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    public Sprite[] sprites; // Array to hold the sprites
    public int currentSpriteIndex = 0; // Index of the currently displayed sprite
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    public bool isRotated = false;

    void Start()
    {
        // Render first sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (sprites.Length > 0)
        {
            ChangeSprite(currentSpriteIndex);
        }
    }

    void Update()
    {
        // if left mouse button pressed changes sprite
        if (Input.GetMouseButtonDown(1) && currentSpriteIndex > 0)
        {
            if (isRotated)
            {
                currentSpriteIndex--;
                ChangeSprite(currentSpriteIndex);
                isRotated = false;
            }
            else
            {
                currentSpriteIndex++;
                ChangeSprite(currentSpriteIndex);
                isRotated = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && currentSpriteIndex > 0)
        {
            currentSpriteIndex = 0;
            ChangeSprite(currentSpriteIndex);
            isRotated = false;
        }
    }

    // Changes sprite
    public void ChangeSprite(int index)
    {
        spriteRenderer.sprite = sprites[index];
    }
}
