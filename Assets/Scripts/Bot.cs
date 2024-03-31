using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;

public class Bot : MonoBehaviour, IKillable
{
    public Field botFieldPrefab;

    private Field field;

    GameManager gameManager;

    private int remainingBoats = 20;

    private double[,] heatMap;

    private int[,] botVision;

    public bool hit { get; set; }

    private string lastHit;

    private string lastSuccessfulHit;

    private Dictionary<int, int> shipsRemaining = new Dictionary<int, int>();

    private enum BotState
    {
        Searching,
        Atacking,
    }

    private BotState state;

    private enum HitShipState
    {
        Unknown,
        Horizontal,
        Vertical,
    }

    private HitShipState shipState;


    private enum GameState
    {
        Early,
        Mid_Late,
        OnlyOneTileLeft,
    }

    private GameState gameState;

    private int shipsPossiblePlacementCount = 19;

    void Start()
    {

        field = Instantiate(botFieldPrefab, transform.position, Quaternion.identity);
        field.transform.SetParent(transform);
        field.CreateField();
        field.SpawnAllShips();
        field.MaskField();
        heatMap = new double[10,10];
        botVision = new int[10,10];
        state = BotState.Searching;
        shipState = HitShipState.Unknown;
        gameState = GameState.Early;
        hit = false;
        lastHit = "";
        lastSuccessfulHit = "";
        gameManager = GetComponentInParent<GameManager>();

        shipsRemaining.Add(1, 4);
        shipsRemaining.Add(2, 3);
        shipsRemaining.Add(3, 2);
        shipsRemaining.Add(4, 1);
    }

    void RecalculateHeatMap()
    {
        int countHitCells = 0;
        //Debug.ClearDeveloperConsole();
        for (int i = 0; i < heatMap.GetLength(0); i++)  
        {
            for (int j = 0; j < heatMap.GetLength(1); j++)
            {
                double heatValue = CalculatePercent(i, j);
                heatMap[i, j] = heatValue;

                if(heatValue == 0)
                    countHitCells++;
            }
        }
        if(countHitCells > 25)
        {
            if(shipsPossiblePlacementCount == 1)
                gameState = GameState.OnlyOneTileLeft;
            else gameState = GameState.Mid_Late;
        }
    }

    double CalculatePercent(int x, int y)
    {
        if (botVision[x, y] != 0)
            return 0;

        double count = 0;
        foreach (KeyValuePair<int, int> ship in shipsRemaining)
        {
            count += CalculateOneShipPlacementPos(ship.Key -1, x, y);
           // Debug.Log("---------------------------------------------------" + count);
        }
        return count / shipsPossiblePlacementCount;
    }

    int CalculateOneShipPlacementPos(int size, int x, int y)
    {
        int startX = Math.Max(0, x - size);
        int endX = Math.Min(9, x + size);
        int startY = Math.Max(0, y - size);
        int endY = Math.Min(9, y + size);
    //    Debug.Log("x: " + x + "   y: " + y + "  ship size: " + (size + 1) + " " +
    //        " startX: " + startX + "  endX: " + endX + "  startY: " + startY + "  endY: " + endY);
        if (size != 0)
            return  CalculateOneShipHorizontal(startX, endX, y, size) + CalculateOneShipVertical(startY, endY, x, size);
        else return CalculateOneShipHorizontal(startX, endX, y, size);
    }


    int CalculateOneShipHorizontal(int start, int end, int yAxis, int size)
    {
        int count = 0;
        for (int j = start; j + size <= end; j++)
        {
            if(IsPlaceValid(j, j + size, true, yAxis))
                count++;
        }
        return count;
    }


    int CalculateOneShipVertical(int start, int end, int xAxis, int size)
    {
        int count = 0;
        for (int j = start; j + size <= end; j++)
        {
            if (IsPlaceValid(j, j + size, false, xAxis))
                count++;
        }
        return count;
    }


    bool IsPlaceValid(int start, int end, bool horizontal, int axis)
    {
        for (int i = start; i <= end; i++)
        {
       //     Debug.Log("isHorizontal: " + horizontal + " i: " + i + " botVision: " + (horizontal ? botVision[i, axis] : botVision[axis, i]));
            if (horizontal && botVision[i, axis] != 0)
                return false;
            else if (!horizontal && botVision[axis, i] != 0)
                return false;
        }
        return true;
    }


    string FindCoordinateWithPotentialTarget(double target = 0.90)
    {
        List<string> coordinates = new List<string>();
        for (int i = 0; i < heatMap.GetLength(0); i++)
        {
            for (int j = 0; j < heatMap.GetLength(1); j++)
            {
                if (heatMap[i, j] >= (gameState == GameState.Early ? 0.50 : target))
                {
                    coordinates.Add(i + " " + j);
                }
            }
        }
        // lastHit = coordinates;
        if (coordinates.Count == 0)
            return FindCoordinateWithPotentialTarget(target - 0.05);
        return GetRandomCoordinate(coordinates);
    }


    string GetRandomCoordinate(List<string> coordinates)
    {
        if (lastHit.Length < 1)
            return coordinates[Random.Range(0, coordinates.Count)];

        int start = 0;
        int end = 0;

        if(gameState == GameState.Early)
        {
            start = 5;
            end = 10;
        }
        else if (gameState == GameState.Mid_Late)
        {
            start = 2;
            end = 6;
        }
        else if (gameState == GameState.OnlyOneTileLeft)
        {
            start = 1;
            end = 10;
        }

        int count = 0;
        while (count < 50)
        {
            int randomCoordinate = Random.Range(0, coordinates.Count);
            if (Math.Abs(int.Parse(coordinates[randomCoordinate].Split(' ')[0]) - int.Parse(lastHit.Split(' ')[0])) +
                Math.Abs(int.Parse(coordinates[randomCoordinate].Split(' ')[1]) - int.Parse(lastHit.Split(' ')[1])) >= start &&
                Math.Abs(int.Parse(coordinates[randomCoordinate].Split(' ')[0]) - int.Parse(lastHit.Split(' ')[0])) +
                Math.Abs(int.Parse(coordinates[randomCoordinate].Split(' ')[1]) - int.Parse(lastHit.Split(' ')[1])) <= end)
                return coordinates[randomCoordinate];
            count++;
        }
        return coordinates[Random.Range(0, coordinates.Count)];
    }


    void TryIdentifyShipState(int xOrig, int yOrig)
    {
        if (lastSuccessfulHit.Length < 1)
            return;

        for (int i = -1; i <= 1; i += 2)
        {
            int y = yOrig + i;
            if (y < 0 || y >= 10) continue;

            if (botVision[xOrig, y] == -1)
            {
                shipState = HitShipState.Vertical;
                return; 
            }
        }

        for (int i = -1; i <= 1; i += 2)
        {
            int x = xOrig + i;
            if (x < 0 || x >= 10) continue;

            if (botVision[x, yOrig] == -1)
            {
                shipState = HitShipState.Horizontal;
                return; 
            }
        }
    }


    string CaseHelperPlus(int xOrig, int yOrig, bool isHorizontal)
    {
        for (int i = 1; i <= 3; i++)
        {
            if (!isHorizontal && yOrig + i < 10)
            {
                if (botVision[xOrig, yOrig + i] == 0)
                    return xOrig + " " + (yOrig + i);
                else if (botVision[xOrig, yOrig + i] == 1)
                    break;
            }

            else if (isHorizontal && xOrig + i < 10)
            {
                if (botVision[xOrig + i, yOrig] == 0)
                    return (xOrig + i) + " " + yOrig;
                else if (botVision[xOrig + i, yOrig] == 1)
                    break;
            }
        }
        return "";
    }

    string CaseHelperMinus(int xOrig, int yOrig, bool isHorizontal)
    {
        for (int i = 1; i <= 3; i++)
        {
            if (!isHorizontal && yOrig - i >= 0)
            {
                if (botVision[xOrig, yOrig - i] == 0)
                    return xOrig + " " + (yOrig - i);
                else if (botVision[xOrig, yOrig - i] == 1)
                    break;
            }
            else if (isHorizontal && xOrig - i >= 0)
            {
                if (botVision[xOrig - i, yOrig] == 0)
                    return xOrig - i + " " + yOrig;
                else if (botVision[xOrig - i, yOrig] == 1)
                    break;
            }
        }
        return "";
    }


    string CaseVertical(int xOrig, int yOrig)
    {
        string coordinates = "";
        if (Random.Range(0, 2) == 1)
            coordinates = CaseHelperPlus(xOrig, yOrig, false);

        if(coordinates.Length < 1) 
            coordinates = CaseHelperMinus(xOrig, yOrig, false);

        if(coordinates.Length < 1)
            coordinates = CaseHelperPlus(xOrig, yOrig, false);

        return coordinates;
    }


    string CaseHorizontal(int xOrig, int yOrig)
    {
        string coordinates = "";
        if (Random.Range(0, 2) == 1)
            coordinates = CaseHelperPlus(xOrig, yOrig, true);

        if (coordinates.Length < 1)
            coordinates = CaseHelperMinus(xOrig, yOrig, true);

        if(coordinates.Length < 1)
            coordinates = CaseHelperPlus(xOrig, yOrig, true);
        return coordinates;
    }


    string CaseUnknown(int xOrig, int yOrig, int attempt = 0)
    {
        
        bool isHorizontal = Random.Range(0, 2) == 0 ? true : false;
        string coordinates = "";

        if (isHorizontal)
        {
            if(Random.Range(0, 2) == 0 && xOrig + 1 < 10 && botVision[xOrig + 1, yOrig] == 0)
            {
                coordinates = xOrig + 1 + " " + yOrig;
            }
            if(coordinates.Length < 1 && xOrig - 1 >= 0 && botVision[xOrig - 1, yOrig] == 0)
            {
                coordinates = xOrig - 1 + " " + yOrig;
            }
        }
        else
        {
            if(Random.Range(0, 2) == 0 && yOrig + 1 < 10 && botVision[xOrig, yOrig + 1] == 0)
                coordinates = xOrig + " " + (yOrig + 1);
            if(coordinates.Length < 1 && yOrig - 1 >= 0 && botVision[xOrig, yOrig - 1] == 0)
                coordinates = xOrig + " " + (yOrig - 1);
        }
        if(coordinates.Length < 1 && attempt < 25)
            coordinates = CaseUnknown(xOrig, yOrig, attempt + 1);
        return coordinates;
    }

    string Attack()
    {
        int xOrig = int.Parse(lastSuccessfulHit.Split(' ')[0]);
        int yOrig = int.Parse(lastSuccessfulHit.Split(' ')[1]);

        string coordinates = "";

        if (shipState == HitShipState.Unknown)
            TryIdentifyShipState(xOrig, yOrig);

        switch (shipState)
        {
            case HitShipState.Unknown:
                coordinates = CaseUnknown(xOrig, yOrig);
                break;
            case HitShipState.Horizontal:
                coordinates = CaseHorizontal(xOrig, yOrig);
                break;
            case HitShipState.Vertical:
                coordinates = CaseVertical(xOrig, yOrig);
                break;
        }
        if(coordinates.Length < 1)
        {
            CheckWhichShipIsDestroyed(xOrig, yOrig);
            lastSuccessfulHit = "";
            shipState = HitShipState.Unknown;
            state = BotState.Searching;
        }
        return coordinates;
    }


    public string ApplyShot()
    {
        gameManager.UpdateBotVision();
        RecalculateHeatMap();
        if (hit)
        {
            lastSuccessfulHit = lastHit;
            botVision[int.Parse(lastSuccessfulHit.Split(' ')[0]), int.Parse(lastSuccessfulHit.Split(' ')[1])] = -1;
            state = BotState.Atacking;
            hit = false;
        }
        
        string coordinates = "";
        switch (state)
        {
            case BotState.Atacking:
                coordinates = Attack();
                break;
            case BotState.Searching:
                coordinates = FindCoordinateWithPotentialTarget();
                break;
              
        }
        if(coordinates.Length < 1)
        {
            Debug.LogError(lastHit + "   Last Successful hit "+ lastSuccessfulHit);
            coordinates = ApplyShot();
        }
                
        lastHit = coordinates;
        return coordinates;
        
        
       // int x = Random.Range(0, 10);
       // int y = Random.Range(0, 10);
       // return x + " " + y;
    }


    void CheckWhichShipIsDestroyed(int x, int y)
    {
        switch (shipState)
        {
            case HitShipState.Unknown:
                CheckIfUnknownD(x, y);
                break;
            case HitShipState.Horizontal: 
                CheckIfHorizontalD(x, y);
                break;
            case HitShipState.Vertical:
                CheckIfVerticalD(x, y);
                break;
        }
    }


    void CheckIfVerticalD(int x, int y)
    {
        int yTop = y;
        int yButtom = y;
        bool checkButtom = true;
        bool checkTop = true;

        int size = 1;
        while (checkButtom || checkTop)
        {
            if (checkButtom)
            {
                if (yButtom + 1 > 9 || yButtom + 1 < 10 && botVision[x, yButtom + 1] == 1)
                    checkButtom = false;

                else
                {
                    size++;
                    yButtom += 1;
                }
                
            }
            if(checkTop)
            {
                if (yTop -1 < 0 || yTop - 1 >= 0 && botVision[x, yTop - 1] == 1)
                    checkTop = false;
                else
                {
                    size++;
                    yTop -= 1;
                }
               
            }
        }
        if (size == 4)
        {
            shipsRemaining[4] = shipsRemaining[4] - 1;
            if (shipsRemaining[4] == 0)
            {
                shipsRemaining.Remove(4);
                shipsPossiblePlacementCount -= 8;
            }
        }
        else if (size == 3)
        {
            shipsRemaining[3] = shipsRemaining[3] - 1;
            if (shipsRemaining[3] == 0)
            {
                shipsRemaining.Remove(3);
                shipsPossiblePlacementCount -= 6;
            }
        }
        else
        {
            shipsRemaining[2] = shipsRemaining[2] - 1;
            if (shipsRemaining[2] == 0)
            {
                shipsRemaining.Remove(2);
                shipsPossiblePlacementCount -= 4;
            }
        }
    }


    void CheckIfHorizontalD(int x, int y)
    {
        int xRight = x;
        int xLeft = x;
        bool checkRight = true;
        bool checkLeft = true;
        int size = 1;
        while (checkRight || checkLeft)
        {
            if(checkRight)
            {
                if (xRight + 1 > 9 || xRight + 1 < 10 && botVision[xRight + 1, y] == 1)
                    checkRight = false;
                else
                {
                    size++;
                    xRight += 1;
                }
                
            }
            else
            {
                if (xLeft - 1 < 0 || xLeft - 1 >= 0 && botVision[xLeft - 1, y] == 1)
                    checkLeft = false;
                else
                {
                    size++;
                    xLeft -= 1;
                }
                
            }
        }
        if(size == 4)
        {
            shipsRemaining[4] = shipsRemaining[4] - 1;
            if (shipsRemaining[4] == 0)
            {
                shipsRemaining.Remove(4);
                shipsPossiblePlacementCount -= 8;
            }
        }
        else if (size == 3)
        {
            shipsRemaining[3] = shipsRemaining[3] - 1;
            if (shipsRemaining[3] == 0)
            {
                shipsRemaining.Remove(3);
                shipsPossiblePlacementCount -= 6;
            }
        }
        else
        {
            shipsRemaining[2] = shipsRemaining[2] - 1;
            if (shipsRemaining[2] == 0)
            {
                shipsRemaining.Remove(2);
                shipsPossiblePlacementCount -= 4;
            }
        }
    }


    void CheckIfUnknownD(int x, int y)
    {
        bool top = false;
        bool bottom = false;
        bool right = false;
        bool left = false;

        if(x + 1 > 9) right= true;
        if(x - 1 < 0) left= true;
        if(y + 1 > 9) bottom = true;
        if(y - 1 < 0) top = true;

        if(!right && botVision[x + 1, y] == 1) 
        {
            right = true;
        }
        if (!left && botVision[x - 1, y] == 1)
        {
            left = true;
        }
        if (!top && botVision[x, y - 1] == 1)
        {
            top = true;
        }
        if (!bottom && botVision[x, y + 1] == 1)
        {
            bottom = true;
        }

        if(right && left && top && bottom)
        {
            shipsRemaining[1] = shipsRemaining[1] - 1;
            if (shipsRemaining[1] == 0)
            {
                shipsRemaining.Remove(1);
                shipsPossiblePlacementCount--;
            }
                
        }

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


    public Field GetField()
    {
        return field;
    }

    public void ShowBotShips()
    {
        field.ShowBotShips();
    }

    public void UpdateBotVision(int[,] newVision)
    {
        for (int i = 0; i < botVision.GetLength(0); i++)
        {
            for (int j = 0; j < botVision.GetLength(1); j++)
            {
                if (newVision[i, j] == 1 && botVision[i, j] == 0)
                    botVision[i, j] = 1;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
