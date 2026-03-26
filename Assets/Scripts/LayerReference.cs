using UnityEngine;

public static class LayerReference
{
    public static LayerMask PlayerLayer => 1 << 7;
    public static LayerMask TerrainLayer => 1 << 8;
    public static LayerMask OneWayPlatformLayer => 1 << 9;
    public static LayerMask HazardLayer => 1 << 10;
    public static LayerMask BoulderLayer => 1 << 11;
    public static LayerMask TerrainAndBoulder => TerrainLayer | BoulderLayer;
    public static LayerMask TerrainAndPlayer => TerrainLayer | PlayerLayer;

}
