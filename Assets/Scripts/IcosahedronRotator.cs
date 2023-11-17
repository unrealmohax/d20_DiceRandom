using System.Collections;
using UnityEngine;

public class IcosahedronRotator 
{
    private readonly IRotatable _rotatable;
    public IcosahedronRotator(IRotatable rotatable)
    {
        _rotatable = rotatable;
    }

    public void StartRotate() 
    {
        CoroutineController.StartCoroutine(RotateRoutine());
    }

    private IEnumerator RotateRoutine()
    {
        float time = 0;
        while (time < _rotatable.RotationDuration)
        {
            yield return null;
            time += Time.deltaTime;

            // ������������ �������� �������� � �������������� ������
            float speed = _rotatable.RotationSpeedCurve.Evaluate(time / _rotatable.RotationDuration);

            // �������� ������� ������ ��� ������
            _rotatable.Transform.Rotate(Vector3.up, _rotatable.SpeedRotation * speed * Time.deltaTime);
        }

        StopRotation();
    }

    void StopRotation()
    {
        RotateObjectToCenter(Icosahedron.Instance.TextMeshPros[0].transform.position);
    }

    void RotateObjectToCenter(Vector3 targetPosition)
    {
        // �������� ����������� �� ����� �� ������� � ��� ���������� �����
        Vector3 directionToUp = targetPosition - _rotatable.Transform.position;

        // ��������� ���� ����� ������� ������������ ����� � ������������ � �����
        float angle = Vector3.Angle(_rotatable.Transform.forward, directionToUp);

        // �������� ��� �������� � �������������� �����-��������
        Vector3 rotationAxis = Vector3.Cross(_rotatable.Transform.forward, directionToUp).normalized;

        // ������� ���������� ��� ��������
        Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);

        // ��������� ��������
        _rotatable.Transform.rotation = rotation * _rotatable.Transform.rotation;
        EventController.Invoke(EventMessage.OnStopDice);
        // ��������������� �������� ����������� ������
        //transform.up = initialUpVector;
    }
}