using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[InitializeOnLoad]
public static class AssetResetManager
{
    static AssetResetManager()
    {
        EditorApplication.playModeStateChanged += HandlePlayModeChanged;
    }

    private static void HandlePlayModeChanged(PlayModeStateChange playModeStateChange)
    {
        Action<IEditorResettable> resetter = null;
        if (playModeStateChange == PlayModeStateChange.ExitingPlayMode)
        {
            resetter = r => r.OnExitPlaymode();
        }
        else if (playModeStateChange == PlayModeStateChange.EnteredPlayMode)
        {
            resetter = r => r.OnEnterPlaymode();
        }
        if (resetter != null)
        {
            ApplyReset(resetter);
        }
    }

    private static void ApplyReset(Action<IEditorResettable> resetter)
    {
        List<IEditorResettable> resetAssets = AssetDatabase.FindAssets("t:ScriptableObject")
                                                .Select(guid => AssetDatabase.LoadAssetByGUID(new GUID(guid), typeof(IEditorResettable)))
                                                .OfType<IEditorResettable>()
                                                .ToList();

        if (resetAssets.Count == 0)
        {
            return;
        }

        foreach (var asset in resetAssets)
        {
            resetter(asset);
        }
    }
}
#endif