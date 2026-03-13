using UnityEngine;

public static class LayerReference
{
    public static LayerMask TerrainLayer => 1 << 8;
    public static LayerMask OneWayPlatformLayer => 1 << 9;
    public static LayerMask HazardLayer => 1 << 10;

}
