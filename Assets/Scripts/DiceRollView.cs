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

    private const string SUCCSESS = "Sucsess";
    private const string FAILURE = "Failure";

    public ParticleSystem BonusParticles => _bonusParticles;

    private void Start()
    {
        RestartViewNewGame();

        EventController.AddListener<bool>(EventMessage.OnFinishGame, FinishGame);
        _rollButton.onClick.AddListener(RollButtonClick);
        _restartButton.onClick.AddListener(RestartViewNewGame);
    }

    public void SetValueText(uint value)
    {
        _valueText.text = value.ToString();
    }

    public void SetBonusValueText(uint value)
    {
        _bonusText.text = value.ToString();
    }

    private void RestartViewNewGame()
    {
        _rollButton.gameObject.SetActive(false);
        _restartButton.gameObject.SetActive(false);
        _resultText.gameObject.SetActive(false);

        SetAlphaTextColor(_bonusText, 1f);

        EventController.Invoke(EventMessage.OnRestartGame);
        CoroutineController.StartCoroutine(DelayOnButton());
    }

    private IEnumerator DelayOnButton() 
    {
        yield return new WaitForSeconds(0.5f);
        _rollButton.gameObject.SetActive(true);
    }
    private void RollButtonClick()
    {
        EventController.Invoke(EventMessage.OnRollButtonClick);
        _rollButton.gameObject.SetActive(false);
    }

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

    private IEnumerator WaitParticles(Action callback) 
    {
        yield return new WaitForSeconds(1);
        EventController.Invoke(EventMessage.OnSumWithBonus);

        yield return new WaitUntil(() => !_bonusParticles.IsAlive());
        callback?.Invoke();
    }

    private void SetResultText(bool isWin)
    {
        _resultText.text = isWin ? SUCCSESS : FAILURE;
        _resultText.color = isWin ? _sucsessTextColor : _failureTextColor;

        CoroutineController.StartCoroutine(FadeInText(_resultText, () => _restartButton.gameObject.SetActive(true)));
        _resultText.gameObject.SetActive(true);
    }

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

    private void SetAlphaTextColor(TextMeshProUGUI text, float alpha)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    }

    private void OnDestroy()
    {
        EventController.RemoveListener<bool>(EventMessage.OnFinishGame, FinishGame);
    }
}
