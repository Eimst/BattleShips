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

    private UIManager textUIM;

    private GameObject lastHoveredTile = null;

    CursorChanger cursor;

    private bool isFirstTime = true;
    private float timer = 0f;
    public int xx = 0;

    public enum GameState
    {
        Setup,
        PlayerTurn,
        BotTurn,
        GameOver
    }

    public GameState currentState = GameState.Setup;

    private GameState previousState;

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
                if (playerFieldInstance.GetRemainingBoats() == 0)
                {
                    timer = timer + Time.deltaTime;
                    if (isFirstTime)
                    {
                        botFieldInstance.ShowBotShips();
                    }
                    if (timer > 5.0)
                    {
                        StartCoroutine(LoadSceneWithDelay("DefeatScene"));
                    }
                    isFirstTime = false;
                }
                else
                    StartCoroutine(LoadSceneWithDelay("VictoryScene"));
                break;
        }
        previousState = currentState;
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
        yield return new WaitForSeconds(1.2f);
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
                break;

            case DestroyResult.Failure:
                // Handle failure (move hit water)
                if (isPlayer)
                    currentState = GameState.BotTurn;
                else
                    currentState = GameState.PlayerTurn;
                SpawnText();
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


    void SpawnText()
    {
        if (currentState == GameState.PlayerTurn && previousState == GameState.BotTurn)
        {
            textUIM.FadeInTextPlayerTurn(0.6f);
        }
        else if (currentState == GameState.BotTurn && previousState == GameState.PlayerTurn)
        {
            textUIM.FadeOutTextPlayerTurn(0.6f);
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
        if (scene.name.Equals("BattleScene"))
        {
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
            previousState = currentState == GameState.PlayerTurn ? GameState.BotTurn : GameState.PlayerTurn;
            textUIM = FindObjectOfType<UIManager>();
            SpawnText();
        }
    }


    IEnumerator LoadSceneWithDelay(string sceneName)
    {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Destroy(instance.gameObject);
        instance = null;
    }


    public void GoToBattle()
    {
        if (playerFieldInstance.AreAllSpawned())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}

