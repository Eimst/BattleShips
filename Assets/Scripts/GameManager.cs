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
        if(SceneManager.GetActiveScene().name.Equals("ShipSelectionScene") && playerFieldInstance.ship4Tiles) // jei paspaustas laivas galima slankioti ir padejus nusispalvina langelis
            MouseTrajectory(-5, 5, true);
        else if(SceneManager.GetActiveScene().name.Equals("BattleScene"))
            MouseTrajectory(2, 12, false);
    }


    void MouseTrajectory(int boadStartX, int boardEndX, bool isSelectionScene) //  Y pozicija visada tokia pati
    {
        Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);
        bool active = false;
        if (hit.collider != null)
        {

            if (hit.collider.transform.position.x >= boadStartX && hit.collider.transform.position.x <= boardEndX &&
                hit.collider.transform.position.y <= 5 && hit.collider.transform.position.y >= -5)
            {
                active = true;
            }
        }

        if (active && !isSelectionScene && Input.GetMouseButtonDown(0)) // Check for left click
        {
            TileClicked(hit.collider.gameObject, rayPos);
            Kill(hit.collider.gameObject.name, botFieldInstance);
        }
        else if(active && isSelectionScene)
        {
            TileClicked(hit.collider.gameObject, rayPos); // 
            if (Input.GetMouseButtonDown(0))
            {
                Kill(hit.collider.gameObject.name, playerFieldInstance);
                playerFieldInstance.ship4Tiles = false;
                // Laivo istatymo algoritmas vietoj kill
            }
                
        }
        
    }


    void TileClicked(GameObject tile, Vector2 clickPosition)
    {
        // This function logs the clicked tile's name and position.
        Debug.Log($"Tile clicked: {tile.name} at position {clickPosition}");
    }


    public void setShip4Tiles()
    {
        playerFieldInstance.ship4Tiles = true;
    }

    public void GeneratePosition()
    {
        playerFieldInstance.ship4Tiles = false;
        playerFieldInstance.RandomPositionGenerator();
    }


    void Kill(string coordinates, IKillable instance)
    {
        int x = int.Parse(coordinates.Split(' ')[0]);
        int y = int.Parse(coordinates.Split(' ')[1]);
        instance.Kill(x, y);
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
    public void GoToBattle()
    {

        if (playerFieldInstance.AreAllSpawned())
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

