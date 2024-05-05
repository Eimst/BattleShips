using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class PowerIconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipText; // Reference to the tooltip text
    private Coroutine hideCoroutine; // Coroutine to handle hiding the tooltip

    // Called when the mouse enters the object
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipText != null)
        {
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine); // Stop existing coroutine if any
            }

            tooltipText.SetActive(true); // Show the tooltip
        }
    }

    // Called when the mouse exits the object
    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip(); // Ensure tooltip is hidden
    }


    // Explicitly hides the tooltip
    private void HideTooltip()
    {
        if (tooltipText != null)
        {
            tooltipText.SetActive(false); // Hide the tooltip
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine); // Stop coroutine
            }
        }
    }
}
