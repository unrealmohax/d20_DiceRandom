using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class IcosahedronRotator : IDisposable
{
    private readonly IRotatable _rotatable;   // Reference to the IRotatable interface
    private int _randomIndex;                   // Index used to select a specific rotation vector

    // Array of pre-defined rotation vectors for each face of the icosahedron
    private Vector3[] _vectorsRotation = new Vector3[]
    {
         new Vector3(-35f, -141.5f, 87f),
        new Vector3(30f, 250f, 85f),
        new Vector3(115f, 180f, 0f),
        new Vector3(5f, 55f, 218f),
        new Vector3(301f, 103f, 185f),

        new Vector3(182f, 53.5f, -34f),
        new Vector3(187.5f, -18f, -30f),
        new Vector3(180f, -90f, -24f),
        new Vector3(8f, 19f, 151f),
        new Vector3(5f, -54f, 141f),

        new Vector3(-143f, 38f, 85),
        new Vector3(-208.5f, 68f, 85),
        new Vector3(-240f, 175f, 175f),
        new Vector3(0f, 53f, 30f),
        new Vector3(-58.5f, 109f, 0f),

        new Vector3(-5.5f, 162f, -30f),
        new Vector3(-1.5f, 234f, -33f),
        new Vector3(0.5f, -55f, -33f),
        new Vector3(0.5f, 17.5f, -33f),
        new Vector3(0.5f, 90f, -33f),
    };

    // Constructor to initialize the IcosahedronRotator with an IRotatable reference
    public IcosahedronRotator(IRotatable rotatable)
    {
        _rotatable = rotatable;
        EventController.AddListener<int>(EventMessage.OnRollDice, StartRotate);
    }

    // Method to start the rotation when the dice is rolled
    public void StartRotate(int randomValue)
    {
        _randomIndex = randomValue - 1;  // Adjusting for 0-based indexing
        CoroutineController.StartCoroutine(RotateRoutine());
    }

    // Coroutine for rotating the icosahedron
    private IEnumerator RotateRoutine()
    {
        float time = 0;
        Vector3 randomVectorRotate = new Vector3(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));

        while (time < _rotatable.RotationDuration)
        {
            yield return null;
            time += Time.deltaTime;

            // Interpolating rotation speed using a curve
            float speed = _rotatable.RotationSpeedCurve.Evaluate(time / _rotatable.RotationDuration);

            if (time < _rotatable.RotationDuration * 0.8f)
            {
                // Rotate using a random vector during the initial phase of rotation
                _rotatable.Transform.Rotate(randomVectorRotate, _rotatable.SpeedRotation * speed * Time.deltaTime);
            }
            else
            {
                // Rotate towards a pre-defined vector during the later phase of rotation
                Vector3 vectorRotation = _vectorsRotation[_randomIndex];
                Quaternion targetQuaternion = Quaternion.Euler(vectorRotation);
                _rotatable.Transform.rotation = Quaternion.RotateTowards(_rotatable.Transform.rotation, targetQuaternion, _rotatable.SpeedRotation * speed * Time.deltaTime);
            }
        }

        // Invoke an event to signal the end of rotation
        EventController.Invoke(EventMessage.OnStopDice);
    }

    // Method to dispose of the listener when the object is destroyed
    public void Dispose()
    {
        EventController.RemoveListener<int>(EventMessage.OnRollDice, StartRotate);
    }
}