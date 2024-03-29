using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Chunks : MonoBehaviour
{
    public Sprite[] sprites; //list of sprites (letters / numbers)
    public int index = 0; //index to choose current sprite

    private void UpdateLetter() // updates sprite by index
    {
        if (sprites.Length > index)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[index]; // gets sprites by index
            // Debug.Log($"Current index: {index}, Sprite name: {sprites[index].name}");
        }
    }

    public void ChangeClour()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.702f, 0.933f, 0.7f, 0.8235f);
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
