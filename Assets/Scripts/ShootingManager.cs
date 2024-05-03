using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static Field;
using static GameManager;

public class ShootingManager : MonoBehaviour
{
    private GameManager gameManager;

    bool isBotTurnHandled = false;

    int missedCount;

    private UIManager UIM;

    private Renderer _renderer;

    public enum ChosenAbility
    {
        None,
        x3,
        Horizontal,
        Vertical,
        Sonar
    }


    [FormerlySerializedAs("chosenAbility")] 
    public ChosenAbility playerChosenAbility = ChosenAbility.None;

    private List<ChosenAbility> availableAbilitiesBot;
    
    public ChosenAbility botChosenAbility = ChosenAbility.None;
    
    private GameObject lastHoveredTile = null;

    CursorChanger cursor;

    private int countMissedShot;

    public delegate void TurnChangeHandler(bool isPlayerTurn);
    public event TurnChangeHandler OnTurnChange;

    public static ShootingManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject); // Ensures singleton pattern
        }
        else
        {
            Instance = this;
        }
        availableAbilitiesBot = new List<ChosenAbility>();
    }


    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        cursor = FindObjectOfType<CursorChanger>();
        missedCount = 0;
        countMissedShot = 4;
        
        UIM = FindObjectOfType<UIManager>();
    }


    public void PlayerShoot()
    {
        PlayerMouseTrajectory();
    }
   

    public void BotShoot()
    {
        if (!isBotTurnHandled)
        {
            StartCoroutine(HandleBotTurn());
        }
            
    }


    private void PlayerMouseTrajectory() //  Y pozicija visada tokia pati
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
                        if (playerChosenAbility == ChosenAbility.None)
                            gameManager.GetBotInstance().GetField().ChangeSprite(lastHoveredTile);
                        else 
                            gameManager.GetBotInstance().GetField().ChangeSpriteSpecialAb(lastHoveredTile, playerChosenAbility);
                    }

                    if (playerChosenAbility == ChosenAbility.None && gameManager.GetBotInstance().GetField().CheckIfShotPossible(currentTile))
                    {
                        gameManager.GetBotInstance().GetField().ChangeSprite(currentTile);
                        cursor.ChangeToAttack(true);
                    }
                    else 
                    {
                        cursor.ChangeToAttack(false);
                    }
                    
                    if (playerChosenAbility != ChosenAbility.None && gameManager.GetBotInstance().GetField().ChangeSpriteSpecialAb(currentTile, playerChosenAbility))
                    {
                        cursor.ChangeToAttack(true);
                    }
                    
                    lastHoveredTile = currentTile;
                }

                if (Input.GetMouseButtonDown(1) && (playerChosenAbility == ChosenAbility.Horizontal || playerChosenAbility == ChosenAbility.Vertical))
                {
                     gameManager.GetBotInstance().GetField().ChangeSpriteSpecialAb(currentTile, playerChosenAbility);
                     lastHoveredTile = null;
                    
                    playerChosenAbility = playerChosenAbility == ChosenAbility.Vertical ? ChosenAbility.Horizontal : ChosenAbility.Vertical;

                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (playerChosenAbility != ChosenAbility.None)
                    {
                        if (gameManager.GetBotInstance().GetField().ChangeSpriteSpecialAb(currentTile, playerChosenAbility))
                        {
                            cursor.ChangeToAttack(false);
                            // shooting with ability below
                            SpecialAbilityUsed(playerChosenAbility, currentTile.name, true);
                            playerChosenAbility = ChosenAbility.None;
                        }
                        else
                        {
                            gameManager.GetBotInstance().GetField().ChangeSpriteSpecialAb(currentTile, playerChosenAbility);
                            Debug.Log("You cant shoot here");
                        }
                    }
                    else if (playerChosenAbility == ChosenAbility.None)
                    {
                        cursor.ChangeToAttack(false);
                        gameManager.GetBotInstance().GetField().ChangeSprite(lastHoveredTile);
                        StateHandler(Destroy(currentTile.name, gameManager.GetBotInstance()), true);
                    } 
                }
            }
        }
        else if (lastHoveredTile != null)
        {
            if (playerChosenAbility == ChosenAbility.None)
                gameManager.GetBotInstance().GetField().ChangeSprite(lastHoveredTile);
            else
            {
                gameManager.GetBotInstance().GetField().ChangeSpriteSpecialAb(lastHoveredTile, playerChosenAbility);
            }
            lastHoveredTile = null;
            cursor.ChangeToAttack(false);
        }
    }


    IEnumerator HandleBotTurn()
    {
        isBotTurnHandled = true;
        
        if(availableAbilitiesBot.Count == 0 || !gameManager.GetBotInstance().IsAbilityIsNeeded())
        {
            yield return new WaitForSeconds(1.2f);
            Field.DestroyResult result;
            do
            {
                result = Destroy(gameManager.GetBotInstance().ApplyShot(), gameManager.GetPlayerInstance());

            } while (result == Field.DestroyResult.IllegalMove);

            StateHandler(result, false);
        }
        else
        {
            Debug.Log("BotHasAbility");
            string coordinates = gameManager.GetBotInstance().ApplyShotWithSpecialAbility(availableAbilitiesBot);
            UIM.FadeInTextBotAb(0.3f, botChosenAbility);
            
            yield return new WaitForSeconds(2f);
            SpecialAbilityUsed(botChosenAbility, coordinates, false);
            botChosenAbility = ChosenAbility.None;
            availableAbilitiesBot.Clear();
            UIM.FadeOutTextBotAb(0.3f);
        }

        isBotTurnHandled = false;

    }


    private void StateHandler(Field.DestroyResult result, bool isPlayer)
    {
        switch (result)
        {
            case DestroyResult.Success:
                if (!isPlayer)
                {
                    gameManager.GetBotInstance().hit = true;
                    missedCount = 0;
                }

                break;

            case DestroyResult.Failure:
                if (isPlayer)
                {
                    UIM.FadeOutPowerButton();
                    OnTurnChange?.Invoke(false);
                }
                else
                {
                    OnTurnChange?.Invoke(true);
                    
                    if (gameManager.gameMode == GameMode.Standard)
                    {
                        if (!gameManager.GetBotInstance().tryEdges)
                            missedCount++;
                        if (missedCount > countMissedShot)
                        {
                            gameManager.GetBotInstance().tryEdges = true;
                            gameManager.GetBotInstance().ResetEdgesCount();
                            missedCount = 0;
                        }
                    }

                }

                gameManager.SendMessage("SpawnText");
                break;

            case DestroyResult.IllegalMove:
                // Handle illegal move
                Debug.Log("Illegal move attempted.");
                
                break;
        }
    }


    private Field.DestroyResult Destroy(string coordinates, IKillable instance)
    {
        int x = int.Parse(coordinates.Split(' ')[0]);
        int y = int.Parse(coordinates.Split(' ')[1]);
        return instance.Destroy(x, y);
    }


    public void SetAbility(int abilityIndex)
    {
        if(!gameManager.PermissionToUsePowers())
            return;
        
        Debug.Log("Called " + abilityIndex);
        playerChosenAbility = (ChosenAbility)abilityIndex;
        
        if (playerChosenAbility == ChosenAbility.Sonar)
        {
            _renderer = GameObject.Find("GreenPower").GetComponent<Renderer>();
            GameObject.Find("GreenPowerSound").GetComponent<AudioSource>().Play();
            _renderer.enabled = true;
            Invoke("DisableRenderer", 0.8f);
        }
        else if (playerChosenAbility == ChosenAbility.x3)
        {
            _renderer = GameObject.Find("RedPower").GetComponent<Renderer>();
            GameObject.Find("RedPowerSound").GetComponent<AudioSource>().Play();
            _renderer.enabled = true;
            Invoke("DisableRenderer", 0.8f);
        }
        else if (playerChosenAbility == ChosenAbility.Horizontal)
        {
            _renderer = GameObject.Find("BluePower").GetComponent<Renderer>();
            GameObject.Find("BluePowerSound").GetComponent<AudioSource>().Play();
            _renderer.enabled = true;
            Invoke("DisableRenderer", 0.8f);
        }
        GameObject.Find("FadePanel").GetComponent<Animator>().Play("FadeIn");
        Invoke("FadeOut", 0.4f);
        Debug.Log(playerChosenAbility);
    }


    public void SetAbilityForBot(int abilityIndex)
    {
        availableAbilitiesBot.Add((ChosenAbility)abilityIndex);
    }

    
    private void DisableRenderer()
    {
        _renderer.enabled = false;

    }

    private void FadeOut()
    {
        GameObject.Find("FadePanel").GetComponent<Animator>().Play("FadeOut");
    }

    public void SetCountMissedShot(int value)
    {
        countMissedShot = value;
    }

    private void SpecialAbilityUsed(ChosenAbility chosenAbility, string coordinates, bool isPlayer)
    {
        int x = int.Parse(coordinates.Split(' ')[0]);
        int y = int.Parse(coordinates.Split(' ')[1]);
        
        switch (chosenAbility)
        {
            case ChosenAbility.Horizontal:
                HorizontalAbilityUsed(y, isPlayer);
                break;
            case ChosenAbility.Vertical:
                VerticalAbilityUsed(x, isPlayer);
                break;
            case ChosenAbility.x3:
                x3AbilityUsed(x, y, isPlayer);
                break;
            case ChosenAbility.Sonar:
                SonarAbilityUsed(x, y, isPlayer, 4f);
                break;
        }
        gameManager.SendMessage("SpawnText");
    }

    private void HorizontalAbilityUsed(int y, bool isPlayer)
    {
        for (int i = 0; i < 10; i++)
        {
            if (gameManager.isThereNoShipLeft()) { break; }
            if (isPlayer)
                Destroy(i + " " + y, gameManager.GetBotInstance());
            else
            {
                Destroy(i + " " + y, gameManager.GetPlayerInstance());
                isBotTurnHandled = false;
            }
        }
        OnTurnChange?.Invoke(!isPlayer);
    }

    private void VerticalAbilityUsed(int x, bool isPlayer)
    {
        for (int i = 0; i < 10; i++)
        {
            if (gameManager.isThereNoShipLeft()) { break; }
            if (isPlayer)
                Debug.Log(Destroy(x + " " + i, gameManager.GetBotInstance()));
            else
            {
                Destroy(x + " " + i, gameManager.GetPlayerInstance());
            }
        }
        OnTurnChange?.Invoke(!isPlayer);
    }

    private void x3AbilityUsed(int x, int y, bool isPlayer)
    {
        for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 2 > 10 ? 10 : x + 2); i++)
        {
            for (int j = y - 1 < 0 ? 0 : y - 1; j < (y + 2 > 10 ? 10 : y + 2); j++)
            {
                if (gameManager.isThereNoShipLeft()) { break; }

                if (isPlayer)
                    Debug.Log(Destroy(i + " " + j, gameManager.GetBotInstance()));
                else
                {
                    Destroy(i + " " + j, gameManager.GetPlayerInstance());
                }

            }
        }
        OnTurnChange?.Invoke(!isPlayer);
    }

    private void SonarAbilityUsed(int x, int y, bool isPlayer, float delay)
    {
        if (!isPlayer)
        {
            gameManager.ShowToPlayerWhichTilesBotDetectedWithSonar(x, y);
        }
        else
        {
            Debug.Log("d>" + delay);
            gameManager.ShowToPlayerWhichTilesPlayerDetectedWithSonar(x, y, delay);
        }
        OnTurnChange?.Invoke(!isPlayer);
    }

    public void SetLastHoveredTile()
    {
        lastHoveredTile = null;
    }


    public void SetCursorToDefault()
    {
        cursor.ChangeToAttack(false);
    }
}
