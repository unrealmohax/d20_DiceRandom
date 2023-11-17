using UnityEngine;

public interface IRotatable 
{
    Transform Transform { get; }
    AnimationCurve RotationSpeedCurve { get; }
    float SpeedRotation { get; }
    float RotationDuration { get; }

}