using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLayerPriority : MonoBehaviour
{
    public string sortingLayerName = "Background";
    public int orderInLayer = 5;
    // Start is called before the first frame update
    void Start()
    {
        // Change sorting layer and order in layer of the Mesh Renderer
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingLayerName = sortingLayerName;
        meshRenderer.sortingOrder = orderInLayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
