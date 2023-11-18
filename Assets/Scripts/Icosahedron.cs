using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Icosahedron : MonoBehaviour, IMovableWithinBounds, IRotatable
{
    // Mesh components
    [SerializeField] private MeshFilter _meshFilter;           // Reference to the MeshFilter component
    [SerializeField] private MeshRenderer _meshRenderer;       // Reference to the MeshRenderer component
    [SerializeField] private Material _diceMaterial;           // Material for the dice
    [SerializeField] private ParticleSystem _bonusEffect;      // Particle system for bonus effects
    [SerializeField] private ParticleSystem _collisionEffect;  // Particle system for collision effects
    [SerializeField] private Transform _parentTransfotm;       // Parent transform for positioning
    [SerializeField, Range(0.01f, 10f)] private float _scale;   // Scale factor for the dice

    // Bounds and Movement
    [SerializeField] private Transform _bottomLeftCornal;      // Reference to the bottom-left corner transform
    [SerializeField] private Transform _upperRightCornal;      // Reference to the upper-right corner transform
    [SerializeField, Range(0.01f, 10f)] private float _moveSpeed;  // Speed of movement

    // Rotation parameters
    [SerializeField] private AnimationCurve _rotationSpeedCurve;   // Curve defining rotation speed over time
    [SerializeField, Range(1, 1000f)] private float _speedRotation; // Base speed of rotation
    [SerializeField, Range(0.01f, 10f)] private float _rotationDuration; // Total duration for a complete rotation

    // List to store TextMeshPro components for each face of the icosahedron
    private List<TextMeshPro> _textMeshPros = new List<TextMeshPro>();

    // Reference to the DiceRollView component
    private DiceRollView _diceRollView;
    public DiceRollView DiceRollView
    {
        get { return _diceRollView; }
        set
        {
            if (_diceRollView == null)
                _diceRollView = value;
        }
    }

    #region IMovableWithinBounds
    // Implementation of IMovableWithinBounds interface properties
    public Transform MovableTransform => _parentTransfotm;
    public Vector3 BottomLeftCornal => _bottomLeftCornal.position;
    public Vector3 UpperRightCornal => _upperRightCornal.position;
    public float MoveSpeed => _moveSpeed;
    public float MoveDuration => _rotationDuration - 1.5f;
    #endregion

    #region IRotatable
    // Implementation of IRotatable interface properties
    public Transform Transform => transform;
    public AnimationCurve RotationSpeedCurve => _rotationSpeedCurve;
    public float SpeedRotation => _speedRotation * 10;
    public float RotationDuration => _rotationDuration;
    #endregion

    // Called whenever the script is loaded or a value is changed in the inspector
    private void OnValidate()
    {
        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Called on the frame when a script is enabled just before any of the Update methods are called
    private void Start()
    {
        // Ensure that the mesh filter and renderer are not null
        _meshFilter = _meshFilter != null ? _meshFilter : GetComponent<MeshFilter>();
        _meshRenderer = _meshRenderer != null ? _meshRenderer : GetComponent<MeshRenderer>();

        // Set the scale of the icosahedron
        transform.localScale = new Vector3(_scale, _scale, _scale);

        // Disable collision and bonus effects initially
        _collisionEffect.gameObject.SetActive(false);
        _bonusEffect.gameObject.SetActive(false);

        // Create an IcosahedronCreator and generate the icosahedron mesh
        var creator = new IcosahedronCreator(transform, _meshFilter, _meshRenderer, _diceMaterial);
        creator.CreateIcosahedron(ref _textMeshPros);

        // Add event listeners for game events
        EventController.AddListener<int, int>(EventMessage.OnRandomAndBonusPoint, SumBonusPoint);
        EventController.AddListener(EventMessage.OnRestartGame, RestartGame);
    }

    // Method to reset the game state
    private void RestartGame()
    {
        // Reset each TextMeshPro component to its original state
        for (int i = 0; i < _textMeshPros.Count; i++)
        {
            _textMeshPros[i].text = (i + 1).ToString();
            _textMeshPros[i].fontSize = 10;
        }
    }

    // Method to play the collision effect
    public void PlayCollisionEffect()
    {
        _collisionEffect.gameObject.SetActive(true);
    }

    // Method to handle the sum of bonus points
    private void SumBonusPoint(int randomValue, int bonusValue)
    {
        CoroutineController.StartCoroutine(SumBonusPointRoutine(randomValue, bonusValue));
    }

    // Coroutine to handle the animation of bonus points
    private IEnumerator SumBonusPointRoutine(int randomValue, int bonusValue)
    {
        // Get the TextMeshPro component for the selected face
        var tmp = _textMeshPros[randomValue - 1];
        float size = tmp.fontSize;

        // Animate the increase in font size
        float time = 0f;
        while (time < 0.5f)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(size, 1.3f * size, time / 0.5f);
            tmp.fontSize = scale;
        }

        // Calculate time delay for bonus points animation
        float timeDelay = DiceRollView.BonusParticles.main.duration / bonusValue;

        // Animate the appearance of bonus points
        for (int i = 0; i < bonusValue; i++)
        {
            yield return new WaitForSeconds(timeDelay);
            tmp.text = (randomValue + i + 1).ToString();
            _bonusEffect.gameObject.SetActive(true);
        }
    }

    // Called when the script is being destroyed
    private void OnDestroy()
    {
        // Remove event listeners to prevent memory leaks
        EventController.RemoveListener(EventMessage.OnRestartGame, RestartGame);
        EventController.RemoveListener<int, int>(EventMessage.OnRandomAndBonusPoint, SumBonusPoint);
    }
}