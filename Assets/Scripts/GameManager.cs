using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Player playerFieldPrefab;
    public Bot botFieldPrefab;

    private Player playerFieldInstance;
    private Bot botFieldInstance;

    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Attempt to find an existing player instance.
        playerFieldInstance = FindObjectOfType<Player>();
        botFieldInstance = FindObjectOfType<Bot>();

        // If no player exists, instantiate one.

        playerFieldInstance = Instantiate(playerFieldPrefab, new Vector3(-5, 5, 0), Quaternion.identity);
        playerFieldInstance.transform.SetParent(transform);

        botFieldInstance = Instantiate(botFieldPrefab, new Vector3(2, 5, 0), Quaternion.identity);
        botFieldInstance.transform.SetParent(transform);
        botFieldInstance.gameObject.SetActive(false);
    }



    void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("BattleScene"))
            MouseTrajectory();
    }

    void MouseTrajectory()
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

    public void GeneratePosition()
    {
        Debug.Log("Trigg");
        playerFieldInstance.RandomPositionGenerator();
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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "BattleScene":
                if (botFieldInstance != null)
                {
                    botFieldInstance.gameObject.SetActive(true); // Activate bot
                }
                if (playerFieldInstance != null)
                {
                    // Set player's position to a different vector in the third scene
                    playerFieldInstance.transform.position = new Vector3(-12, 5, 0);
                }
                break;

            // Switch for future scenes.
        }
    }
}

