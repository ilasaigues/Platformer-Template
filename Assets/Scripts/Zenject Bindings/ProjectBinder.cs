using UnityEngine;
using Zenject;

public class ProjectBinder : MonoInstaller
{
    public TimeContext TimeContext;
    public VFXSpawner VFXSpawner;
    public GameInputHandler GameInputHandler;
    public GameManager GameManager;

    public override void InstallBindings()
    {
        QuickBind(TimeContext);
        QuickBind(VFXSpawner);
        QuickBind(GameInputHandler);
        QuickBind(GameManager);
        Container.BindFactory<VFXObject, VFXObject.Factory>().FromComponentInNewPrefab(VFXSpawner.VFXObjectPrefab);
    }

    private void QuickBind<T>(T instance)
    {
        Container.Bind<T>().FromInstance(instance).AsSingle();
    }

}
