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
    [SerializeField] private GameObject _bonusParticles;
    private const string SUCCSESS = "Sucsess";
    private const string FAILURE = "Failure";

    private void Start()
    {
        _rollButton.gameObject.SetActive(false);
        _restartButton.gameObject.SetActive(false);
        _resultText.gameObject.SetActive(false);

        EventController.AddListener<bool>(EventMessage.OnFinishGame, FinishGame);
        _rollButton.onClick.AddListener(RollButtonClick);
        _restartButton.onClick.AddListener(RestartGame);
        CoroutineController.StartCoroutine(DelayOnButton());
    }

    public void SetValueText(uint value)
    {
        _valueText.text = value.ToString();
    }

    public void SetBonusValueText(uint value)
    {
        _bonusText.text = value.ToString();
    }

    private void RestartGame() 
    {
        _resultText.gameObject.SetActive(false);
        _restartButton.gameObject.SetActive(false);
        _bonusText.color = new Color(_bonusText.color.r, _bonusText.color.g, _bonusText.color.b, 1);

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
        EventController.Invoke(EventMessage.OnRollDice);
        _rollButton.gameObject.SetActive(false);
    }

    private void FinishGame(bool isWin)
    {
        if (int.Parse(_bonusText.text) > 0)
        {
            _bonusParticles.SetActive(true);
            CoroutineController.StartCoroutine(FadeOutText(_bonusText, () =>
            {
                SetResultText(isWin);
            }));
        }
        else
        {
            SetResultText(isWin);
        }
    }

    private void SetResultText(bool isWin)
    {
        if (isWin)
        {
            _resultText.text = SUCCSESS;
            _resultText.color = _sucsessTextColor;

        }
        else
        {
            _resultText.text = FAILURE;
            _resultText.color = _failureTextColor;
        }

        CoroutineController.StartCoroutine(FadeInText(_resultText, () => _restartButton.gameObject.SetActive(true)));
        _resultText.gameObject.SetActive(true);
    }

    private IEnumerator FadeInText(TextMeshProUGUI text, Action callback = null)
    {
        Color startColor = text.color;
        float currentTime = 0f;

        while (currentTime < fadeInTime)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeInTime);
            text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        callback?.Invoke();
    }

    private IEnumerator FadeOutText(TextMeshProUGUI text, Action callback = null)
    {
        Color startColor = text.color;
        float currentTime = 0f;

        while (currentTime < _fadeOutTime)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / _fadeOutTime);
            text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        callback?.Invoke();
    }

    private void OnDestroy()
    {
        EventController.RemoveListener<bool>(EventMessage.OnFinishGame, FinishGame);
    }
}
