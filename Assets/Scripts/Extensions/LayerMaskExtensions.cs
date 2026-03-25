using UnityEngine;

public static class LayerMaskExtensions
{
    public static ContactFilter2D ToContactFilter2D(this LayerMask value)
    {
        return new ContactFilter2D()
        {
            useLayerMask = true,
            layerMask = value,
        };
    }
}