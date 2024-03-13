using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour, IKillable
{
    public Field botFieldPrefab;

    // This will hold the instantiated Field component
    private Field field;

    void Start()
    {

        field = Instantiate(botFieldPrefab, transform.position, Quaternion.identity);
        field.transform.SetParent(transform);
        field.CreateField();
        field.SpawnAllShips();
    }


    public void Kill(int x1, int y1)
    {
        field.Kill(x1, y1);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
