using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class IcosahedronMover : IDisposable
{
    private readonly IMovableWithinBounds _movable;
    private Vector3 _startPosition;
    private CoroutineTask _moveToPoint;
    private int lastWallIndex;

    public IcosahedronMover(IMovableWithinBounds imovable)
    {
        _movable = imovable;
        EventController.AddListener<int>(EventMessage.OnRollDice, StartMove);
    }

    private void StartMove(int randomIndex)
    {
        _startPosition = _movable.MovableTransform.position;
        CoroutineController.StartCoroutine(MoveRoutine(), "MoveRoutine");
    }

    private IEnumerator MoveRoutine() 
    {
        float time = 0;
        while (time < _movable.MoveDuration)
        {
            yield return null;
            time += Time.deltaTime;

            if (_moveToPoint == null) 
            {
                Vector3 targetPosition = GetNewPositionMove(GetRandomEnumValue());
                _moveToPoint = CoroutineController.StartCoroutine(MoveToPoint(targetPosition));
            }
        }

        StopMove();
    }

    public void StopMove()
    {
        _moveToPoint?.Stop();
        CoroutineController.StartCoroutine(MoveToPoint(_startPosition));
    }

    private IEnumerator MoveToPoint(Vector3 targetPosition)
    {
        while (Vector3.Distance(_movable.MovableTransform.position, targetPosition) > 0.1f)
        {
            _movable.MovableTransform.position = Vector3.MoveTowards(_movable.MovableTransform.position, targetPosition, _movable.MoveSpeed * Time.deltaTime);
            yield return null;
        }

        _movable.PlayCollisionEffect();
        _moveToPoint = null;
    }

    private Wall GetRandomEnumValue()
    {
        Array values = Enum.GetValues(typeof(Wall));
        int randomIndex = 0;
        do 
        {
            randomIndex = Random.Range(0, values.Length);
        } 
        while (randomIndex == lastWallIndex);
        lastWallIndex = randomIndex;

        return (Wall)values.GetValue(randomIndex);
    }

    private Vector3 GetNewPositionMove(Wall wall)
    {
        Vector3 newPosition = Vector3.zero;
        switch (wall)
        {
            case Wall.left:
                newPosition = new Vector3(_movable.BottomLeftCornal.x, Random.Range(_movable.BottomLeftCornal.y, _movable.UpperRightCornal.y), 0);
                break;

            case Wall.right:
                newPosition = new Vector3(_movable.UpperRightCornal.x, Random.Range(_movable.BottomLeftCornal.y, _movable.UpperRightCornal.y), 0);
                break;

            case Wall.top:
                newPosition = new Vector3(Random.Range(_movable.BottomLeftCornal.x, _movable.UpperRightCornal.x), _movable.UpperRightCornal.y, 0);
                break;

            case Wall.bot:
                newPosition = new Vector3(Random.Range(_movable.BottomLeftCornal.x, _movable.UpperRightCornal.x), _movable.BottomLeftCornal.y, 0);
                break;
        }
        return newPosition;
    }

    public void Dispose()
    {
        EventController.RemoveListener<int>(EventMessage.OnRollDice, StartMove);
    }

    enum Wall 
    {
        right,
        left,
        bot,
        top,
    }
}