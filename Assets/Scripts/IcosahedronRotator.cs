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

            // Интерполяция скорости вращения с использованием кривой
            float speed = _rotatable.RotationSpeedCurve.Evaluate(time / _rotatable.RotationDuration);

            // Вращение объекта вокруг его центра
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
        // Получаем направление от точки на объекте к его локальному верху
        Vector3 directionToUp = targetPosition - _rotatable.Transform.position;

        // Вычисляем угол между текущим направлением вверх и направлением к точке
        float angle = Vector3.Angle(_rotatable.Transform.forward, directionToUp);

        // Получаем ось вращения с использованием кросс-продукта
        Vector3 rotationAxis = Vector3.Cross(_rotatable.Transform.forward, directionToUp).normalized;

        // Создаем кватернион для вращения
        Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);

        // Применяем вращение
        _rotatable.Transform.rotation = rotation * _rotatable.Transform.rotation;
        EventController.Invoke(EventMessage.OnStopDice);
        // Восстанавливаем исходное направление вверха
        //transform.up = initialUpVector;
    }
}