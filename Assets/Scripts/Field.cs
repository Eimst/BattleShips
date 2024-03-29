using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public int currShipNum = 1;
    public int[,] shipsArray;

    public int[] shipsCount;

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
        shipsCount = new int[] { 4, 3, 2, 1 };
        letters = new GameObject[fieldLength];
        numbers = new GameObject[fieldLength];
        field = new GameObject[fieldLength, fieldLength];
        // Reset the fieldArray to a new, empty state
        fieldArray = new int[fieldLength, fieldLength];
        shipsArray = new int[fieldLength, fieldLength];
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

    public bool ChangeSprite(GameObject coordinates)
    {
        int x = int.Parse(coordinates.name.Split(' ')[0]);
        int y = int.Parse(coordinates.name.Split(' ')[1]);
        if (field[x, y].GetComponent<Chunks>().index == 20)
        {
            field[x, y].GetComponent<Chunks>().index = 0;
        }
           
        else if (field[x, y].GetComponent<Chunks>().index == 0)
        {
            field[x, y].GetComponent<Chunks>().index = 20;
            return true;
        }
        return false;    
    }


    public void MaskField()
    {
        for (int i = 0; i < fieldLength; i++)
        {
            for (int j = 0; j < fieldLength; j++)
            {
                field[i, j].GetComponent<Chunks>().index = 0;
            }
        }
    }

   

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
                shipsArray[x1 + k, y1] = currShipNum;
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
                shipsArray[x1, y1 + k] = currShipNum;
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
            shipsArray[x1, y1] = currShipNum;
            fieldArray[x1, y1] = 1;
            for (int i = 0; i < 8; i++)
            {
                if (x1 + dir[i][0] >= 0 && x1 + dir[i][0] <= 9 && y1 + dir[i][1] >= 0 && y1 + dir[i][1] <= 9)
                    fieldArray[x1 + dir[i][0], y1 + dir[i][1]] = 1;
            }
            field[x1, y1].GetComponent<Chunks>().index = 1;
            shipsCount[0]--;
        }
        currShipNum++;
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
        int[] illegalIndexes = new int[] { 9, 10, 12, 13, 14, 15, 16, 17, 18, 19 };
        if (illegalIndexes.Contains(field[x1, y1].GetComponent<Chunks>().index))
            return DestroyResult.IllegalMove;
            
        else 
        {
            if (shipsArray[x1, y1] == 0)
            {
                field[x1, y1].GetComponent<Chunks>().index = 9;
                return DestroyResult.Failure;
            }
            field[x1, y1].GetComponent<Chunks>().index = 10;
            if (IsAllShipDestroyed(shipsArray[x1, y1], x1, y1))
                DestroyAllShip(x1, y1, shipsArray[x1, y1]);
            shipsArray[x1, y1] *= -1;
            return DestroyResult.Success;
        }
            
    }
    public bool IsAllShipDestroyed(int shipNum, int x, int y)
    {
        for (int i = 0; i < fieldLength; i++)
        {
            for (int j = 0; j < fieldLength; j++)
            {
                if (shipsArray[i, j] == shipNum && (i != x || j != y))
                    return false;
            }
        }
        return true;
    }

    public void DestroyAllShip(int x, int y, int shipNum)
    {
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
        shipsArray[x, y] *= -1;
        int size = -1;
        string type = "";
        int startX = 10;
        int startY = 10;
        for (int j = 0; j < fieldLength; j++)
        {
            for (int i = 0; i < fieldLength; i++)
            {
                if (shipsArray[i, j] == shipNum * (-1))
                {
                    if (startX == 10 && startY == 10)
                    {
                        startX = i;
                        startY = j;
                        if ((i + 1) <= 9)
                            if (shipsArray[i + 1, j] == (shipNum * (-1)))
                                type = "h";
                            else type = "v";
                        else if ((j + 1) <= 9)
                            if (shipsArray[i, j + 1] == (shipNum * (-1)))
                                type = "v";
                            else type = "h";
                    }
                    size++;
                }
            }
        }
        if (size == 0)
        {
            field[x, y].GetComponent<Chunks>().index = 12;
            for (int i = 0; i < 8; i++)
            {
                if (x + dir[i][0] >= 0 && x + dir[i][0] <= 9 && y + dir[i][1] >= 0 && y + dir[i][1] <= 9)
                    if (field[x + dir[i][0], y + dir[i][1]].GetComponent<Chunks>().index == 0)
                        field[x + dir[i][0], y + dir[i][1]].GetComponent<Chunks>().index = 9;
            }

        }
        else
        {
            for (int i = 0; i <= size; i++)
            {
                if (type == "h")
                {
                    if (i == 0)
                    {
                        field[startX + i, startY].GetComponent<Chunks>().index = 12;
                    }
                    else if (i == 1)
                    {
                        if (i == size)
                            field[startX + i, startY].GetComponent<Chunks>().index = 18;
                        else field[startX + i, startY].GetComponent<Chunks>().index = 14;
                    }
                    else if (i == 2)
                    {
                        if (i == size)
                            field[startX + i, startY].GetComponent<Chunks>().index = 18;
                        else field[startX + i, startY].GetComponent<Chunks>().index = 16;
                    }
                    else if (i == 3)
                    {
                        field[startX + i, startY].GetComponent<Chunks>().index = 18;
                    }
                    for (int j = 0; j < 8; j++)
                        if (startX + i + dir[j][0] >= 0 && startX + i + dir[j][0] <= 9 && startY + dir[j][1] >= 0 && startY + dir[j][1] <= 9)
                            if (field[startX + i + dir[j][0], startY + dir[j][1]].GetComponent<Chunks>().index == 0)
                                field[startX + i + dir[j][0], startY + dir[j][1]].GetComponent<Chunks>().index = 9;
                }
                if (type == "v")
                {
                    if (i == 0)
                    {
                        field[startX, startY + i].GetComponent<Chunks>().index = 13;
                    }
                    else if (i == 1)
                    {
                        if (i == size)
                            field[startX, startY + i].GetComponent<Chunks>().index = 19;
                        else field[startX, startY + i].GetComponent<Chunks>().index = 15;
                    }
                    else if (i == 2)
                    {
                        if (i == size)
                            field[startX, startY + i].GetComponent<Chunks>().index = 19;
                        else field[startX, startY + i].GetComponent<Chunks>().index = 17;
                    }
                    else if (i == 3)
                    {
                        field[startX, startY + i].GetComponent<Chunks>().index = 19;
                    }
                    for (int j = 0; j < 8; j++)
                        if (startX + dir[j][0] >= 0 && startX + dir[j][0] <= 9 && startY + i + dir[j][1] >= 0 && startY + i + dir[j][1] <= 9)
                            if (field[startX + dir[j][0], startY + i + dir[j][1]].GetComponent<Chunks>().index == 0)
                                field[startX + dir[j][0], startY + i + dir[j][1]].GetComponent<Chunks>().index = 9;
                }
            }
        }
    }

    public bool DespawnShip(int x, int y, ref int size, ref bool isVert)
    {
        if (field[x, y].GetComponent<Chunks>().index == 0)
        {
            return false;
        }
        DespawnShipRecursive(0, x, y, ref size, ref isVert);
        shipsCount[size - 1]++;
        int revovedNum = shipsArray[x, y];
        int xstart = x - size < 0 ? 0 : x - size;
        int ystart = y - size < 0 ? 0 : y - size;
        int xend = x + size > 10 ? 10 : x + size;
        int yend = y + size > 10 ? 10 : y + size;
        int xEndBou = -1;
        int xStartBou = 11;
        int yEndBou = -1;
        int yStartBou = 11;
        for (int i = xstart; i < xend; i++)
        {
            for (int j = ystart; j < yend; j++)
            {
                if (shipsArray[i, j] == revovedNum) 
                { 
                    shipsArray[i, j] = 0;
                    fieldArray[i, j] = 0;
                    if (xStartBou > i - 1) { xStartBou = i - 1; }
                    if (yStartBou > j - 1) { yStartBou = j - 1; }
                    if (xEndBou < i + 1) { xEndBou = i + 1; }
                    if (yEndBou < j + 1) { yEndBou = j + 1; }
                };
            }
        }
        xEndBou = xEndBou + 1 > 10 ? 10 : xEndBou + 1;
        yEndBou = yEndBou + 1 > 10 ? 10 : yEndBou + 1;
        for (int i = xStartBou < 0 ? 0 : xStartBou; i < xEndBou; i++)
        {
            for (int j = yStartBou < 0 ? 0 : yStartBou; j < yEndBou; j++)
            {
                xstart = i - 1 < 0 ? 0 : i - 1;
                ystart = j - 1 < 0 ? 0 : j - 1;
                xend = i + 2 > 10 ? 10 : i + 2;
                yend = j + 2 > 10 ? 10 : j + 2;
                fieldArray[i, j] = Update9x9(shipsArray, xstart, ystart, xend, yend);
            }
        }
        return true;
    }

    private int Update9x9(int[,] array, int sx, int sy, int ex, int ey)
    {
        int num = 0;
        for (int i = sx; i < ex; i++)
        {
            for (int j = sy; j < ey; j++)
            {
                if (array[i, j] > 0) { return 1; }
            }
        }
        return num;
    }


    private void DespawnShipRecursive(int direction, int x, int y, ref int size, ref bool isVert)
    {

        if(x<0 || x>9 || y < 0 || y> 9)
        {
            return;
        }
        Chunks chunk = field[x, y].GetComponent<Chunks>();
        if (chunk.index == 0)
        {
            return;
        }

        if (direction == 1 || direction == 3) { isVert = false; }
        if (direction == 2 || direction == 4) { isVert = true; }
        size++;
        chunk.index = 0;

        switch (direction)
        {
            case 0:
                DespawnShipRecursive(1, x-1, y, ref size, ref isVert);
                DespawnShipRecursive(2, x, y-1, ref size, ref isVert);
                DespawnShipRecursive(3, x+1, y, ref size, ref isVert);
                DespawnShipRecursive(4, x, y+1, ref size, ref isVert);
                break;
            case 1:
                DespawnShipRecursive(1, x - 1, y, ref size, ref isVert);
                break;
            case 2:
                DespawnShipRecursive(2, x, y - 1, ref size, ref isVert);
                break;
            case 3:
                DespawnShipRecursive(3, x + 1, y, ref size, ref isVert);
                break;
            case 4:
                DespawnShipRecursive(4, x, y + 1, ref size, ref isVert);
                break;
            default:
                break;
        }
        return;
    }

    public void ShowBotShips()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (shipsArray[i, j] > 0 && field[i, j].GetComponent<Chunks>().index == 0)
                {
                    field[i, j].GetComponent<Chunks>().index = ShipPartIndex(i, j);
                    field[i, j].GetComponent<Chunks>().BlinkSprites();
                }
            }
        }
    }

    private int ShipPartIndex(int x, int y)
    {
        int size = 0;
        bool isVert = false;
        FindSizeDirection(0, x, y, ref size, ref isVert);
        if (size == 1)
        {
            return 1;
        }
        else if (isVert)
        {
            switch (size)
            {
                case 2:
                    if (y - 1 < 0 || shipsArray[x, y - 1] == 0)
                    {
                        return 2;
                    }
                    else
                    {
                        return 8;
                    }
                    break;
                case 3:
                    if (y - 1 < 0 || shipsArray[x, y - 1] == 0)
                    {
                        return 2;
                    }
                    else if (y + 1 > 9 || shipsArray[x, y + 1] == 0)
                    {
                        return 8;
                    }
                    else
                    {
                        return 4;
                    }
                    break;
                default:
                    if (y - 1 < 0 || shipsArray[x, y - 1] == 0)
                    {
                        return 2;
                    }
                    else if (y + 1 > 9 || shipsArray[x, y + 1] == 0)
                    {
                        return 8;
                    }
                    else if (y + 1 <= 9 && shipsArray[x, y + 1] != 0)
                    {
                        if (y + 2 > 9 || shipsArray[x, y + 2] == 0)
                        {
                            return 6;
                        }
                        else
                        {
                            return 4;
                        }
                    }
                    break;
            }
        }
        else
        {
            switch (size)
            {
                case 2:
                    if (x - 1 < 0 || shipsArray[x - 1, y] == 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return 7;
                    }
                    break;
                case 3:
                    if (x - 1 < 0 || shipsArray[x - 1, y] == 0)
                    {
                        return 1;
                    }
                    else if (x + 1 > 9 || shipsArray[x + 1, y] == 0)
                    {
                        return 7;
                    }
                    else
                    {
                        return 3;
                    }
                    break;
                default:
                    if (x - 1 < 0 || shipsArray[x - 1, y] == 0)
                    {
                        return 1;
                    }
                    else if (x + 1 > 9 || shipsArray[x + 1, y] == 0)
                    {
                        return 7;
                    }
                    else if (x + 1 <= 9 && shipsArray[x + 1, y] != 0)
                    {
                        if (x + 2 > 9 || shipsArray[x + 2, y] == 0)
                        {
                            return 5;
                        }
                        else
                        {
                            return 3;
                        }
                    }
                    break;
            }
        }
        return 10;
    }

    public void FindSizeDirection(int direction, int x, int y, ref int size, ref bool isVert)
    {

        if (x < 0 || x > 9 || y < 0 || y > 9)
        {
            return;
        }
        if (shipsArray[x, y] == 0)
        {
            return;
        }

        if (direction == 1 || direction == 3) { isVert = false; }
        if (direction == 2 || direction == 4) { isVert = true; }
        size++;

        switch (direction)
        {
            case 0:
                FindSizeDirection(1, x - 1, y, ref size, ref isVert);
                FindSizeDirection(2, x, y - 1, ref size, ref isVert);
                FindSizeDirection(3, x + 1, y, ref size, ref isVert);
                FindSizeDirection(4, x, y + 1, ref size, ref isVert);
                break;
            case 1:
                FindSizeDirection(1, x - 1, y, ref size, ref isVert);
                break;
            case 2:
                FindSizeDirection(2, x, y - 1, ref size, ref isVert);
                break;
            case 3:
                FindSizeDirection(3, x + 1, y, ref size, ref isVert);
                break;
            case 4:
                FindSizeDirection(4, x, y + 1, ref size, ref isVert);
                break;
            default:
                break;
        }
        return;
    }

    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
