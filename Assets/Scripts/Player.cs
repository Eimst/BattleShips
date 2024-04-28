using System;
using UnityEngine;

public class Player : MonoBehaviour, IKillable
{
    public Field playerFieldPrefab;
    private Field field;
    private GameManager gameManager;

    public int remainingBoats = 20;


    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Ensure the player persists across scenes
    }

 
    void Start()
    {
        field = Instantiate(playerFieldPrefab, transform.position, Quaternion.identity);
        field.transform.SetParent(transform);
        field.CreateField();
        gameManager = GetComponentInParent<GameManager>();
    }


    public void RandomPositionGenerator()
    {
        Debug.Log("Triggered");
        if (field != null)
        {
            ResetField();
            field.SpawnAllShips();
            Debug.Log("RandomPositionGenerator called.");
        }
        else
        {
            Debug.LogError("Field is not instantiated.");
        }
    }


    public void ResetField()
    {
        field.CreateField();
    }

    public bool AreAllSpawned()
    {
        return field.AreAllSpawned();
    }


    public Field.DestroyResult Destroy(int x, int y)
    {
        Field.DestroyResult result = field.Destroy(x, y);
        if (result == Field.DestroyResult.Success)
        {
            gameManager.GetBotInstance().SetHitShipStatus(x, y);
            remainingBoats--;
        }
        return result;
    }

    
    public int[] GetShipsCount()
    {
        return field.ShipsCount();
    }

    public int GetRemainingBoats()
    {
        return remainingBoats;
    }

    public Field GetField()
    {
        return field;
    }

    
}
