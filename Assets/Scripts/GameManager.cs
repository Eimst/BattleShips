using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using Random = UnityEngine.Random;
using TMPro;
using System;
using static Field;

public class GameManager : MonoBehaviour 
{
    public Player playerFieldPrefab;
    public Bot botFieldPrefab;

    private Player playerFieldInstance;
    private Bot botFieldInstance;

    public static GameManager instance;

    public TextMeshProUGUI textMeshProUGUI;

    public enum GameState
    {
        Setup,
        PlayerTurn,
        BotTurn,
        GameOver
    }

    public GameState currentState = GameState.Setup;

    bool isBotTurnHandled = false;


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
        textMeshProUGUI.transform.SetParent(transform);
        textMeshProUGUI.gameObject.SetActive(false);

    }


    void Update()
    {
        if(playerFieldInstance.GetRemainingBoats() == 0 || botFieldInstance.GetRemainingBoats() == 0)
            currentState = GameState.GameOver;

        switch (currentState)
        {
            case GameState.Setup:
                if (SceneManager.GetActiveScene().name.Equals("ShipSelectionScene") && playerFieldInstance.ship4Tiles) // jei paspaustas laivas galima slankioti ir padejus nusispalvina langelis
                    MouseTrajectory(true);
                break;

            case GameState.PlayerTurn:
                MouseTrajectory(false);
                break;

            case GameState.BotTurn:
                if (!isBotTurnHandled)
                    StartCoroutine(HandleBotTurn());
                break;

             case GameState.GameOver:
                textMeshProUGUI.text = playerFieldInstance.GetRemainingBoats() == 0 ? "You lost!" : "You Win!";
                textMeshProUGUI.gameObject.SetActive(true);
                break;
        }
    }


    void MouseTrajectory(bool isSelectionScene) //  Y pozicija visada tokia pati
    {
        int boardStartX;
        int boardEndX;

        boardStartX = isSelectionScene ? -5 : 2;
        boardEndX = isSelectionScene ? 5 : 12;

        Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);
        bool active = false;
        if (hit.collider != null)
        {

            if (hit.collider.transform.position.x >= boardStartX && hit.collider.transform.position.x <= boardEndX &&
                hit.collider.transform.position.y <= 5 && hit.collider.transform.position.y >= -5)
            {
                active = true;
            }
        }

        if (active && !isSelectionScene && Input.GetMouseButtonDown(0)) // Check for left click
        {
            TileClicked(hit.collider.gameObject, rayPos);
            StateHandler(Destroy(hit.collider.gameObject.name, botFieldInstance), true); 
        }
        else if(active && isSelectionScene)
        {
            TileClicked(hit.collider.gameObject, rayPos); 
            if (Input.GetMouseButtonDown(0))
            {
                Destroy(hit.collider.gameObject.name, playerFieldInstance);
                playerFieldInstance.ship4Tiles = false;
                // Laivo istatymo algoritmas vietoj destroy
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


    IEnumerator HandleBotTurn()
    {
        isBotTurnHandled = true;
        yield return new WaitForSeconds(0.7f);
        Field.DestroyResult result;
        do
        {
            result = Destroy(botFieldInstance.ApplyShot(), playerFieldInstance);

        } while (result == Field.DestroyResult.IllegalMove);

        StateHandler(result, false);

        isBotTurnHandled = false;

    }


    void StateHandler(Field.DestroyResult result, bool isPlayer)
    {
        switch (result)
        {
            case DestroyResult.Success:
                // Handle success (move hit something)
                break;

            case DestroyResult.Failure:
                // Handle failure (move hit water)
                if(isPlayer)
                    currentState = GameState.BotTurn;
                else
                    currentState = GameState.PlayerTurn;
                break;

            case DestroyResult.IllegalMove:
                // Handle illegal move
                Debug.Log("Illegal move attempted.");
                if (isPlayer)
                    currentState = GameState.PlayerTurn;
                else
                    currentState = GameState.BotTurn;
                break;
        }
    }


    Field.DestroyResult Destroy(string coordinates, IKillable instance)
    {
        int x = int.Parse(coordinates.Split(' ')[0]);
        int y = int.Parse(coordinates.Split(' ')[1]);
        return instance.Destroy(x, y);
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

                currentState = Random.Range(0, 2) == 0 ? GameState.PlayerTurn : GameState.BotTurn;

                Canvas canvas = FindObjectOfType<Canvas>(); // Find the active canvas
                if (canvas != null && textMeshProUGUI != null)
                {
                    textMeshProUGUI.transform.position = new Vector3(-210, 150, 0);
                    textMeshProUGUI.transform.SetParent(canvas.transform, false); // False to keep local orientation
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

