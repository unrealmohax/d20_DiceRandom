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
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private ParticleSystem _collisionEffect;
    [SerializeField] private List<TextMeshPro> _textMeshPros = new List<TextMeshPro>();
    [SerializeField] private Transform _parentTransfotm;
    [SerializeField, Range(0.01f, 10f)] private float _scale;

    [SerializeField] private Transform _bottomLeftCornal;
    [SerializeField] private Transform _upperRightCornal;
    [SerializeField, Range(0.01f, 10f)] private float _moveSpeed;
    [SerializeField, Range(0.01f, 10f)] private float _moveDuration;


    [SerializeField] private AnimationCurve _rotationSpeedCurve;
    [SerializeField, Range(1, 1000f)] private float _speedRotation;
    [SerializeField, Range(0.01f, 10f)] private float _rotationDuration;

    private IcosahedronMover _mover;
    private IcosahedronRotator _rotator;
    public IcosahedronMover Mover 
    {
        get { return _mover; }
        set 
        {
            if (_mover == null)
                _mover = value; 
        }
    }
    public IcosahedronRotator Rotator
    {
        get { return _rotator; }
        set
        {
            if (_rotator == null)
                _rotator = value;
        }
    }

    #region IMovableWithinBounds
    public Transform MovableTransform => _parentTransfotm;
    public Vector3 BottomLeftCornal => _bottomLeftCornal.position;
    public Vector3 UpperRightCornal => _upperRightCornal.position;
    public float MoveSpeed => _moveSpeed;
    public float MoveDuration => _rotationDuration - 0.5f;

    #endregion
    #region IRotatable
    public Transform Transform => transform;

    public AnimationCurve RotationSpeedCurve => _rotationSpeedCurve;

    public float SpeedRotation => _speedRotation * 10;

    public float RotationDuration => _rotationDuration;
    #endregion
    public IReadOnlyList<TextMeshPro> TextMeshPros => _textMeshPros;

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

        var creator = new IcosahedronCreator(transform, _meshFilter, _meshRenderer, _diceMaterial, _particleSystem);
        creator.CreateIcosahedron(ref _textMeshPros);


        EventController.AddListener(EventMessage.OnRollDice, RollDice);
    }

    public void RollDice() 
    {
        _mover.StartMove();
        _rotator.StartRotate();
    }

    public void PlayCollisionEffect() 
    {
        _collisionEffect.gameObject.SetActive(true);
    }
    private void OnDestroy()
    {
        EventController.RemoveListener(EventMessage.OnRollDice, RollDice);
    }
}