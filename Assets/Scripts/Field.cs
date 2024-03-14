using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Field : MonoBehaviour
{
    public GameObject letter, number, square;

    public GameObject[] letters;
    public GameObject[] numbers;
    public GameObject[,] field; 

    int fieldLength = 10;

    public int[,] fieldArray;

    public int[] shipsCount = new int[] { 4, 3, 2, 1};

    public enum DestroyResult
    {
        Success,
        Failure,
        IllegalMove
    }


    public void CreateField()
    {
        if (letters != null)
        {
            foreach (GameObject letterObj in letters)
            {
                if (letterObj != null) Destroy(letterObj);
            }
        }

        if (numbers != null)
        {
            foreach (GameObject numberObj in numbers)
            {
                if (numberObj != null) Destroy(numberObj);
            }
        }

        if (field != null)
        {
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    if (field[i, j] != null) Destroy(field[i, j]);
                }
            }
        }
        letters = new GameObject[fieldLength];
        numbers = new GameObject[fieldLength];
        field = new GameObject[fieldLength, fieldLength];
        // Reset the fieldArray to a new, empty state
        fieldArray = new int[fieldLength, fieldLength];
        Vector3 startPos = transform.position;
        float X = startPos.x + 1;
        float Y = startPos.y - 1;

        letters = new GameObject[fieldLength];
        numbers = new GameObject[fieldLength];

        for (int i = 0; i < fieldLength; i++)
        {
            letters[i] = Instantiate(letter, new Vector3(X, Y, startPos.z), Quaternion.identity, transform);
            letters[i].transform.position = new Vector3(X, startPos.y, startPos.z);
            letters[i].GetComponent<Chunks>().index = i;
            X++;

            numbers[i] = Instantiate(number, new Vector3(X, Y, startPos.z), Quaternion.identity, transform);
            numbers[i].transform.position = new Vector3(startPos.x, Y, startPos.z);
            numbers[i].GetComponent<Chunks>().index = i;
            Y--;
        }

        X = startPos.x + 1;
        Y = startPos.y - 1;

        field = new GameObject[fieldLength, fieldLength];

        for (int j = 0; j < fieldLength; j++)
        {
            for (int i = 0; i < fieldLength; i++)
            {
                field[i, j] = Instantiate(square, new Vector3(X, Y, startPos.z), Quaternion.identity, transform);
                field[i, j].GetComponent<Chunks>().index = 0;
                field[i, j].transform.position = new Vector3(X, Y, startPos.z);
                field[i, j].name = $"{i} {j}"; // Name for easier identification

                // Add a BoxCollider2D component to the square
                var collider = field[i, j].AddComponent<BoxCollider2D>();
                collider.isTrigger = true; // Make it a trigger if you don't want physical collisions
                X++;
            }
            X = startPos.x + 1;
            Y--;
        }
    }


    /*
    public bool IsPlaceValid(int x1, int y1, int x2, int y2, bool horizontal)
    {
        List<int> index = new List<int> {1,3,5,7,2,4,6,8};
        int startP;
        int endP;

        if (horizontal)
        {
            if (x1 + x2 > 9 || y1 > 9) return false;
            startP = x1 - 1 >= 0 ? x1 - 1 : x1;
            endP = x2 + 1 < 10 ? x2 + 1 : x2;
        }
        else
        {
            if (y1 + y2 > 9 || x1 > 9) return false;
            startP = y1 - 1 >= 0 ? y1 - 1 : y1;
            endP = y2 + 1 < 10 ? y2 + 1 : y2;
        }
        for (int i = startP; i <= endP; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (horizontal)
                {
                    if (y1 + j < 0 || y1 + j > 9)
                        continue;

                    if (index.Contains(field[i, y1 + j].GetComponent<Chunks>().index))
                        return false;
                }
                else
                {
                    if (x1 + j < 0 || x1 + j > 9)
                        continue;

                    if (index.Contains(field[x1 + j, i].GetComponent<Chunks>().index))
                        return false;
                }
            }
           
        }
        return true;
    }


    public void EnterShip1(int x1, int y1, bool horizontal, int k)
    {
        if (k == 4)
        {
            if(horizontal)
            {
                field[x1, y1].GetComponent<Chunks>().index = 1;
                field[x1 + 1, y1].GetComponent<Chunks>().index = 3;
                field[x1 + 2, y1].GetComponent<Chunks>().index = 5;
                field[x1 + 3, y1].GetComponent<Chunks>().index = 7;
            }
            else
            {
                field[x1, y1].GetComponent<Chunks>().index = 2;
                field[x1, y1 + 1].GetComponent<Chunks>().index = 4;
                field[x1, y1 + 2].GetComponent<Chunks>().index = 6;
                field[x1, y1 + 3].GetComponent<Chunks>().index = 8;
            }
            
        }
        else if(k == 3)
        {
            if (horizontal)
            {
                field[x1, y1].GetComponent<Chunks>().index = 1;
                field[x1 + 1, y1].GetComponent<Chunks>().index = 5;
                field[x1 + 2, y1].GetComponent<Chunks>().index = 7;
            }
            else
            {
                field[x1, y1].GetComponent<Chunks>().index = 2;
                field[x1, y1 + 1].GetComponent<Chunks>().index = 6;
                field[x1, y1 + 2].GetComponent<Chunks>().index = 8;
            }

        }
        else if(k == 2)
        {
            if (horizontal)
            {
                field[x1, y1].GetComponent<Chunks>().index = 1;
                field[x1 + 1, y1].GetComponent<Chunks>().index = 7;
            }
            else
            {
                field[x1, y1].GetComponent<Chunks>().index = 2;
                field[x1, y1 + 1].GetComponent<Chunks>().index = 8;
            }
        }
        else
        {
            if (horizontal)
            {
                field[x1, y1].GetComponent<Chunks>().index = 1;
            }
            else
            {
                field[x1, y1].GetComponent<Chunks>().index = 2;
            }
        }
        field[x1, y1].GetComponent<Chunks>().UpdateLetter();
    }
    */


    public bool IsWrongPlace(int x1, int y1, int x2, int y2)
    {
        if (x1 >= 0 && x1 <= 9 && x2 >= 0 && x2 <= 9 && y1 >= 0 && y1 <= 9 && y2 >= 0 && y2 <= 9)
        {
            if (fieldArray[x1, y1] == 1 || fieldArray[x2, y2] == 1)
                return true;
            else return false;
        }
        return true;
    }



    public void EnterShip(int x1, int y1, int x2, int y2, ref bool done)
    {
        if (x2 - x1 >= y2 - y1)
            if (shipsCount[x2 - x1] == 0)
                return;
        if (y2 - y1 > x2 - x1)
            if (shipsCount[y2 - y1] == 0)
                return;

        if (IsWrongPlace(x1, y1, x2, y2)) return;

        done = true;

        int[][] dir = {
            new int[] { -1, 0 },
            new int[] { -1, 1 },
            new int[] { 0, 1 },
            new int[] { 1, 1 },
            new int[] { 1, 0 },
            new int[] { 1, -1 },
            new int[] { 0, -1 },
            new int[] { -1, -1 }
        };

        if (x2 - x1 > 0)
        {
            for (int k = 0; k < x2 - x1 + 1; k++)
            {
                fieldArray[x1 + k, y1] = 1;
                for (int i = 0; i < 8; i++)
                {
                    if (x1 + k + dir[i][0] >= 0 && x1 + k + dir[i][0] <= 9 && y1 + dir[i][1] >= 0 && y1 + dir[i][1] <= 9)
                        fieldArray[x1 + k + dir[i][0], y1 + dir[i][1]] = 1;
                }
                if (k == 0)
                {
                    field[x1 + k, y1].GetComponent<Chunks>().index = 1;
                    continue;
                }
                if (k == 1)
                {
                    if (x1 + k == x2)
                        field[x1 + k, y1].GetComponent<Chunks>().index = 7;
                    else
                        field[x1 + k, y1].GetComponent<Chunks>().index = 3;
                    continue;
                }
                if (k == 2)
                {
                    if (x1 + k == x2)
                        field[x1 + k, y1].GetComponent<Chunks>().index = 7;
                    else
                        field[x1 + k, y1].GetComponent<Chunks>().index = 5;
                    continue;
                }
                if (k == 3) field[x1 + k, y1].GetComponent<Chunks>().index = 7;
            }
            shipsCount[x2 - x1]--;
        }
        else if (y2 - y1 > 0)
        {
            for (int k = 0; k < y2 - y1 + 1; k++)
            {
                fieldArray[x1, y1 + k] = 1;
                for (int i = 0; i < 8; i++)
                {
                    if (x1 + dir[i][0] >= 0 && x1 + dir[i][0] <= 9 && y1 + k + dir[i][1] >= 0 && y1 + k + dir[i][1] <= 9)
                        fieldArray[x1 + dir[i][0], y1 + k + dir[i][1]] = 1;
                }
                if (k == 0)
                {
                    field[x1, y1 + k].GetComponent<Chunks>().index = 2;
                    continue;
                }
                if (k == 1)
                {
                    if (y1 + k == y2)
                        field[x1, y1 + k].GetComponent<Chunks>().index = 8;
                    else
                        field[x1, y1 + k].GetComponent<Chunks>().index = 4;
                    continue;
                }
                if (k == 2)
                {
                    if (y1 + k == y2)
                        field[x1, y1 + k].GetComponent<Chunks>().index = 8;
                    else
                        field[x1, y1 + k].GetComponent<Chunks>().index = 6;
                    continue;
                }
                if (k == 3) field[x1, y1 + k].GetComponent<Chunks>().index = 8;
            }
            shipsCount[y2 - y1]--;
        }
        else if (x2 - x1 == 0 && y2 - y1 == 0)
        {
            fieldArray[x1, y1] = 1;
            for (int i = 0; i < 8; i++)
            {
                if (x1 + dir[i][0] >= 0 && x1 + dir[i][0] <= 9 && y1 + dir[i][1] >= 0 && y1 + dir[i][1] <= 9)
                    fieldArray[x1 + dir[i][0], y1 + dir[i][1]] = 1;
            }
            field[x1, y1].GetComponent<Chunks>().index = 1;
            shipsCount[0]--;
        }
    }


    public bool AreAllSpawned()
    {
        bool spawned = true;
        for (int i = 0; i < 4; i++)
        {
            if (shipsCount[i] > 0)
                spawned = false;
        }
        return spawned;
    }


    public void SpawnAllShips()
    {
        shipsCount = new int[] { 4, 3, 2, 1};

        int currShip = 3;

        int count = 0;

        string[] variations = new string[] { "1234", "1243", "1324", "1342", "1423", "1432", "2134", "2143",
            "2314", "2341", "2413", "2431", "3124", "3142", "3214", "3241", "3412", "3421", "4123", "4132", 
            "4231", "4213", "4312", "4321" };

        while (true)
        {
            if (AreAllSpawned() || count >= 500)
            {
                return;
            }
            int x = Random.Range(0, 10);
            int y = Random.Range(0, 10);
            int index = Random.Range(0, variations.Length);
            string var = variations[index];
            count++;
            bool spawned = false;
            if (shipsCount[currShip] == 0)
                currShip--;
            foreach (char num in var)
            {
                if (num.Equals('1'))
                {
                    EnterShip(x, y, x + currShip, y, ref spawned);
                    if (spawned)
                        break; 
                }
                if (num.Equals('2'))
                {
                    EnterShip(x - currShip, y, x, y, ref spawned);
                    if (spawned)
                        break;
                }
                if (num.Equals('3'))
                {
                    EnterShip(x, y, x, y + currShip, ref spawned);
                    if (spawned)
                        break; ;
                }
                if (num.Equals('4'))
                {
                    EnterShip(x, y - currShip, x, y, ref spawned);
                    if (spawned)
                        break;
                }
            }
        }
        
    }


     public DestroyResult Destroy(int x1, int y1)
     {
        if (field[x1, y1].GetComponent<Chunks>().index == 9 || field[x1, y1].GetComponent<Chunks>().index == 10)
            return DestroyResult.IllegalMove;

        if (field[x1, y1].GetComponent<Chunks>().index == 0)
        {
            field[x1, y1].GetComponent<Chunks>().index = 9;
            return DestroyResult.Failure;
        }
            
        else 
        {
            field[x1, y1].GetComponent<Chunks>().index = 10;
            return DestroyResult.Success;
        }
            
    }
    

    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
