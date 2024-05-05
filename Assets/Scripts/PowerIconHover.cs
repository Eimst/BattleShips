using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections; // Needed to use coroutines

public class PowerIconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltipText; // Reference to the tooltip text
    public float tooltipDuration = 1.0f; // Time before the tooltip disappears

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


            // Start a coroutine to hide the tooltip after the specified duration
            hideCoroutine = StartCoroutine(HideTooltipAfterDelay());
        }
    }

    // Called when the mouse exits the object
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipText != null)
        {
            if (hideCoroutine != null)
            {
                StopCoroutine(hideCoroutine); // Stop the coroutine to prevent premature hiding
            }

            tooltipText.SetActive(false); // Hide the tooltip
        }
    }

    // Coroutine to hide the tooltip after a delay
    private IEnumerator HideTooltipAfterDelay()
    {
        yield return new WaitForSeconds(tooltipDuration); // Wait for the specified time
        tooltipText.SetActive(false); // Hide the tooltip
    }
}
