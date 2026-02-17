using System;
using UnityEngine;
public class BehaviourChangeRequest
{
    public Type NewBehaviourType;
}

public abstract class BaseBehaviour
{

    public abstract void Enter(); // When entering the behaviour
    public abstract void Exit(); // When exiting the behaviour

    /// <summary>
    /// Returns null if the behaviour should remain, returns a change request if it should change.
    /// </summary>
    /// <returns></returns>
    public abstract BehaviourChangeRequest VerifyBehaviour();

    public abstract void Update(float delta);
    public abstract void FixedUpdate(float delta);

}

