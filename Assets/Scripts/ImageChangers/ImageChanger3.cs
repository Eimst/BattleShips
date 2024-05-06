using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageChanger3 : MonoBehaviour
{
    public Sprite[] images;
    private int currentImageIndex = 0;
    private Image imageComponent;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        if (images.Length > 0)
        {
            ChangeSprite(currentImageIndex);
        }
    }

    void Update()
    {
        Field field = FindObjectOfType<Field>();
        SpriteChanger script = FindObjectOfType<SpriteChanger>();
        // finds if there is one ship taken
        int correc = script.currentSpriteIndex > 4 && script.currentSpriteIndex < 7 ? 1 : 0;
        if (field != null)
        {
            // if there is one or more ships left shows button if not then hides it
            if (currentImageIndex < 1)
            {
                if (field.shipsCount[2] - correc < 1)
                {
                    currentImageIndex = 1;
                    ChangeSprite(1);
                }
            }
            else
            {
                if (field.shipsCount[2] - correc > 0)
                {
                    currentImageIndex = 0;
                    ChangeSprite(0);
                }
            }
        }
    }

    // changes mouse picked ship sprite
    public void ChangeSprite(int index)
    {
        imageComponent.sprite = images[index];
    }

    // when button is clicked and where is ships left of this size changes sprite of mouse taken ship
    public void OnClick()
    {
        Field field = FindObjectOfType<Field>();
        if (field.shipsCount[2] > 0 && field.blinkCount == 0 )
        {
            SpriteChanger script = FindObjectOfType<SpriteChanger>();
            script.currentSpriteIndex = 5;
            script.ChangeSprite(5);
            script.isRotated = false;
            script.isNew = true;
        }
    }
}
