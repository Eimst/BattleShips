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
                    botFieldInstance.GetField().ShowBotShips();
                    StartCoroutine(LoadSceneWithDelay("DefeatScene", 6f));
                }
                else
                    StartCoroutine(LoadSceneWithDelay("VictoryScene", 1f));
                break; 
        }
        previousState = currentState;
    }



    private void IsGameOver()
    {
        if (playerFieldInstance.GetRemainingBoats() == 0 || botFieldInstance.GetRemainingBoats() == 0)
            currentState = GameState.GameOver;
    }


    public void GeneratePosition()
    {
        playerFieldInstance.RandomPositionGenerator();
    }


    public void ResetField()
    {
        playerFieldInstance.ResetField();
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
            UIM.FadeInTextPlayerTurn(0.6f);
        }
        else if (currentState == GameState.BotTurn && previousState == GameState.PlayerTurn)
        {
            UIM.FadeOutTextPlayerTurn(0.6f);
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


    private void CheckIfPowersAvailable()
    {
        if (_playerTurnCount % _x3PowerRep == 0)
            UIM.FadeInPowerButton(1);

        if (_playerTurnCount % _hozVerPowerRep == 0)
            UIM.FadeInPowerButton(2);

        if (_playerTurnCount % _sonarPowerRep == 0)
            UIM.FadeInPowerButton(4);
        
        if (_botTurnCount % _x3PowerRep == 0)
            shootingManager.SetAbilityForBot(1);

        if (_botTurnCount % _hozVerPowerRep == 0)
            shootingManager.SetAbilityForBot(2);

        //if (_botTurnCount % _sonarPowerRep == 0)
          //  shootingManager.SetAbilityForBot(4);
    }

    public void SetPowersRep(int x3, int hozVez, int sonar)
    {
        _x3PowerRep = x3;
        _hozVerPowerRep = hozVez;
        _sonarPowerRep = sonar;
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

