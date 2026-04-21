using System;
using UnityEngine;

public abstract class TimeContextModule : IDisposable
{
    protected TimeContext TimeContext;

    protected TimeContextModule(TimeContext timeContext)
    {
        TimeContext = timeContext;
        TimeContext.ContextTimescale.OnValueChanged += OnTimescaleChanged;
        TimeContext.ContextTimescale.OnValueChangedWithHistory += OnTimescaleChangedWithHistory;
    }

    public void Dispose()
    {
        TimeContext.ContextTimescale.OnValueChanged -= OnTimescaleChanged;
        TimeContext.ContextTimescale.OnValueChangedWithHistory -= OnTimescaleChangedWithHistory;
    }
    public virtual void OnTimescaleChanged(float newScale) { }
    public virtual void OnTimescaleChangedWithHistory(float oldScale, float newScale) { }

}
public abstract class TimeContextModule<T> : TimeContextModule where T : Component
{
    protected T ReferencedComponent;
    protected TimeContextModule(T component, TimeContext timeContext) : base(timeContext)
    {
        ReferencedComponent = component;
    }
}

public class AnimatorContextModule : TimeContextModule<Animator>
{
    public AnimatorContextModule(Animator component, TimeContext timeContext) : base(component, timeContext) { }

    public override void OnTimescaleChanged(float newScale)
    {
        ReferencedComponent.speed = newScale;
    }
}

public class ParticleSystemContextModule : TimeContextModule<ParticleSystem>
{
    public ParticleSystemContextModule(ParticleSystem component, TimeContext timeContext) : base(component, timeContext) { }

    public override void OnTimescaleChanged(float newScale)
    {
        if (ReferencedComponent == null) return;
        var mainModule = ReferencedComponent.main;
        mainModule.simulationSpeed = newScale;
    }
}
