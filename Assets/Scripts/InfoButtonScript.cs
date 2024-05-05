using UnityEngine;
using UnityEngine.UI;

public class ToggleInfoPanel : MonoBehaviour
{
    public GameObject InfoPanel; // Reference to the info panel

    private void Start()
    {
        // Ensure the panel starts hidden
        if (InfoPanel != null)
        {
            InfoPanel.SetActive(false);
        }

        // Attach the click event to the button's onClick
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(TogglePanelVisibility);
        }
    }

    // This method toggles the panel's active state
    private void TogglePanelVisibility()
    {
        if (InfoPanel != null)
        {
            bool isActive = InfoPanel.activeSelf;
            InfoPanel.SetActive(!isActive);
        }
    }
}
