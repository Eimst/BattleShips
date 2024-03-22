using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject sound;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(sound);
    }
}
