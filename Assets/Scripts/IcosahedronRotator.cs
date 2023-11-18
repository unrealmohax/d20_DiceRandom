using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class IcosahedronRotator : IDisposable
{
    private readonly IRotatable _rotatable;
    private int _randomIndex;
     
    private Vector3[] _position = new Vector3[]
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

    public IcosahedronRotator(IRotatable rotatable)
    {
        _rotatable = rotatable;
        EventController.AddListener<int>(EventMessage.OnRollDice, StartRotate);
    }

    public void StartRotate(int randomValue) 
    {
        _randomIndex = randomValue -1;
        CoroutineController.StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        float time = 0;
        Vector3 randomVectorRotate = new Vector3(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));
        while (time < _rotatable.RotationDuration)
        {
            yield return null;
            time += Time.deltaTime;

            // »нтерпол€ци€ скорости вращени€ с использованием кривой
            float speed = _rotatable.RotationSpeedCurve.Evaluate(time / _rotatable.RotationDuration);

            if (time < _rotatable.RotationDuration * 0.8f)
            {
                _rotatable.Transform.Rotate(randomVectorRotate, _rotatable.SpeedRotation * speed * Time.deltaTime);
            }
            else 
            {
                Vector3 vectorRotation = _position[_randomIndex];
                Quaternion targetQuaternion = Quaternion.Euler(vectorRotation);
                _rotatable.Transform.rotation = Quaternion.RotateTowards(_rotatable.Transform.rotation, targetQuaternion, _rotatable.SpeedRotation * speed * Time.deltaTime);
            }
        }

        EventController.Invoke(EventMessage.OnStopDice);
    }

    public void Dispose()
    {
        EventController.RemoveListener<int>(EventMessage.OnRollDice, StartRotate);
    }
}