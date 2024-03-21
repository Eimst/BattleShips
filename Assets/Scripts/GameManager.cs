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

    private GameObject lastHoveredTile = null;

    CursorChanger cursor;

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

        cursor = FindObjectOfType<CursorChanger>();
    }


    void Update()
    {

        switch (currentState)
        {
            case GameState.PlayerTurn:
                MouseTrajectory();
                IsGameOver();
                break;

            case GameState.BotTurn:
                if (!isBotTurnHandled)
                    StartCoroutine(HandleBotTurn());
                IsGameOver();
                break;

             case GameState.GameOver:
                textMeshProUGUI.text = playerFieldInstance.GetRemainingBoats() == 0 ? "You Lost!" : "You Won!";
                textMeshProUGUI.gameObject.transform.localPosition = new Vector3(-500, 135, 0);
                textMeshProUGUI.gameObject.SetActive(true);
                break;
        }
    }


    void IsGameOver()
    {
        if (playerFieldInstance.GetRemainingBoats() == 0 || botFieldInstance.GetRemainingBoats() == 0)
            currentState = GameState.GameOver;
    }


    void MouseTrajectory() //  Y pozicija visada tokia pati
    {
        int boardStartX;
        int boardEndX;

        boardStartX = 2;
        boardEndX = 12;

        Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);
        if (hit.collider != null)
        {

            if (hit.collider.transform.position.x >= boardStartX && hit.collider.transform.position.x <= boardEndX &&
                hit.collider.transform.position.y <= 5 && hit.collider.transform.position.y >= -5)
            {
                GameObject currentTile = hit.collider.gameObject;

                if (currentTile != lastHoveredTile)
                {
                    if (lastHoveredTile != null)
                    {
                        botFieldInstance.GetField().ChangeSprite(lastHoveredTile);
                    }

                    if (botFieldInstance.GetField().ChangeSprite(currentTile))
                        cursor.ChangeCursor(true);
                    else
                        cursor.ChangeCursor(false);

                    lastHoveredTile = currentTile;
                }


                if (Input.GetMouseButtonDown(0))
                {
                    cursor.ChangeCursor(false);
                    botFieldInstance.GetField().ChangeSprite(lastHoveredTile);
                    StateHandler(Destroy(currentTile.name, botFieldInstance), true);
                    return;
                }

            }
        }
        else if (lastHoveredTile != null)
        {
            botFieldInstance.GetField().ChangeSprite(lastHoveredTile);
            lastHoveredTile = null;
            cursor.ChangeCursor(false);
        }
    }


    public void GeneratePosition()
    {
        playerFieldInstance.RandomPositionGenerator();
    }


    public void ResetField()
    {
        playerFieldInstance.ResetField();
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
                    botFieldInstance.transform.position = new Vector3(1, 6, 0);
                    botFieldInstance.gameObject.SetActive(true); // Activate bot
                }
                if (playerFieldInstance != null)
                {
                    // Set player's position to a different vector in the third scene
                    playerFieldInstance.transform.position = new Vector3(-11, 6, 0);
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
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

