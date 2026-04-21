using System;
using System.Collections.Generic;
using UnityEngine;

public class TimeContext : MonoBehaviour
{
    // floatReference for context timescale
    public FloatValue ContextTimescale;

    // getter for fixed and delta times
    public float DeltaTime => (ContextTimescale ? ContextTimescale.Value : 1) * UnityEngine.Time.deltaTime;
    public float FixedDeltaTime => (ContextTimescale ? ContextTimescale.Value : 1) * UnityEngine.Time.fixedDeltaTime;

    public bool Paused => ContextTimescale ? ContextTimescale.Value == 0 : false;

    [HideInInspector]
    public float Time;

    public static Dictionary<Type, Func<Component, TimeContext, TimeContextModule>> ModuleFactoryMap = new()
    {
        {typeof(Animator),(c,tc)=>new AnimatorContextModule((Animator)c,tc)},
        {typeof(ParticleSystem),(c,tc)=>new ParticleSystemContextModule((ParticleSystem)c,tc)},
    };


    public Dictionary<Type, List<TimeContextModule>> StoredModules = new();

    void Start()
    {
        CreateContextModules();
    }

    void OnDestroy()
    {
        foreach (var moduleList in StoredModules.Values)
        {
            foreach (var module in moduleList)
            {
                module.Dispose();
            }
        }
    }

    void Update()
    {
        Time += DeltaTime;
    }

    // automatic assignment of wrapper classes to child components that require time handling
    public void CreateContextModules()
    {
        foreach (var managableType in ModuleFactoryMap.Keys)
        {
            var components = GetComponentsInChildren(managableType, true);
            foreach (var component in components)
            {
                if (component == null) continue;
                if (!StoredModules.ContainsKey(managableType))
                {
                    StoredModules.Add(managableType, new());
                }
                StoredModules[managableType].Add(ModuleFactoryMap[managableType](component, this));
            }
        }
    }

}
