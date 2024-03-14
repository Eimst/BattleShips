using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class Bot : MonoBehaviour, IKillable
{
    public Field botFieldPrefab;

    // This will hold the instantiated Field component
    private Field field;

    private int remainingBoats = 20;


    void Start()
    {

        field = Instantiate(botFieldPrefab, transform.position, Quaternion.identity);
        field.transform.SetParent(transform);
        field.CreateField();
        field.SpawnAllShips();
    }


    public string ApplyShot()
    {
        int x = Random.Range(0, 10);
        int y = Random.Range(0, 10);
        return x + " " + y;
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


    // Update is called once per frame
    void Update()
    {
        
    }
}
