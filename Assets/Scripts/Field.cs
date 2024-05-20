using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Random = UnityEngine.Random;

public class Field : MonoBehaviour
{
    public GameObject bullet;
    public GameObject water;
    public GameObject fire;
    public GameObject explosion;
    public GameObject sonar;

    public List<GameObject> squaresOnFire;
    public int currOnFire = 0;

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

    private bool[,] isHit;
    private bool Starts = false;
    private float timer = 0f;
    public int blinkCount = 0;
    int blinkIndex = 0;
    float blinkdelay = 0;
    private ArrayList blinkingShipsArray;
    private ArrayList IndicatedTiles;

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

        isHit = new bool[10, 10];
        shipsCount = new int[] { 4, 3, 2, 1 };
        letters = new GameObject[fieldLength];
        numbers = new GameObject[fieldLength];
        field = new GameObject[fieldLength, fieldLength];
        squaresOnFire = new List<GameObject>();
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


    public bool CheckIfShotPossible(GameObject coordinates)
    {
        int x = int.Parse(coordinates.name.Split(' ')[0]);
        int y = int.Parse(coordinates.name.Split(' ')[1]);
        return field[x, y].GetComponent<Chunks>().index == 0 && shipsArray[x, y] >= 0;
    }


    public void ChangeSprite(GameObject coordinates)
    {
        int x = int.Parse(coordinates.name.Split(' ')[0]);
        int y = int.Parse(coordinates.name.Split(' ')[1]);

        
        if (field[x, y].GetComponent<Chunks>().index == 20)
        {
            field[x, y].GetComponent<Chunks>().index = 0;
        }
           
        else if (field[x, y].GetComponent<Chunks>().index == 0 && shipsArray[x, y] >= 0)
        {
            field[x, y].GetComponent<Chunks>().index = 20;
        }   
    }


    public bool ChangeSpriteSpecialAb(GameObject coordinates, ShootingManager.ChosenAbility ability)
    {
        int x = int.Parse(coordinates.name.Split(' ')[0]);
        int y = int.Parse(coordinates.name.Split(' ')[1]);

        int count = 0;
        if (ability == ShootingManager.ChosenAbility.x3 || ability == ShootingManager.ChosenAbility.Sonar)
        {
            count = X3TileChanger(x, y, count);
        }
        else if(ability == ShootingManager.ChosenAbility.Vertical)
        {
            count = VerHozTileChanger(x, y, false, count);
        }
        else if (ability == ShootingManager.ChosenAbility.Horizontal)
        {
            count = VerHozTileChanger(x, y, true, count);
        }
        return count > 0;
    }


    private int VerHozTileChanger(int x, int y, bool isHoz, int count)
    {
        
        int xOrig = x;
        int yOrig = y;
        
        for (int i = 0; i <= 9; i++)
        {
            x = isHoz ? i : x;
            y = isHoz ? y : i;
            
            if (field[x, y].GetComponent<Chunks>().index == 20)
            {
                field[x, y].GetComponent<Chunks>().index = 0;
                count++;
            }
            else if (shipsArray[x, y] >= 0 && field[x, y].GetComponent<Chunks>().index == 0)
            {
                
                field[x, y].GetComponent<Chunks>().index = 20;
                count++;
            }
        }
        return count;
    }


    private int X3TileChanger(int x, int y, int count)
    {
        int xStart = x - 1 < 0 ? 0 : x - 1;
        int xEnd = x + 1 > 9 ? 9 : x + 1;
        int yStart = y - 1 < 0 ? 0 : y - 1;
        int yEnd = y + 1 > 9 ? 9 : y + 1;

        for (int i = xStart; i <= xEnd; i++)
        {
            for (int j = yStart; j <= yEnd; j++)
            {
                
                if (field[i, j].GetComponent<Chunks>().index == 20 )
                {
                    field[i, j].GetComponent<Chunks>().index = 0;
                    count++;
                }

                else if (shipsArray[i, j] >= 0 && field[i, j].GetComponent<Chunks>().index == 0)
                {
                    
                    field[i, j].GetComponent<Chunks>().index = 20;
                    count++;
                }
            }
        }
        return count;
    }


    /// <summary>
    /// Returns the coordinates of detected ships around the specified coordinate and updates the vision.
    /// </summary>
    /// <param name="coord"></param>
    /// <param name="vision"></param>
    /// <returns></returns>
    public Stack<string> ReturnSonarResults(string coord, ref int[,] vision)
    {
        int x = int.Parse(coord.Split(' ')[0]);
        int y = int.Parse(coord.Split(' ')[1]);

        Stack<string> ships = new Stack<string>();
        for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 2 > 10 ? 10 : x + 2); i++)
        {
            for (int j = y - 1 < 0 ? 0 : y - 1; j < (y + 2 > 10 ? 10 : y + 2); j++)
            {
                // If ship is detected, add its coordinates to the stack
                if (shipsArray[i, j] > 0)
                {
                    ships.Push(i + " " + j);
                }
                // If no ship is detected, update vision
                else if(shipsArray[i, j] == 0)
                    vision[i, j] = 1;
            }
        }

        return ships;
    }
    

    /// <summary>
    /// Changes the appearance of tiles for the sonar effect animation.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="gameManager"></param>
    /// <param name="delay"></param>
    /// <param name="isPlayer"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public IEnumerator SonarTileChanger(int x, int y, GameManager gameManager, float delay, bool isPlayer, int count = 0)
    {
        
        if (count == 0)
        {
            gameManager.isAnimationDone = false;
            yield return new WaitForSeconds(delay);
        }

        for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 2 > 10 ? 10 : x + 2); i++)
        {
            for (int j = y - 1 < 0 ? 0 : y - 1; j < (y + 2 > 10 ? 10 : y + 2); j++)
            {
                // Change tile appearance based on ship detection and animation count
                if (count == 0)
                {
                    SpriteRenderer renderer = field[i, j].GetComponent<SpriteRenderer>();
                    if (renderer != null && shipsArray[i, j] > 0 && !isHit[i, j])
                    {
                        GameObject.Find("SonarSound").GetComponent<AudioSource>().Play();
                        if (isPlayer)
                            renderer.color = new Color(0.4125f, 0.7525f, 0.3845f, 1); // Darken the sprite
                        else
                        {
                            renderer.color = new Color(0.7f, 0.3f, 0.3f, 1);
                        }
                    }
                }

                else
                {
                    SpriteRenderer renderer = field[i, j].GetComponent<SpriteRenderer>();
                    if (renderer != null && shipsArray[i, j] > 0 && !isHit[i, j])
                    {
                        renderer.color = new Color(1f, 1f, 1f, 1); // Darken the sprite
                    }
                }
            }
        }
        
        // Initiate recursive call for additional animation frames
        if (count < 1)
        {
            yield return new WaitForSeconds(1.5f);
            gameManager.isAnimationDone = true;
            StartCoroutine(SonarTileChanger(x, y, gameManager, 0, isPlayer, count + 1));
        }
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

    public int[,] GetBoardVision()
    {
        int[,] boardArray = new int[10, 10];
        for (int i = 0; i < fieldLength; i++)
        {
            for (int j = 0; j < fieldLength; j++)
            {
                if(field[i, j].GetComponent<Chunks>().index == 9)
                {
                    boardArray[i, j] = 1;
                }
            }
        }
        return boardArray;
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
    public IEnumerator MoveBulletWithDelay(Vector3 startPos, int x1, int y1)
    {
        FindObjectOfType<GameManager>().isAnimationDone = false;
        GameObject bulletInstance = Instantiate(bullet, new Vector3(startPos.x + 1 + x1, startPos.y + 2, 0), Quaternion.identity);
        bulletInstance.transform.Rotate(0, 0, -90);
        while (bulletInstance.transform.position.y > startPos.y - 1 - y1)
        {
            bulletInstance.transform.Translate(Vector3.right * 0.1f);
            yield return null;
        }
        Destroy(bulletInstance);
        if (shipsArray[x1, y1] == 0)
        {
            Renderer waterRenderer = water.GetComponent<Renderer>();
            waterRenderer.sortingOrder = 30;
            water.transform.localScale = new Vector3(1f, 1f, 1f);
            //AudioSource.PlayClipAtPoint(waterSound, new Vector3(startPos.x + 1 + x1, startPos.y - 1 - y1, 0), 1f);
            GameObject waterSound = GameObject.Find("WaterSound");
            waterSound.GetComponent<AudioSource>().Play();
            GameObject waterInstance = Instantiate(water, new Vector3(startPos.x + 1 + x1, startPos.y - 1 - y1, 0), Quaternion.identity);
            Destroy(waterInstance, 1.35f);
            field[x1, y1].GetComponent<Chunks>().index = 9;
        }
        else
        {
            Renderer fireRenderer = fire.GetComponent<Renderer>();
            fireRenderer.sortingOrder = 30;
            //field[x1, y1].GetComponent<Chunks>().index = 10;
            fire.transform.localScale = new Vector3(0.5f, 0.35f, 1f);
            if (IsAllShipDestroyed(shipsArray[x1, y1], x1, y1))
            {
                //AudioSource.PlayClipAtPoint(explosionSound, new Vector3(startPos.x + 1 + x1, startPos.y - 1 - y1, 0), 1f);
                GameObject explosionSound = GameObject.Find("ExplosionSound");
                explosionSound.GetComponent<AudioSource>().Play();
            }
            else
            {
                //AudioSource.PlayClipAtPoint(fireSound, new Vector3(startPos.x + 1 + x1, startPos.y - 1 - y1, 0), 1f);
                GameObject fireSound = GameObject.Find("FireSound");
                fireSound.GetComponent<AudioSource>().Play();
            } 
            squaresOnFire.Add(Instantiate(fire, new Vector3(startPos.x + 1 + x1, startPos.y - 1 - y1, 0), Quaternion.identity));
            if (IsAllShipDestroyed(shipsArray[x1, y1], x1, y1))
                DestroyAllShip(x1, y1, shipsArray[x1, y1]);
            shipsArray[x1, y1] *= -1;
        }

        FindObjectOfType<GameManager>().isAnimationDone = true;
    }
    public DestroyResult Destroy(int x1, int y1)
     {
        Vector3 startPos = transform.position;
        int[] illegalIndexes = new int[] { 9, 10, 12, 13, 14, 15, 16, 17, 18, 19 };
        if (illegalIndexes.Contains(field[x1, y1].GetComponent<Chunks>().index))
            return DestroyResult.IllegalMove;
        if (shipsArray[x1, y1] < 0)
            return DestroyResult.IllegalMove;

        else
        {
            isHit[x1, y1] = true;
            Renderer bulletRenderer = bullet.GetComponent<Renderer>();
            bulletRenderer.sortingOrder = 40;
            bullet.transform.localScale = new Vector3(1f, 1f, 1f);
            bullet.transform.Rotate(0f, 0f, 90f);
            GameObject bulletSound = GameObject.Find("BulletSound");
            bulletSound.GetComponent<AudioSource>().Play();
            //AudioSource.PlayClipAtPoint(bulletSound, new Vector3(startPos.x + 1 + x1, startPos.y - 1 - y1, 0), 1f);
            StartCoroutine(MoveBulletWithDelay(startPos, x1, y1));
            if (shipsArray[x1, y1] == 0)
            {
                return DestroyResult.Failure;
            }
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
        Vector3 startPos = transform.position;
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
            for (int ii = 0; ii < squaresOnFire.Count; ii++)
                if (squaresOnFire[ii].transform.position.x == startPos.x + 1 + startX && squaresOnFire[ii].transform.position.y == startPos.y - 1 - startY)
                    squaresOnFire[ii].GetComponent<Renderer>().enabled = false;

            Renderer explosionRenderer = explosion.GetComponent<Renderer>();
            explosionRenderer.sortingOrder = 30;
            explosion.transform.localScale = new Vector3(1f, 1f, 1f);
            GameObject explosionInstance = Instantiate(explosion, new Vector3(startPos.x + 1 + startX, startPos.y - 1 - startY, 0), Quaternion.identity);
            Destroy(explosionInstance, 1f);

            field[x, y].GetComponent<Chunks>().index = 12;
            shipsCount[0]--;
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
                    for (int ii = 0; ii < squaresOnFire.Count; ii++)
                        if (squaresOnFire[ii].transform.position.x == startPos.x + 1 + startX + i && squaresOnFire[ii].transform.position.y == startPos.y - 1 - startY)
                            squaresOnFire[ii].GetComponent<Renderer>().enabled = false;

                    Renderer explosionRenderer = explosion.GetComponent<Renderer>();
                    explosionRenderer.sortingOrder = 30;
                    explosion.transform.localScale = new Vector3(1f, 1f, 1f);
                    GameObject explosionInstance = Instantiate(explosion, new Vector3(startPos.x + 1 + startX + i, startPos.y - 1 - startY, 0), Quaternion.identity);
                    Destroy(explosionInstance, 1f);

                    if (i == 0)
                    {
                        field[startX + i, startY].GetComponent<Chunks>().index = 12;
                    }
                    else if (i == 1)
                    {
                        if (i == size)
                        {
                            field[startX + i, startY].GetComponent<Chunks>().index = 18;
                            shipsCount[1]--;
                        }
                        else field[startX + i, startY].GetComponent<Chunks>().index = 14;
                    }
                    else if (i == 2)
                    {
                        if (i == size)
                        {
                            field[startX + i, startY].GetComponent<Chunks>().index = 18;
                            shipsCount[2]--;
                        }
                        else field[startX + i, startY].GetComponent<Chunks>().index = 16;
                    }
                    else if (i == 3)
                    {
                        field[startX + i, startY].GetComponent<Chunks>().index = 18;
                        shipsCount[3]--;
                    }
                    for (int j = 0; j < 8; j++)
                        if (startX + i + dir[j][0] >= 0 && startX + i + dir[j][0] <= 9 && startY + dir[j][1] >= 0 && startY + dir[j][1] <= 9)
                            if (field[startX + i + dir[j][0], startY + dir[j][1]].GetComponent<Chunks>().index == 0)
                                field[startX + i + dir[j][0], startY + dir[j][1]].GetComponent<Chunks>().index = 9;
                }
                if (type == "v")
                {
                    for (int ii = 0; ii < squaresOnFire.Count; ii++)
                        if (squaresOnFire[ii].transform.position.x == startPos.x + 1 + startX && squaresOnFire[ii].transform.position.y == startPos.y - 1 - (startY + i))
                            squaresOnFire[ii].GetComponent<Renderer>().enabled = false;

                    Renderer explosionRenderer = explosion.GetComponent<Renderer>();
                    explosionRenderer.sortingOrder = 30;
                    explosion.transform.localScale = new Vector3(1f, 1f, 1f);
                    GameObject explosionInstance = Instantiate(explosion, new Vector3(startPos.x + 1 + startX, startPos.y - 1 - (startY + i), 0), Quaternion.identity);
                    Destroy(explosionInstance, 1f);

                    if (i == 0)
                    {
                        field[startX, startY + i].GetComponent<Chunks>().index = 13;
                    }
                    else if (i == 1)
                    {
                        if (i == size)
                        {
                            field[startX, startY + i].GetComponent<Chunks>().index = 19;
                            shipsCount[1]--;
                        }
                        else field[startX, startY + i].GetComponent<Chunks>().index = 15;
                    }
                    else if (i == 2)
                    {
                        if (i == size)
                        {
                            field[startX, startY + i].GetComponent<Chunks>().index = 19;
                            shipsCount[2]--;
                        }
                        else field[startX, startY + i].GetComponent<Chunks>().index = 17;
                    }
                    else if (i == 3)
                    {
                        field[startX, startY + i].GetComponent<Chunks>().index = 19;
                        shipsCount[3]--;
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
        if (Starts) { return; }
        blinkingShipsArray = new ArrayList();
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (shipsArray[i, j] > 0 && field[i, j].GetComponent<Chunks>().index == 0)
                {
                    int[] newArr = new int[3];
                    newArr[0] = i;
                    newArr[1] = j;
                    newArr[2] = ShipPartIndex(i, j);
                    blinkingShipsArray.Add(newArr);
                    field[i, j].GetComponent<Chunks>().ChangeClour();
                }
            }
        }
        blinkdelay = 0.75f;
        blinkCount = 500;
        blinkIndex = 0;
        Starts = true;
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

    private void Blink(int index)
    {
        //Debug.Log(blinkingShipsArray.Count);
        foreach (int[] part in blinkingShipsArray)
        {
            //Debug.Log(field[part[0], part[1]].GetComponent<Chunks>().index == 0);
            if (field[part[0], part[1]].GetComponent<Chunks>().index == index)
            {
                field[part[0], part[1]].GetComponent<Chunks>().index = part[2];
            }
            else
            {
                field[part[0], part[1]].GetComponent<Chunks>().index = index;
                //Debug.Log(field[part[0], part[1]].GetComponent<Chunks>().index);
            }
        }
    }

    public bool CheckForShips(int x, int y, int size, bool isVertical)
    {
        if (isVertical)
        {
            for (int j = y; j < y + size; j++)
            {
                if (fieldArray[x, j] > 0)
                {
                    return false;
                }
            }
        }
        else
        {
            for (int j = x; j < x + size; j++)
            {
                if (fieldArray[j, y] > 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Indicate(int x, int y, int size, bool isVertical, bool isWrongPlace)
    {
        if (isVertical)
        {
            for (int j = y; j < y + size; j++)
            {
                int[] newArr = new int[3];
                newArr[0] = x;
                newArr[1] = j;
                newArr[2] = field[x, j].GetComponent<Chunks>().index;
                field[x, j].GetComponent<Chunks>().index = isWrongPlace ? 22 : 21;
                IndicatedTiles.Add(newArr);
            }
        }
        else
        {
            for (int j = x; j < x + size; j++)
            {
                int[] newArr = new int[3];
                newArr[0] = j;
                newArr[1] = y;
                newArr[2] = field[j, y].GetComponent<Chunks>().index;
                field[j, y].GetComponent<Chunks>().index = isWrongPlace ? 22 : 21;
                IndicatedTiles.Add(newArr);
            }
        }
    }

    public void IndicateWhenPutOnWrongPlace(int x, int y, int size, bool isRotated)
    {
        if (Starts) { return; }
        blinkingShipsArray = new ArrayList();
        if (isRotated)
        {
            for (int i = y; i <= y + size; i++)
            {
                int[] newArr = new int[3];
                newArr[0] = x;
                newArr[1] = i;
                newArr[2] = field[x, i].GetComponent<Chunks>().index;
                blinkingShipsArray.Add(newArr);
            }
        }
        else
        {
            for (int i = x; i <= x + size; i++)
            {
                int[] newArr = new int[3];
                newArr[0] = i;
                newArr[1] = y;
                newArr[2] = field[i, y].GetComponent<Chunks>().index;
                blinkingShipsArray.Add(newArr);
            }
        }
        blinkdelay = 0.35f;
        blinkCount = 4;
        blinkIndex = 23;
        Starts = true;
    }

    public void StopIndication()
    {
        foreach (int[] part in IndicatedTiles)
        {
            field[part[0], part[1]].GetComponent<Chunks>().index = part[2];
        }
        IndicatedTiles.Clear();
    }

    /// <summary>
    /// plays sonar animation when sonar is used in chosen field position
    /// </summary>
    /// <param name="x">x coordinate of field</param>
    /// <param name="y">y coordinate of field</param>
    public void PlaySonarAnimation(int x, int y)
    {
        Vector3 startPos = transform.position;
        Renderer sonarRenderer = sonar.GetComponent<Renderer>();
        sonarRenderer.sortingOrder = 50;
        sonar.transform.localScale = new Vector3(1f, 1f, 1f);
        GameObject.Find("ScannerSound").GetComponent<AudioSource>().Play();
        GameObject sonarInstance = Instantiate(sonar, new Vector3(startPos.x + x + 1, startPos.y - y - 1, 0), Quaternion.identity);
        Destroy(sonarInstance, 2.3f);
    }

    public int[] ShipsCount()
    {
        return shipsCount;
    }

    void Start()
    {
        IndicatedTiles = new ArrayList();
    }


    // Update is called once per frame
    void Update()
    {
        if (Starts)
        {
            timer += Time.deltaTime;
            if (timer > blinkdelay)
            {
                //Debug.Log("GG");
                Blink(blinkIndex);
                timer = 0f;
                blinkCount--;
                if (blinkCount == 0) Starts = false;
            }
        }
    }
}
