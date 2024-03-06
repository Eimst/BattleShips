using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
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
     
    /*
    public void RandomPositionGenerator()
    {
        Debug.Log("Button pressed and RandomPositionGenerator called.");

        if (field == null)
        {
            Debug.LogError("Field is not instantiated.");
            return;
        }
        field.CreateField();
        for (int i = 4; i >= 1; i--)
        {
            int count = 5 - i;
            int operationCount = 0;
            while(count > 0 && operationCount < 300)
            {
                bool horizontal = false;
                if (Random.Range(0, 2).Equals(1))
                {
                    horizontal = true;
                }
                int startP = 0;

                startP = Random.Range(0, 10);
                

                int endP = startP + i;
                if (horizontal)
                {
                    int yAxis = Random.Range(0, 10);
                    if (field.IsPlaceValid(startP, yAxis, endP, yAxis, horizontal))
                    {
                        field.EnterShip1(startP, yAxis, horizontal, i);
                        count--;
                    }

                }
                else
                {
                    int xAxis = Random.Range(0, 10);
                    if (field.IsPlaceValid(xAxis, startP, xAxis, endP, horizontal))
                    {
                        field.EnterShip1(xAxis, startP, horizontal, i);
                        count--;
                    }

                }
                operationCount++;
            }

        }

    }*/


    // Update is called once per frame
    void Update()
    {
        
    }
}
