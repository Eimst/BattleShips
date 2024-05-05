using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class HideAfterDelay : MonoBehaviour
{
    public TextMeshProUGUI sonarTooltip; // Reference to the TextMeshProUGUI field
    public TextMeshProUGUI bombTooltip; // Reference to the TextMeshProUGUI field
    public TextMeshProUGUI lineStrikeTooltip; // Reference to the TextMeshProUGUI field
    public float delay = 2.0f; // Delay before hiding the text

    private Coroutine hideCoroutine; // Reference to the coroutine

    // Method to be called when the button is pressed
    public void OnButtonPress()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine); // Stop existing coroutine, if any
        }

        hideCoroutine = StartCoroutine(HideTextAfterDelayCoroutine());
    }

    // Coroutine to hide the text field after a delay
    private IEnumerator HideTextAfterDelayCoroutine()
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay

        if (sonarTooltip != null)
        {
            sonarTooltip.gameObject.SetActive(false); // Hide the text field
        }
        if (bombTooltip != null)
        {
            bombTooltip.gameObject.SetActive(false); // Hide the text field
        }
        if (lineStrikeTooltip != null)
        {
            lineStrikeTooltip.gameObject.SetActive(false); // Hide the text field
        }
    }
}