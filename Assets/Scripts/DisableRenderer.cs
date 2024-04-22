using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("GreenPower").GetComponent<Renderer>().enabled = false;
        GameObject.Find("RedPower").GetComponent<Renderer>().enabled = false;
        GameObject.Find("BluePower").GetComponent<Renderer>().enabled = false;
    }
}
