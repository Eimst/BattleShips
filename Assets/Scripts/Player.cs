using System;
using UnityEngine;

public class Player : MonoBehaviour, IKillable
{
    public Field playerFieldPrefab;
    private Field field;

    public int remainingBoats = 20;

    public bool ship4Tiles { get; set; }


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
            field.CreateField();
            field.SpawnAllShips();
            Debug.Log("RandomPositionGenerator called.");
        }
        else
        {
            Debug.LogError("Field is not instantiated.");
        }
    }


    public bool AreAllSpawned()
    {
        return field.AreAllSpawned();
    }
    

    public bool Kill(int x1, int y1)
    {
        if(field.Kill(x1, y1))
        {
            remainingBoats--;
            return true;
        }
        return false;
    }


    public int GetRemainingBoats()
    {
        return remainingBoats;
    }

}
