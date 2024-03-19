using System;
using UnityEngine;

public class Player : MonoBehaviour, IKillable
{
    public Field playerFieldPrefab;
    private Field field;

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
            remainingBoats--;
        }
        return result;
    }


    public int GetRemainingBoats()
    {
        return remainingBoats;
    }
}
