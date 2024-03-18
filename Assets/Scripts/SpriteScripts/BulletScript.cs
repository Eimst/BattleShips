using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Vector3 direction = new Vector3(1, 0, 0); // Direction of movement
    public float speed = 10f; // Speed of movement

    void Update()
    {
        // Calculate the distance to move based on speed and time
        float distanceToMove = speed * Time.deltaTime;

        // Move the UI image
        transform.Translate(direction * distanceToMove, Space.World);

        // If the UI image moves out of the screen, destroy it
        if (!IsVisibleFromCamera())
        {
            Destroy(gameObject);
        }
    }

    // Function to check if the UI image is visible from the camera
    bool IsVisibleFromCamera()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        // Check if the UI image is within the camera's view frustum
        RectTransform rectTransform = GetComponent<RectTransform>();
        Bounds bounds = new Bounds(rectTransform.rect.center, rectTransform.rect.size);
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }
}
