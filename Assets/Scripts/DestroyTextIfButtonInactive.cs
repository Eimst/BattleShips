using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DestroyTextIfButtonInactive : MonoBehaviour
{
    public TextMeshProUGUI textField; // Reference to the text field to destroy
    public Button checkButton; // Reference to the button to check
    public float checkInterval = 1.0f; // Time interval for checking

    private Coroutine checkCoroutine; // Coroutine for periodic checking

    private void Start()
    {
        if (textField == null || checkButton == null)
        {
            Debug.LogError("TextField or CheckButton reference is not set.");
            return;
        }

        // Start the coroutine to check if the button is not active
        checkCoroutine = StartCoroutine(CheckAndDestroyCoroutine());
    }

    private IEnumerator CheckAndDestroyCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval); // Wait for the interval

            if (!checkButton.gameObject.activeSelf && textField != null)
            {
                // If the button is not active, destroy the text field
                Destroy(textField.gameObject);
                break; // Exit the loop after destroying the text
            }
        }
    }

    private void OnDestroy()
    {
        // Stop the coroutine to prevent memory leaks
        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
        }
    }
}
