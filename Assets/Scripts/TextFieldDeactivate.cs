using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class TextFieldDeactivate : MonoBehaviour
{
    public TextMeshProUGUI textField; // Reference to the text field to deactivate
    public Button checkButton; // Reference to the button to check
    public float checkInterval = 1.0f; // Time interval for checking the button's active state

    private Coroutine checkCoroutine; // Coroutine for checking and deactivating

    private void Start()
    {
        if (textField == null || checkButton == null)
        {
            Debug.LogError("TextField or CheckButton reference is not set.");
            return;
        }

        // Start the coroutine that checks the button state
        checkCoroutine = StartCoroutine(CheckAndDeactivateCoroutine());
    }

    private IEnumerator CheckAndDeactivateCoroutine()
    {
        while (true) // Continuous loop
        {
            yield return new WaitForSeconds(checkInterval); // Wait for the interval

            if (!checkButton.gameObject.activeSelf && textField.gameObject.activeSelf)
            {
                // If the button is not active and the text field is active, deactivate the text field
                textField.gameObject.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        // Stop the coroutine to avoid memory leaks
        if (checkCoroutine != null)
        {
            StopCoroutine(checkCoroutine);
        }
    }
}
