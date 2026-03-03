using UnityEngine;

public static class LayerReference
{
    public static LayerMask TerrainLayer => 1 << 8;
    public static LayerMask OneWayPlatformLayer => 1 << 9;

}
