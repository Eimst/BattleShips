using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript1 : MonoBehaviour
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
        int correc = script.currentSpriteIndex > 2 && script.currentSpriteIndex < 5 ? 1 : 0;
        if (field != null)
        {
            if (currentImageIndex < 1)
            {
                if (field.shipsCount[1] - correc == 0)
                {
                    currentImageIndex = 1;
                    ChangeSprite(1);
                }
            }
            else
            {
                if (field.shipsCount[1] - correc > 0)
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
        SpriteChanger script = FindObjectOfType<SpriteChanger>();
        script.currentSpriteIndex = 3;
        script.ChangeSprite(3);
        script.isRotated = false;
    }
}
