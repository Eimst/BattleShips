using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageChanger4 : MonoBehaviour
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
        int correc = script.currentSpriteIndex > 6 ? 1 : 0;
        if (field != null)
        {
            if (currentImageIndex < 1)
            {
                if (field.shipsCount[3] - correc < 1)
                {
                    currentImageIndex = 1;
                    ChangeSprite(1);
                }
            }
            else
            {
                if (field.shipsCount[3] - correc > 0)
                {
                    currentImageIndex = 0;
                    ChangeSprite(0);
                }
            }
        }
    }

    public void ChangeSprite(int index)
    {
        imageComponent.sprite = images[index];
    }

    public void OnClick()
    {
        Field field = FindObjectOfType<Field>();
        if (field.shipsCount[3] > 0)
        {
            SpriteChanger script = FindObjectOfType<SpriteChanger>();
            script.currentSpriteIndex = 7;
            script.ChangeSprite(7);
            script.isRotated = false;
        }
    }
}
