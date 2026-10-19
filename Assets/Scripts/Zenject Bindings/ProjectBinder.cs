using UnityEngine;
using Zenject;

public class ProjectBinder : MonoInstaller
{
    public TimeContext TimeContext;
    public VFXSpawner VFXSpawner;
    public GameInputHandler GameInputHandler;
    public SceneTransitionManager SceneTransitionManager;
    public WorldManager WorldManager;
    public UIInputHandler UIInputHandler;
    public AudioManager AudioManager;

    public ApplicationStateManager ApplicationStateManager;

    public override void InstallBindings()
    {
        QuickBind(TimeContext);
        QuickBind(VFXSpawner);
        QuickBind(GameInputHandler);
        QuickBind(SceneTransitionManager);
        QuickBind(WorldManager);
        QuickBind(UIInputHandler);
        QuickBind(ApplicationStateManager);
        QuickBind(AudioManager);
        Container.BindFactory<VFXObject, VFXObject.Factory>().FromComponentInNewPrefab(VFXSpawner.VFXObjectPrefab);
    }

    private void QuickBind<T>(T instance)
    {
        Container.Bind<T>().FromInstance(instance).AsSingle();
    }

}
