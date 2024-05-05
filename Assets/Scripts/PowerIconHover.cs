using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PowerIconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipText; // Reference to the tooltip text

    public string tooltipMessage; // The message to display when hovering

    // When the mouse enters the object
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipText != null)
        {
            tooltipText.SetActive(true); // Show the tooltip
            tooltipText.GetComponent<Text>().text = tooltipMessage; // Set the message
        }
    }

    // When the mouse exits the object
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipText != null)
        {
            tooltipText.SetActive(false); // Hide the tooltip
        }
    }
}
