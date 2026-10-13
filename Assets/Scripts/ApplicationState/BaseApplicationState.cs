using UnityEngine;

public abstract class BaseApplicationState
{
    virtual public void OnEnterState() { }
    virtual public void OnExitState() { }
    virtual public void DoUpdate() { }
}

public class RootMenuState : BaseApplicationState
{
    public override void OnEnterState()
    {
        // disable game inputs
        // find the topmost ui and focus on it
    }
}
public class GameRunningState : BaseApplicationState
{
    public override void OnEnterState()
    {
        // enable game inputs
        // close all UI that should be closed
        // 
    }
}