using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgScroll : MonoBehaviour
{
    public float scrollSpeed = 10f;
    public float tileSize = 30f; // Adjust this according to the size of your background tile

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the movement amount based on scroll speed
        float movement = scrollSpeed * Time.deltaTime;

        // Move the background
        transform.Translate(Vector3.left * movement);

        // Check if we've scrolled completely past the left boundary
        if (transform.position.x < startPosition.x - tileSize)
        {
            // Move the background back to its starting position to create an infinite scrolling effect
            transform.position = new Vector3(startPosition.x + tileSize, startPosition.y, startPosition.z);
        }

        // Check if we've scrolled completely past the right boundary
        if (transform.position.x > startPosition.x + tileSize)
        {
            // Move the background back to its starting position to create an infinite scrolling effect
            transform.position = new Vector3(startPosition.x - tileSize, startPosition.y, startPosition.z);
        }
    }
}