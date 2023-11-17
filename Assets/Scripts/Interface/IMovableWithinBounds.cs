using UnityEngine;

public interface IMovableWithinBounds 
{
    Transform MovableTransform { get; }
    Vector3 BottomLeftCornal { get; }
    Vector3 UpperRightCornal { get; }
    float MoveSpeed { get; }
    float MoveDuration { get; }
    void PlayCollisionEffect();
}