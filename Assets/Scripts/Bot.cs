using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : MonoBehaviour
{
    public Field playerFieldPrefab;

    // This will hold the instantiated Field component
    private Field field;

    void Start()
    {
        // Instantiate the player's field from the prefab
        if (playerFieldPrefab == null)
        {
            Debug.LogError("playerFieldPrefab is not assigned in the inspector.");
            return;
        }

        // Instantiate the player's field from the prefab
        field = Instantiate(playerFieldPrefab, transform.position, Quaternion.identity);
        field.CreateField();
        if (field == null)
        {
            Debug.LogError("Failed to instantiate playerFieldPrefab.");
            return;
        }
        RandomPositionGenerator();
    }

    // Call this method from GameManager or other classes to get the instantiated Field
    public Field GetFieldInstance()
    {
        return field;
    }

    public void Kill(int x1, int y1)
    {
        field.Kill(x1, y1);
    }
     
    public void RandomPositionGenerator()
    {
        Debug.Log("Button pressed and RandomPositionGenerator called.");

        if (field == null)
        {
            Debug.LogError("Field is not instantiated.");
            return;
        }
        field.CreateField();
        for (int i = 1; i <= 4; i++)
        {
            for (int j = i; j <= 4; j++)
            {
                int count = 0;
                while (true)
                {
                    bool horizontal = false;
                    if (Random.Range(0, 2).Equals(1))
                    {
                        horizontal = true;
                    }
                    int startP = Random.Range(0, 10);
                    int endP = startP + i;
                    if (horizontal)
                    {
                        int yAxis = Random.Range(0, 10);
                        if (field.IsPlaceValid(startP, yAxis, endP, yAxis, horizontal))
                        {
                            field.EnterShip1(startP, yAxis, horizontal, i);
                            break;
                        }

                    }
                    else
                    {
                        int xAxis = Random.Range(0, 10);
                        if (field.IsPlaceValid(xAxis, startP, xAxis, endP, horizontal))
                        {
                            field.EnterShip1(xAxis, startP, horizontal, i);
                            break;
                        }

                    }
                    count++;

                    if (count > 300)
                        break;

                }

            }


        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
