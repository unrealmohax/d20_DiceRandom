using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Icosahedron : MonoBehaviour, IMovableWithinBounds, IRotatable
{
    public static Icosahedron Instance;
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _diceMaterial;
    [SerializeField] private ParticleSystem _bonusEffect;
    [SerializeField] private ParticleSystem _collisionEffect;
    [SerializeField] private Transform _parentTransfotm;
    [SerializeField, Range(0.01f, 10f)] private float _scale;

    [SerializeField] private Transform _bottomLeftCornal;
    [SerializeField] private Transform _upperRightCornal;
    [SerializeField, Range(0.01f, 10f)] private float _moveSpeed;

    [SerializeField] private AnimationCurve _rotationSpeedCurve;
    [SerializeField, Range(1, 1000f)] private float _speedRotation;
    [SerializeField, Range(0.01f, 10f)] private float _rotationDuration;

    private List<TextMeshPro> _textMeshPros = new List<TextMeshPro>();

    private DiceRollView _diceRollView;
    public DiceRollView DiceRollView 
    {
        get { return _diceRollView; }
        set 
        { 
            if(_diceRollView == null)
            _diceRollView = value; 
        }
    }

    #region IMovableWithinBounds
    public Transform MovableTransform => _parentTransfotm;
    public Vector3 BottomLeftCornal => _bottomLeftCornal.position;
    public Vector3 UpperRightCornal => _upperRightCornal.position;
    public float MoveSpeed => _moveSpeed;
    public float MoveDuration => _rotationDuration - 1.5f;

    #endregion
    #region IRotatable
    public Transform Transform => transform;

    public AnimationCurve RotationSpeedCurve => _rotationSpeedCurve;

    public float SpeedRotation => _speedRotation * 10;

    public float RotationDuration => _rotationDuration;
    #endregion

    private void OnValidate()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        Instance = this;
        _meshFilter = _meshFilter != null ? _meshFilter : GetComponent<MeshFilter>();
        _meshRenderer = _meshRenderer != null ? _meshRenderer : GetComponent<MeshRenderer>();

        transform.localScale = new Vector3(_scale, _scale, _scale);

        _collisionEffect.gameObject.SetActive(false);
        _bonusEffect.gameObject.SetActive(false);

        var creator = new IcosahedronCreator(transform, _meshFilter, _meshRenderer, _diceMaterial);
        creator.CreateIcosahedron(ref _textMeshPros);

        EventController.AddListener<int, int>(EventMessage.OnRandomAndBonusPoint, SumBonusPoint);
        EventController.AddListener(EventMessage.OnRestartGame, RestartGame);
    }

    private void RestartGame()
    {
        for (int i = 0; i < _textMeshPros.Count; i++) 
        {
            _textMeshPros[i].text = (i + 1).ToString();
            _textMeshPros[i].fontSize = 10;
        }
    }

    public void PlayCollisionEffect() 
    {
        _collisionEffect.gameObject.SetActive(true);
    }
    
    private void SumBonusPoint(int randomValue, int bonusValue)
    {
        CoroutineController.StartCoroutine(SumBonusPointRoutine(randomValue, bonusValue));
    }
    private IEnumerator SumBonusPointRoutine(int randomValue, int bonusValue) 
    {
        var tmp = _textMeshPros[randomValue - 1];
        float size = tmp.fontSize;

        float time = 0f;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(size, 1.3f * size, time / 0.5f);
            tmp.fontSize = scale;
        }

        float timeDelay = DiceRollView.BonusParticles.main.duration / bonusValue;
        for (int i = 0; i < bonusValue; i++) 
        {
            yield return new WaitForSeconds(timeDelay);
            tmp.text = (randomValue + i + 1).ToString();
            _bonusEffect.gameObject.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        EventController.RemoveListener(EventMessage.OnRestartGame, RestartGame);
        EventController.RemoveListener<int, int>(EventMessage.OnRandomAndBonusPoint, SumBonusPoint);
    }
}