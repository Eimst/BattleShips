using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI textPlayerTurn;

    public Toggle check;

    private bool lastState = true;
    private bool isFadingIn = false;
    private bool isFadingOut = false;


    private void Update()
    {
        bool currentState = check.isOn;

        if (currentState != lastState)
        {
            lastState = currentState;
            if (currentState && FindObjectOfType<GameManager>().currentState == GameManager.GameState.PlayerTurn)
            {
                StartCoroutine(FadeInTextCoroutine(textPlayerTurn, 1f));
            }
            else
            {
                StartCoroutine(FadeOutTextCoroutine(textPlayerTurn, 1f));
            }
            
        }


    }


    public void FadeInTextPlayerTurn(float duration)
    {
        if (check.isOn && !isFadingIn)
        {
            StartCoroutine(FadeInTextCoroutine(textPlayerTurn, duration));
        }
    }

    public void FadeOutTextPlayerTurn(float duration)
    {
        if (check.isOn && !isFadingOut)
        {
            StartCoroutine(FadeOutTextCoroutine(textPlayerTurn, duration));
        }
    }

    private IEnumerator FadeInTextCoroutine(TextMeshProUGUI text, float duration)
    {
        isFadingIn = true;
        isFadingOut = false;
        yield return new WaitUntil(() => FindObjectOfType<GameManager>().isAnimationDone);
        text.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        isFadingIn = false;
    }

    private IEnumerator FadeOutTextCoroutine(TextMeshProUGUI text, float duration)
    {
        isFadingOut = true;
        isFadingIn = false;
        yield return new WaitUntil(() => FindObjectOfType<GameManager>().isAnimationDone);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.SmoothStep(1f, 0f, elapsed / duration);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        text.gameObject.SetActive(false);
        isFadingOut = false;
    }
}
