using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Field;
using static GameManager;

public class ShootingManager : MonoBehaviour
{

    private GameManager gameManager;

    bool isBotTurnHandled = false;

    int missedCount;

    public enum ChosenAbility
    {
        None,
        x3,
        Horizontal,
        Vertical,
        Sonar
    }


    public ChosenAbility chosenAbility = ChosenAbility.None;

    private GameObject lastHoveredTile = null;

    CursorChanger cursor;

    private int countMissedShot;

    bool skip = false;

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
    }


    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        cursor = FindObjectOfType<CursorChanger>();
        missedCount = 0;
        countMissedShot = 4;
    }


    public void PlayerShoot()
    {
        PlayerMouseTrajectory();
    }
   

    public void BotShoot()
    {
        if (!isBotTurnHandled)
            StartCoroutine(HandleBotTurn());
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
                    if (lastHoveredTile != null && !skip)
                    {
                        if (chosenAbility == ChosenAbility.None)
                            gameManager.GetBotInstance().GetField().ChangeSprite(lastHoveredTile);
                        else 
                            gameManager.GetBotInstance().GetField().ChangeSpriteSpecialAb(lastHoveredTile, chosenAbility);
                    }
                    else
                        skip = false;

                    if (chosenAbility == ChosenAbility.None && gameManager.GetBotInstance().GetField().CheckIfShotPossible(currentTile))
                    {
                        gameManager.GetBotInstance().GetField().ChangeSprite(currentTile);
                        cursor.ChangeToAttack(true);
                    }
                    else 
                    {
                        cursor.ChangeToAttack(false);
                    }
                    
                    if (chosenAbility != ChosenAbility.None && gameManager.GetBotInstance().GetField().ChangeSpriteSpecialAb(currentTile, chosenAbility))
                    {
                        cursor.ChangeToAttack(true);
                    }
                    else if(chosenAbility != ChosenAbility.None)
                    {
                        gameManager.GetBotInstance().GetField().ChangeSprite(currentTile);
                        skip = true;
                    }

                    lastHoveredTile = currentTile;
                }

                if (Input.GetMouseButtonDown(1) && (chosenAbility == ChosenAbility.Horizontal || chosenAbility == ChosenAbility.Vertical))
                {
                    if(!skip) 
                    {
                        gameManager.GetBotInstance().GetField().ChangeSpriteSpecialAb(currentTile, chosenAbility);
                        skip = false;
                    }
                    
                    chosenAbility = chosenAbility == ChosenAbility.Vertical ? ChosenAbility.Horizontal : ChosenAbility.Vertical;

                    if (gameManager.GetBotInstance().GetField().ChangeSpriteSpecialAb(currentTile, chosenAbility))
                    {
                        cursor.ChangeToAttack(true);
                        skip = false;
                    }
                    else
                    {
                        cursor.ChangeToAttack(false);
                        gameManager.GetBotInstance().GetField().ChangeSprite(currentTile);
                        skip = true;
                    }
                        
                }

                if (Input.GetMouseButtonDown(0))
                {
                    
                    if (chosenAbility != ChosenAbility.None)
                    {
                        if (gameManager.GetBotInstance().GetField().ChangeSpriteSpecialAb(currentTile, chosenAbility))
                        {
                            chosenAbility = ChosenAbility.None;
                            cursor.ChangeToAttack(false);
                            // shooting with ability below
                            StateHandler(Destroy(currentTile.name, gameManager.GetBotInstance()), true);
                        }
                        else
                        {
                            gameManager.GetBotInstance().GetField().ChangeSprite(currentTile);
                            Debug.Log("You cant shoot here");
                        }
                        
                    }

                    else if (chosenAbility == ChosenAbility.None)
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
            if (chosenAbility == ChosenAbility.None)
                gameManager.GetBotInstance().GetField().ChangeSprite(lastHoveredTile);
            else
            {
                gameManager.GetBotInstance().GetField().ChangeSpriteSpecialAb(lastHoveredTile, chosenAbility);
            }
            lastHoveredTile = null;
            cursor.ChangeToAttack(false);
        }
    }


    IEnumerator HandleBotTurn()
    {
        isBotTurnHandled = true;
        yield return new WaitForSeconds(1.2f);
        Field.DestroyResult result;
        do
        {
            result = Destroy(gameManager.GetBotInstance().ApplyShot(), gameManager.GetPlayerInstance());

        } while (result == Field.DestroyResult.IllegalMove);

        StateHandler(result, false);

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
                    OnTurnChange?.Invoke(false);
                else
                {
                    OnTurnChange?.Invoke(true);
                    if (!gameManager.GetBotInstance().tryEdges)
                        missedCount++;
                    if (missedCount > countMissedShot)
                    {
                        gameManager.GetBotInstance().tryEdges = true;
                        gameManager.GetBotInstance().ResetEdgesCount();
                        missedCount = 0;
                    }

                }

                gameManager.SendMessage("SpawnText");
                break;

            case DestroyResult.IllegalMove:
                // Handle illegal move
                Debug.Log("Illegal move attempted.");
                if (isPlayer)
                    OnTurnChange?.Invoke(true);
                else
                    OnTurnChange?.Invoke(false);
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
        Debug.Log("Called " + abilityIndex);
        chosenAbility = (ChosenAbility)abilityIndex;
        Debug.Log(chosenAbility);
    }

    public void SetCountMissedShot(int value)
    {
        countMissedShot = value;
    }


}
