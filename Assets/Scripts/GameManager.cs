using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using Random = UnityEngine.Random;
using TMPro;
using System;
using UnityEngine.UI;
using static Field;

public class GameManager : MonoBehaviour
{
    public Player playerFieldPrefab;
    public Bot botFieldPrefab;

    private Player playerFieldInstance;
    private Bot botFieldInstance;

    public static GameManager instance;

    private UIManager UIM;

    public CloseOrOpenPanel panelController;

    public enum GameState
    {
        Setup,
        PlayerTurn,
        BotTurn,
        GameOver
    }

    private ShootingManager shootingManager;

    public GameState currentState = GameState.Setup;

    private GameState previousState;

    public bool isAnimationDone { get; set; }

    private int _playerTurnCount;
    
    private int _botTurnCount;
    
    public bool isKeyBindPressed { get; set; }

    public enum GameMode
    {
        Standard,
        Special
    }

    public GameMode gameMode;


    private int _x3PowerRep = 5;

    private int _hozVerPowerRep = 10;

    private int _sonarPowerRep = 8;

    private bool _disappear;

    private bool endIsNotLaunched = true;

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
        isAnimationDone = true;
        _playerTurnCount = 1;
        _botTurnCount = 1;
    }


    void Update()
    {

        if (UIM is not null)
        {
            int[] playerShips = playerFieldInstance.GetShipsCount();
            botFieldInstance.UpdatePlayerShipsCount(playerShips);
            string playerText = playerShips[3].ToString() + "        "
                + playerShips[2].ToString() + "      " + playerShips[1].ToString() + "   " + playerShips[0].ToString();
            UIM.ShowRemainingShips(false, playerText);
            int[] botShips = botFieldInstance.GetShipsCount();

            string botText = botShips[3].ToString() + "        " + 
                botShips[2].ToString() + "      " + botShips[1].ToString() + "   " + botShips[0].ToString();

            UIM.ShowRemainingShips(true, botText);
        }
        if (!isAnimationDone)
            return;

        if (panelController != null && panelController.isOpen)
            return;

        IsGameOver();

        switch (currentState)
        {
            case GameState.PlayerTurn:
                shootingManager.PlayerShoot();
                break;

            case GameState.BotTurn:
                shootingManager.BotShoot();   
                break;

             case GameState.GameOver:
                //Debug.Log(playerFieldInstance.GetRemainingBoats());
                if (playerFieldInstance.GetRemainingBoats() == 0)
                {
                    if (endIsNotLaunched)
                    {
                        botFieldInstance.GetField().ShowBotShips();
                        StartCoroutine(LoadSceneWithDelay("DefeatScene", 6f));
                        endIsNotLaunched = false;
                    }
                }
                else
                {
                    if (endIsNotLaunched)
                    {
                        StartCoroutine(LoadSceneWithDelay("VictoryScene", 0.75f));
                        endIsNotLaunched = false;
                    }
                }
                break;
                 
        }
        previousState = currentState;
    }
    
    
    
    public void SetPowersRep(int x3, int hozVer, int sonar)
    {
        if(x3 == 1 && hozVer == 1 && sonar == 3)
        {
            PlayerPrefs.SetInt("Mode", 0);
            gameMode = GameMode.Standard;
            return;
        }
        
        _x3PowerRep = x3;
        _hozVerPowerRep = hozVer;
        _sonarPowerRep = sonar;
    }

    
    private void IsGameOver()
    {
        if (playerFieldInstance.GetRemainingBoats() == 0 || botFieldInstance.GetRemainingBoats() == 0)
            currentState = GameState.GameOver;
    }


    public void GeneratePosition()
    {
        if (playerFieldInstance.GetField().blinkCount == 0)
        {
            playerFieldInstance.RandomPositionGenerator();
        }
    }


    public void ResetField()
    {
        if (playerFieldInstance.GetField().blinkCount == 0)
        {
            playerFieldInstance.ResetField();
        }
    }


    private void HandleTurnChange(bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            currentState = GameState.PlayerTurn;
            
            if (gameMode == GameMode.Standard) return;
            _botTurnCount++; 
        }
        else
        {
            isKeyBindPressed = false;
            currentState = GameState.BotTurn;
            
            if (gameMode == GameMode.Standard) return;
            _playerTurnCount++; 
        }
        CheckIfPowersAvailable();
    }

    

    private void SpawnText()
    {
        if (currentState == GameState.PlayerTurn && previousState == GameState.BotTurn)
        {
            UIM.FadeInTextPlayerTurn(0.4f);
        }
        else if (currentState == GameState.BotTurn && previousState == GameState.PlayerTurn)
        {
            UIM.FadeOutTextPlayerTurn(0.4f);
        }
    }
    
    
    public bool isThereNoShipLeft()
    {
        return botFieldInstance.GetRemainingBoats() == 0;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals("BattleScene"))
        {
            playerFieldInstance.GetField().shipsCount = new int[] { 4, 3, 2, 1 };

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
            // this.mode = PlayerPrefs.GetInt("Mode") == 0 ? GameMode.Standard : GameMode.Special;
            currentState = Random.Range(0, 2) == 0 ? GameState.PlayerTurn : GameState.BotTurn;
            previousState = currentState == GameState.PlayerTurn ? GameState.BotTurn : GameState.PlayerTurn;

            if (ShootingManager.Instance == null)
            {
                GameObject shootingManagerObject = new GameObject("ShootingManager");
                shootingManagerObject.AddComponent<ShootingManager>();
            }

            shootingManager = ShootingManager.Instance;

            // Subscribe to the ShootingManager's event
            if (shootingManager != null)
            {
                shootingManager.OnTurnChange += HandleTurnChange;
            }

            botFieldInstance.SetShootingManager(shootingManager);
            UIM = FindObjectOfType<UIManager>();
            panelController = FindObjectOfType<CloseOrOpenPanel>();
            if (PlayerPrefs.GetInt("Mode") == 1)
            {
                UIM.AddSpecialPower();
                gameMode = GameMode.Special;
                CheckIfPowersAvailable();
            }
            else gameMode = GameMode.Standard;
            SpawnText();
            
        }
    }



    public bool PermissionToUsePowers()
    {
        if(currentState == GameState.PlayerTurn && isAnimationDone)
        {
            StartCoroutine(StopGameUntilPowerAnimationFadesOut());
            return true;
        }
        return false;
    }


    IEnumerator StopGameUntilPowerAnimationFadesOut()
    {
        isAnimationDone = false;

        yield return new WaitForSeconds(0.8f);
        isAnimationDone = true;
    }


    IEnumerator LoadSceneWithDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);

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


    public Player GetPlayerInstance()
    {
        return playerFieldInstance;
    }


    public Bot GetBotInstance()
    {
        return botFieldInstance;
    }


    public void UpdateBotVision()
    {
        botFieldInstance.UpdateBotVision(playerFieldInstance.GetField().GetBoardVision());
    }

    /// <summary>
    /// Checks if powers available for bot or player
    /// </summary>
    private void CheckIfPowersAvailable()
    {
        
        if (_x3PowerRep > 1 && _playerTurnCount % _x3PowerRep == 0)
            UIM.FadeInPowerButton(1);

        if (_hozVerPowerRep > 1 && _playerTurnCount % _hozVerPowerRep == 0)
            UIM.FadeInPowerButton(2);

        if (_sonarPowerRep > 3 && _playerTurnCount % _sonarPowerRep == 0)
            UIM.FadeInPowerButton(4);
        
        if (_x3PowerRep > 1 && _botTurnCount % _x3PowerRep == 0)
            shootingManager.SetAbilityForBot(1);

        if (_hozVerPowerRep > 1 && _botTurnCount % _hozVerPowerRep == 0)
            shootingManager.SetAbilityForBot(2);

        if (_sonarPowerRep > 3 && _botTurnCount % _sonarPowerRep == 0)
            shootingManager.SetAbilityForBot(4);
    }


    /// <summary>
    /// Returns the sonar results from the player board.
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="vision"></param>
    /// <returns></returns>
    public Stack<String> ReturnSonarResultFromPlayerBoard(string coord, ref int[,] vision)
    {
        return playerFieldInstance.GetField().ReturnSonarResults(coord, ref vision);
    }


    /// <summary>
    /// Initiates the sonar effect animation to show detected ships.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="delay"></param>
    /// <param name="isPlayer"></param>
    public void ShowDetectedShipsWithSonar(int x, int y, float delay, bool isPlayer)
    {
        if(isPlayer)
            StartCoroutine(botFieldInstance.GetField().SonarTileChanger(x, y, this, delay, isPlayer));
        else
            StartCoroutine(playerFieldInstance.GetField().SonarTileChanger(x, y, this, delay, isPlayer));
    }
    
    /// <summary>
    /// Prepares the game board for key bind activation.
    /// </summary>
    public void PrepareBoardForKeyBindActivation()
    {
        Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);
        if (hit.collider.transform.position.x >= 2 && hit.collider.transform.position.x <= 12 &&
            hit.collider.transform.position.y <= 5 && hit.collider.transform.position.y >= -5)
        {
            GameObject currentTile = hit.collider.gameObject;
            botFieldInstance.GetField().ChangeSprite(currentTile);
            // Reset cursor and last hovered tile for shooting manager
            shootingManager.SetCursorToDefault();
            shootingManager.SetLastHoveredTile();
        }
    }

    private void OnDestroy()
    {
        // Always make sure to unsubscribe from the event when the GameObject is destroyed
        if (shootingManager != null)
        {
            shootingManager.OnTurnChange -= HandleTurnChange;
        }
    }

}

