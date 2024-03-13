using UnityEngine;

public class Player : MonoBehaviour, IKillable
{
    public Field playerFieldPrefab;
    private Field field;

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

    public void Kill(int x1, int y1)
    {
        field.Kill(x1, y1);
    }

}
