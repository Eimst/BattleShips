using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    public float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move the sprite horizontally
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        // If the sprite goes off-screen, reset its position to the opposite side
        if (transform.position.x <= -10f) // Adjust -10f according to your scene size
        {
            Vector3 newPos = transform.position;
            newPos.x += 20f; // Adjust 20f according to your scene size
            transform.position = newPos;
        }
    }
}
