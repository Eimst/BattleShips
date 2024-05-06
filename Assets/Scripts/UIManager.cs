using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI textPlayerTurn;

    public TextMeshProUGUI botAbText;
    
    public TMP_Text shipsRemainingPlayer;
    public TMP_Text shipsRemainingBot;

    public Toggle check;

    private bool lastState = true;
    private bool isFadingIn = false;
    private bool isFadingOut = false;
    
    public GameObject powersPanel;
    
    public Button x3;
    public TextMeshProUGUI x3Tooltip;

    public Button hozVer;
    public TextMeshProUGUI hozVerTooltip;

    public Button sonar;
    public TextMeshProUGUI sonarTooltip;

    private GameManager _gameManager;


    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        bool currentState = check.isOn;

        if (currentState != lastState)
        {
            lastState = currentState;
            if (currentState && FindObjectOfType<GameManager>().currentState == GameManager.GameState.PlayerTurn)
            {
                StartCoroutine(FadeInTextCoroutine(textPlayerTurn, 0.5f));
            }
            else
            {
                StartCoroutine(FadeOutTextCoroutine(textPlayerTurn, 0.5f));
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

    
    public void FadeInTextBotAb(float duration, ShootingManager.ChosenAbility botChosenAbility)
    {
        switch (botChosenAbility)
        {
            case ShootingManager.ChosenAbility.x3:
                botAbText.SetText( "Bot activated special ability \n3x3");
                break;
            case ShootingManager.ChosenAbility.Horizontal:
                botAbText.SetText("Bot activated special ability \nHorizontal");
                break;
            case ShootingManager.ChosenAbility.Vertical:
                botAbText.SetText("Bot activated special ability \nVertical");
                break;
            case ShootingManager.ChosenAbility.Sonar:
                botAbText.SetText("Bot activated special ability \nSonar");
                break;
        }
        StartCoroutine(FadeInTextCoroutine(botAbText, duration));
    }
    
    
    public void FadeOutTextBotAb(float duration)
    {
        StartCoroutine(FadeOutTextCoroutine(botAbText, duration));
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


    public void AddSpecialPower()
    {
        powersPanel.SetActive(true);
    }
    
    public void ShowRemainingShips(bool bot, string text)
    {
        if (bot)
            shipsRemainingBot.text = text;
        else shipsRemainingPlayer.text = text;
    }
    
    
    
    public void FadeInPowerButton(int ability, float duration = 0.3f)
    {
        switch (ability)
        {
            case 1:
                StartCoroutine(FadeInPowerCoroutine(duration, x3));
                break;
            case 2:
                StartCoroutine(FadeInPowerCoroutine(duration, hozVer));
                break;
            case 4:
                StartCoroutine(FadeInPowerCoroutine(duration, sonar));
                break;
            default:
                Debug.LogWarning("No such ability: " + ability);
                break;
        }
    }

    private IEnumerator FadeInPowerCoroutine(float duration, Button button)
    {
     
        //button.gameObject.SetActive(true); // Set the button active but transparent
        Image backgroundImage = button.GetComponent<Image>();
        backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 0f); 

        yield return new WaitUntil(() => _gameManager.currentState == GameManager.GameState.PlayerTurn && _gameManager.isAnimationDone);
        button.gameObject.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, alpha);
           
            yield return null;
        }

        // Optionally set alpha to 1f explicitly to avoid minor transparency issues after fade
        backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 1f);
    }
    
    
    public void FadeOutPowerButton(float duration = 1f)
    {
        if(x3.IsActive())
            StartCoroutine(FadeOutPowerCoroutine(duration, x3, x3Tooltip));
        
        if(hozVer.IsActive())
            StartCoroutine(FadeOutPowerCoroutine(duration, hozVer, hozVerTooltip));
        
        if(sonar.IsActive())
            StartCoroutine(FadeOutPowerCoroutine(duration, sonar, sonarTooltip));
    }
    
    private IEnumerator FadeOutPowerCoroutine(float duration, Button button, TextMeshProUGUI toolTip)
    {
        Image backgroundImage = button.GetComponent<Image>();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.SmoothStep(1f, 0f, elapsed / duration);
            backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, alpha);
            yield return null;
        }
        //  backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 0f);
        toolTip.gameObject.SetActive(false);
        button.gameObject.SetActive(false);
    }
    
   

}
