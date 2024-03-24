using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Chunks : MonoBehaviour
{
    public Sprite[] sprites; //list of sprites (letters / numbers)
    public int index = 0; //index to choose current sprite
    private int savedstate = 0;
    private float timer = 0f;
    public void UpdateLetter() // updates sprite by index
    {
        timer += Time.deltaTime;
        if (sprites.Length > index)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[index]; // gets sprites by index
            // Debug.Log($"Current index: {index}, Sprite name: {sprites[index].name}");
        }
        if (savedstate > 0)
        {
            if (index == 0 && timer > 0.5)
            {
                index = savedstate;
                GetComponent<SpriteRenderer>().sprite = sprites[index];
                timer = 0f;
            }
            else if (index != 0 && timer > 1.25)
            {
                index = 0;
                GetComponent<SpriteRenderer>().sprite = sprites[index];
                timer = 0f;
            }
        }
    }

    public void BlinkSprites()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.702f, 0.933f, 0.7f, 0.8235f);
        savedstate = index;
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateLetter();
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLetter();
    }
}
