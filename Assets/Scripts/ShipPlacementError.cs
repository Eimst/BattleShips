using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShipPlacementError : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI error;
    private bool isBlinking = false;

    public void ShowError()
    {
        if(!isBlinking)
            StartCoroutine(PulseButtonCoroutine());
    }
    

    private IEnumerator PulseButtonCoroutine(float durationPerPulse = 0.4f)
    {
        isBlinking = true;
        error.gameObject.SetActive(true);
        Color originalColor = error.color;
        RectTransform rectTransform = error.GetComponent<RectTransform>();
        Vector3 originalScale = rectTransform.localScale;
        int pulseCount = 0;


        while (pulseCount < 3)
        {
            float elapsed = 0f;
            while (elapsed < durationPerPulse)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.SmoothStep(1f, 0.8f, elapsed / durationPerPulse);
                float scale = Mathf.Lerp(1f, 0.95f, elapsed / durationPerPulse);
                error.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                rectTransform.localScale = originalScale * scale;
                yield return null;
            }

            elapsed = 0f; // Reset elapsed time for the fade-in
            while (elapsed < durationPerPulse)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.SmoothStep(0.8f, 1f, elapsed / durationPerPulse); 
                float scale = Mathf.Lerp(0.95f, 1f, elapsed / durationPerPulse);
                error.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                rectTransform.localScale = originalScale * scale;
                yield return null;
            }

            pulseCount++; // Increment the pulse counter after a complete fade out and fade in
        }
        
        error.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        rectTransform.localScale = originalScale;
        
        error.gameObject.SetActive(false);
        isBlinking = false;
    }
}
