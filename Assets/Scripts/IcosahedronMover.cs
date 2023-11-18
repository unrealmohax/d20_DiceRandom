using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class IcosahedronMover : IDisposable
{
    private readonly IMovableWithinBounds _movable;   // Reference to the IMovableWithinBounds interface
    private Vector3 _startPosition;                    // Initial position of the movable object
    private CoroutineTask _moveToPoint;                // Coroutine task for moving to a point
    private int lastWallIndex;                         // Index of the last chosen wall

    // Constructor to initialize the IcosahedronMover with an IMovableWithinBounds reference
    public IcosahedronMover(IMovableWithinBounds imovable)
    {
        _movable = imovable;
        EventController.AddListener<int>(EventMessage.OnRollDice, StartMove);
    }

    // Method to start the movement when the dice is rolled
    private void StartMove(int randomIndex)
    {
        _startPosition = _movable.MovableTransform.position;
        CoroutineController.StartCoroutine(MoveRoutine(), "MoveRoutine");
    }

    // Coroutine for continuous movement within the bounds
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

    // Method to stop the movement
    public void StopMove()
    {
        _moveToPoint?.Stop();
        CoroutineController.StartCoroutine(MoveToPoint(_startPosition));
    }

    // Coroutine to move to a specific target position
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

    // Method to get a random wall (enum value)
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

    // Method to calculate a new position based on the chosen wall
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

    // Method to dispose of the listener when the object is destroyed
    public void Dispose()
    {
        EventController.RemoveListener<int>(EventMessage.OnRollDice, StartMove);
    }

    // Enum representing the possible walls
    enum Wall
    {
        right,
        left,
        bot,
        top,
    }
}