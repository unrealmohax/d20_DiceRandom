using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private TextMeshProUGUI _bonusText;

    [SerializeField] private GameObject _buttonImage;
    [SerializeField] private Button _rollButton;
    [SerializeField] private Button _restartButton;

    [SerializeField] private Color _sucsessTextColor;
    [SerializeField] private Color _failureTextColor;
    [SerializeField] private float fadeInTime = 4f;
    [SerializeField] private float _fadeOutTime = 2f;

    [SerializeField] private ParticleSystem _bonusParticles;

    private const string SUCCSESS = "Success";   
    private const string FAILURE = "Failure";

    public ParticleSystem BonusParticles => _bonusParticles;

    private void Start()
    {
        RestartViewNewGame();

        EventController.AddListener<bool>(EventMessage.OnFinishGame, FinishGame);
        _rollButton.onClick.AddListener(RollButtonClick);
        _restartButton.onClick.AddListener(RestartViewNewGame);
    }

    // Method to set the value text on the UI
    public void SetValueText(uint value)
    {
        _valueText.text = value.ToString();
    }

    // Method to set the bonus value text on the UI
    public void SetBonusValueText(uint value)
    {
        _bonusText.text = value.ToString();
    }

    // Method to initialize the UI for a new game
    private void RestartViewNewGame()
    {
        _rollButton.gameObject.SetActive(false);
        _restartButton.gameObject.SetActive(false);
        _resultText.gameObject.SetActive(false);

        SetAlphaTextColor(_bonusText, 1f);

        EventController.Invoke(EventMessage.OnRestartGame);
        CoroutineController.StartCoroutine(DelayOnButton());
    }

    // Coroutine to introduce a delay before showing the roll button
    private IEnumerator DelayOnButton()
    {
        yield return new WaitForSeconds(0.5f);
        _rollButton.gameObject.SetActive(true);
    }

    // Method to handle the roll button click event
    private void RollButtonClick()
    {
        EventController.Invoke(EventMessage.OnRollButtonClick);
        _rollButton.gameObject.SetActive(false);
    }

    // Method to handle the finish game event
    private void FinishGame(bool isWin)
    {
        if (int.Parse(_bonusText.text) > 0)
        {
            _bonusParticles.gameObject.SetActive(true);
            CoroutineController.StartCoroutine(FadeOutText(_bonusText, () =>
            {
                CoroutineController.StartCoroutine(WaitParticles(() =>
                {
                    SetResultText(isWin);
                }));
            }));
        }
        else
        {
            SetResultText(isWin);
        }
    }

    // Coroutine to wait for the bonus particles to finish before triggering further events
    private IEnumerator WaitParticles(Action callback)
    {
        yield return new WaitForSeconds(1);
        EventController.Invoke(EventMessage.OnSumWithBonus);

        yield return new WaitUntil(() => !_bonusParticles.IsAlive());
        callback?.Invoke();
    }

    // Method to set the result text on the UI based on win/loss
    private void SetResultText(bool isWin)
    {
        _resultText.text = isWin ? SUCCSESS : FAILURE;
        _resultText.color = isWin ? _sucsessTextColor : _failureTextColor;

        CoroutineController.StartCoroutine(FadeInText(_resultText, () => _restartButton.gameObject.SetActive(true)));
        _resultText.gameObject.SetActive(true);
    }

    // Coroutine to fade in text over time
    private IEnumerator FadeInText(TextMeshProUGUI text, Action callback = null)
    {
        float currentTime = 0f;

        while (currentTime < fadeInTime)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInTime);
            SetAlphaTextColor(text, alpha);

            yield return null;
        }

        callback?.Invoke();
    }

    // Coroutine to fade out text over time
    private IEnumerator FadeOutText(TextMeshProUGUI text, Action callback = null)
    {
        float currentTime = 0f;

        while (currentTime < _fadeOutTime)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / _fadeOutTime);
            SetAlphaTextColor(text, alpha);

            yield return null;
        }

        callback?.Invoke();
    }

    // Method to set alpha value for text color
    private void SetAlphaTextColor(TextMeshProUGUI text, float alpha)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }

    // Method to remove event listener when the object is destroyed
    private void OnDestroy()
    {
        EventController.RemoveListener<bool>(EventMessage.OnFinishGame, FinishGame);
    }
}
