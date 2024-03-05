using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player playerFieldPrefab;
    public Bot botFieldPrefab;

    private Player playerFieldInstance;
    private Bot botFieldInstance;

    void Start()
    {
        // Instantiate the player's field
        if (playerFieldPrefab != null)
        {
            playerFieldInstance = Instantiate(playerFieldPrefab, new Vector3(-12, 5, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogError("Player Field Prefab is not assigned in the GameManager!");
        }

        // Instantiate the bot's field
        if (botFieldPrefab != null)
        {
            botFieldInstance = Instantiate(botFieldPrefab, new Vector3(2, 5, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogError("Bot Field Prefab is not assigned in the GameManager!");
        }
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Check for left click
        {
            Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log($"Hit: {hit.collider.gameObject.name} at {hit.collider.gameObject.transform.position}");

                if (hit.collider.transform.position.x >= 2 && hit.collider.transform.position.x <= 12 &&
                    hit.collider.transform.position.y <= 5 && hit.collider.transform.position.y >= -5)
                {
                    TileClicked(hit.collider.gameObject, rayPos);
                    Kill(hit.collider.gameObject.name);
                }
                else
                {
                    Debug.Log("Click outside bot's field");
                }
            }
            else
            {
                Debug.Log($"Missed: Ray Position {rayPos}");
            }
        }
    }

    void Kill(string coordinates)
    {
        botFieldInstance.Kill(int.Parse(coordinates.Split(' ')[0]), int.Parse(coordinates.Split(' ')[1]));
    }

    void TileClicked(GameObject tile, Vector2 clickPosition)
    {
        // This function logs the clicked tile's name and position.
        Debug.Log($"Tile clicked: {tile.name} at position {clickPosition}");
    }
}

