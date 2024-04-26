using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

    private ShootingManager _shootingManager;

    private enum GameState
    {
        Early,
        Mid_Late,
        OnlyOneTileLeft,
        TryEdges,
    }

    private GameState gameState;

    private int shipsPossiblePlacementCount = 19;

    public bool tryEdges {get; set;}

    private int tryEdgesCount;

    private int shotCount;


    private Stack<string> _unfinishedShips;

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
        tryEdges = false;
        shipsRemaining.Add(1, 4);
        shipsRemaining.Add(2, 3);
        shipsRemaining.Add(3, 2);
        shipsRemaining.Add(4, 1);
        shotCount = 5;
        field.shipsCount = new int[] { 4, 3, 2, 1 };
        _unfinishedShips = new Stack<string>();
    }



    public void SetShootingManager(ShootingManager shootingManager)
    {
        _shootingManager = shootingManager;
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

        if (shipsPossiblePlacementCount == 1)
        {
            shotCount = 2;
            FindAnyObjectByType<ShootingManager>().SendMessage("SetCountMissedShot", 2);
        }
            

        if(!tryEdges)
        {
            if (countHitCells > 25)
            {
                if (shipsPossiblePlacementCount == 1)
                    gameState = GameState.OnlyOneTileLeft;
                else gameState = GameState.Mid_Late;

            }
            else gameState = GameState.Early;
            
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


    string FindCoordinateWithPotentialTarget(double target = 0.90, int attempt = 0)
    {
        List<string> coordinates = new List<string>();
        for (int i = 0; i < heatMap.GetLength(0); i++)
        {
            for (int j = 0; j < heatMap.GetLength(1); j++)
            {
                if(gameState == GameState.TryEdges && EdgesCoordinates(i, j, target - 0.3))
                {
                    coordinates.Add(i + " " + j);
                }
                else if (gameState != GameState.TryEdges && heatMap[i, j] >= (gameState == GameState.Early ? 0.80 : target))
                {
                    coordinates.Add(i + " " + j);
                }
            }
        }
        // lastHit = coordinates;
        if(attempt == 5)
        {
            gameState = GameState.Mid_Late;
            tryEdges = false;
        }
            
        if (coordinates.Count == 0)
            return FindCoordinateWithPotentialTarget(target - 0.05, attempt + 1);
        return GetRandomCoordinate(coordinates);
    }


    bool EdgesCoordinates(int i, int j, double target)
    {
        if((i >= 0 && (j == 0 || j == 9)) || (j >= 0 && (i == 0 || i == 9)))
            if (heatMap[i, j] >= target)
                return true;
        return false;
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
        else if(gameState == GameState.TryEdges)
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
            {
                if((gameState == GameState.TryEdges || gameState == GameState.Early) && IsClearNearby(int.Parse(coordinates[randomCoordinate].Split(' ')[0]),
                    int.Parse(coordinates[randomCoordinate].Split(' ')[1])))
                {
                    return coordinates[randomCoordinate];
                }
                else if(gameState != GameState.Early) 
                    return coordinates[randomCoordinate];
            }

                
            count++;
        }
        return coordinates[Random.Range(0, coordinates.Count)];
    }


    bool IsClearNearby(int x, int y)
    {
        for (int i = -1; i <= 1; i++)
        {
            if(y + i < 0 || y + i > 9) continue;
            for (int j = -1; j <= 1; j++)
            {
                if (x + j < 0 || x + j > 9 || (i == 0 && j == 0)) 
                    continue;
                //Debug.Log(i + " " + j + " " + botVision[x+j, y + i]);
                if (botVision[x + j, y + i] != 0)
                    return false;
                
            }
        }
        
        return true;
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
        string coordinates = "";

        for (int i = 1; i <= 3; i++)
        {
            if (!isHorizontal && yOrig + i < 10)
            {
                if (botVision[xOrig, yOrig + i] == 0)
                {
                    coordinates = xOrig + " " + (yOrig + i) + ";" + heatMap[xOrig, yOrig + i];
                    return coordinates;
                }
                else if (botVision[xOrig, yOrig + i] == 1)
                    break;
            }

            else if (isHorizontal && xOrig + i < 10)
            {
                if (botVision[xOrig + i, yOrig] == 0)
                {
                    coordinates = xOrig + i + " " + yOrig + ";" + heatMap[xOrig + i, yOrig];
                    return coordinates;
                }
                else if (botVision[xOrig + i, yOrig] == 1)
                    break;
            }
        }
        return coordinates;
    }

    string CaseHelperMinus(int xOrig, int yOrig, bool isHorizontal)
    {
        string coordinates = "";
        for (int i = 1; i <= 3; i++)
        {
            if (!isHorizontal && yOrig - i >= 0)
            {
                if (botVision[xOrig, yOrig - i] == 0)
                {
                    coordinates = xOrig + " " + (yOrig - i) + ";" + heatMap[xOrig, yOrig - i];
                    return coordinates; 
                }
                else if (botVision[xOrig, yOrig - i] == 1)
                    break;
            }
            else if (isHorizontal && xOrig - i >= 0)
            {
                if (botVision[xOrig - i, yOrig] == 0)
                {
                    coordinates = xOrig - i + " " + yOrig + ";" + heatMap[xOrig - i, yOrig];
                    return coordinates;
                }
                else if (botVision[xOrig - i, yOrig] == 1)
                    break;
            }
        }
        return coordinates;
    }

    string CaseVertical(int xOrig, int yOrig)
    {
        Dictionary<string, double> heatList = new Dictionary<string, double>();
        string plusCoor = CaseHelperPlus(xOrig, yOrig, false);
        string minusCoor = CaseHelperMinus(xOrig, yOrig, false);

        if(plusCoor.Length > 0)
            heatList.Add(plusCoor.Split(";")[0], double.Parse(plusCoor.Split(";")[1]));

        if(minusCoor.Length > 0)
            heatList.Add(minusCoor.Split(";")[0], double.Parse(minusCoor.Split(";")[1]));

        double maxHeat = 0;
        string coordinates = "";
        foreach(KeyValuePair<string, double> heat in heatList)
        {
            if(heat.Value > maxHeat)
            {
                maxHeat = heat.Value;
                coordinates = heat.Key;
            }
        }

        return coordinates;
    }


    string CaseHorizontal(int xOrig, int yOrig)
    {
        Dictionary<string, double> heatList = new Dictionary<string, double>();
        string plusCoor = CaseHelperPlus(xOrig, yOrig, true);
        string minusCoor = CaseHelperMinus(xOrig, yOrig, true);

        if (plusCoor.Length > 0)
            heatList.Add(plusCoor.Split(";")[0], double.Parse(plusCoor.Split(";")[1]));

        if (minusCoor.Length > 0)
            heatList.Add(minusCoor.Split(";")[0], double.Parse(minusCoor.Split(";")[1]));

        double maxHeat = 0;
        string coordinates = "";
        foreach (KeyValuePair<string, double> heat in heatList)
        {
            if (heat.Value > maxHeat)
            {
                maxHeat = heat.Value;
                coordinates = heat.Key;
            }
        }

        return coordinates;
    }


    string CaseUnknown(int xOrig, int yOrig)
    {

        Dictionary<string, double> heatList = new Dictionary<string, double>();

        if(xOrig + 1 < 10 && botVision[xOrig + 1, yOrig] == 0)
            heatList.Add(xOrig + 1 + " " + yOrig, heatMap[xOrig + 1, yOrig]);

        if(xOrig - 1 >= 0 && botVision[xOrig - 1, yOrig] == 0)
            heatList.Add(xOrig - 1 + " " + yOrig, heatMap[xOrig - 1, yOrig]);

        if (yOrig + 1 < 10 && botVision[xOrig, yOrig + 1] == 0)
            heatList.Add(xOrig + " " + (yOrig + 1), heatMap[xOrig, yOrig + 1]);

        if (yOrig - 1 >= 0 && botVision[xOrig, yOrig - 1] == 0)
            heatList.Add(xOrig + " " + (yOrig - 1), heatMap[xOrig, yOrig - 1]);

        double maxHeat = 0;
        string coordinates = "";
        foreach(KeyValuePair<string, double> heat in heatList)
        {
            if(heat.Value > maxHeat)
            {
                maxHeat = heat.Value;
                coordinates = heat.Key;
            }
        }
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
        if (hit)
        {
            lastSuccessfulHit = lastHit;
           // botVision[int.Parse(lastSuccessfulHit.Split(' ')[0]), int.Parse(lastSuccessfulHit.Split(' ')[1])] = -1;
            state = BotState.Atacking;
            hit = false;
        }
        else if (tryEdges)
        {
            tryEdgesCount--;
            if (tryEdgesCount <= 0)
                tryEdges = false;
            else gameState = GameState.TryEdges;
        }

        if (state == BotState.Searching && _unfinishedShips.Count > 0)
        {
            state = BotState.Atacking;
            lastSuccessfulHit = _unfinishedShips.Pop();
        }
        RecalculateHeatMap();
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
            //Debug.LogError(lastHit + "   Last Successful hit "+ lastSuccessfulHit);
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
        if (size == 4 && shipsRemaining.ContainsKey(4))
        {
            shipsRemaining[4] = shipsRemaining[4] - 1;
            if (shipsRemaining[4] == 0)
            {
                shipsRemaining.Remove(4);
                shipsPossiblePlacementCount -= 8;
            }
        }
        else if (size == 3 && shipsRemaining.ContainsKey(3))
        {
            shipsRemaining[3] = shipsRemaining[3] - 1;
            if (shipsRemaining[3] == 0)
            {
                shipsRemaining.Remove(3);
                shipsPossiblePlacementCount -= 6;
            }
        }
        else if(shipsRemaining.ContainsKey(2))
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
        if(size == 4 && shipsRemaining.ContainsKey(4))
        {
            shipsRemaining[4] = shipsRemaining[4] - 1;
            if (shipsRemaining[4] == 0)
            {
                shipsRemaining.Remove(4);
                shipsPossiblePlacementCount -= 8;
            }
        }
        else if (size == 3 && shipsRemaining.ContainsKey(3))
        {
            shipsRemaining[3] = shipsRemaining[3] - 1;
            if (shipsRemaining[3] == 0)
            {
                shipsRemaining.Remove(3);
                shipsPossiblePlacementCount -= 6;
            }
        }
        else if(shipsRemaining.ContainsKey(2))
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
        var top = false;
        var bottom = false;
        var right = false;
        var left = false;

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

    public int[] GetShipsCount()
    {
        return field.ShipsCount();
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

    public void ResetEdgesCount()
    {
        tryEdgesCount = shotCount;
    }


    public void SetHitShipStatus(int x, int y)
    {
        botVision[x, y] = -1;
        if (_shootingManager.botChosenAbility != ShootingManager.ChosenAbility.None)
        {
            _unfinishedShips.Push(x + " " + y);
        }
    }
    
    
    
    
    
    
    // With Ability ////////////////////////////////////////////////////////////////////////////////////////////////////

    private string CalculateBestPositionForShot(List<ShootingManager.ChosenAbility> abilities)
    {
        string[] x3 = new string[2];
        string[] hozVer = new string[3];
        
        if(abilities.Contains(ShootingManager.ChosenAbility.x3))
            x3 = PlaceSliderX3();
        
        if(abilities.Contains(ShootingManager.ChosenAbility.Horizontal))
            hozVer = SpecialAbiHoz();

        string coord = DetermineWhichAbilityToUse(x3, hozVer);

        if (coord.Length > 0)
            return coord;
        
        // sonaras
        return "";
    }


    private string DetermineWhichAbilityToUse(string[] x3, string[] hozVer)
    {
        if (hozVer[0] == null && x3[0] == null)
            return "";
        
        if ((x3[0] != null && hozVer[0] == null) || (x3[0] != null && hozVer[0] != null && double.Parse(x3[1]) > double.Parse(hozVer[1])))
        {
            _shootingManager.botChosenAbility = ShootingManager.ChosenAbility.x3;
            return x3[0];
        }
        
        _shootingManager.botChosenAbility = hozVer[2].Equals("1") ? ShootingManager.ChosenAbility.Horizontal : ShootingManager.ChosenAbility.Vertical;
        
        return hozVer[0];
    }


    private string[] SpecialAbiHoz()
    {
        // Hoz
        double max = 0;
        string coordHoz = "";
        for (int i = 0; i <= 9; i++)
        {
            double sum = 0;
            for (int j = 0; j <= 9; j++)
            {
                sum += heatMap[j, i];
            }

            if (sum > max)
            {
                max = sum;
                coordHoz = 0+ " " + i;
            }
        }
        // Ver
        string[] vertical = SpecialAbiVer(max);

        if (vertical[0].Length > 0)
            return vertical;

        return new [] {coordHoz, max.ToString(), "1"};
    }

    
    private string[] SpecialAbiVer(double max)
    {
        string coord = "";
        for (int i = 0; i <= 9; i++)
        {
            double sum = 0;
            for (int j = 0; j <= 9; j++)
            {
                sum += heatMap[i, j];
            }
            if (sum > max)
            {
                max = sum;
                coord = i + " " + 0;
            }
        }

        return new [] {coord, max.ToString(), "0"};
    }
    
    
    private string[] PlaceSliderX3()
    {
        double max = 0;
        string coordinates = "";
        for (int i = 0; i <= 9; i++)
        {
            for (int j = 0; j <= 9; j++)
            {
                double current = AbilityX3(i, j);
                if (current >= max)
                {
                    max = current;
                    coordinates = i + " " + j;
                }
            }
        }

        return new [] {coordinates, max.ToString()};

    }

    private double AbilityX3(int x, int y)
    {
        double sum = 0;
        for (int i = x - 1 < 0 ? 0 : x - 1; i < (x + 2 > 10 ? 10 : x + 2); i++)
        {
            for (int j = y - 1 < 0 ? 0 : y - 1; j < (y + 2 > 10 ? 10 : y + 2); j++)
            {
                sum += heatMap[i, j];
            }
        }

        return sum;
    }
    
    
    
    public string ApplyShotWithSpecialAbility(List<ShootingManager.ChosenAbility> abilities)
    {
        gameManager.UpdateBotVision();
        RecalculateHeatMap();
        return CalculateBestPositionForShot(abilities);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
