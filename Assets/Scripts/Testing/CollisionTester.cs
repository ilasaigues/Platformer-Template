using System;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[RequireComponent(typeof(CollisionController))]
public class CollisionTester : MonoBehaviour
{

    public Vector2 movementDirection;

    public LayerMask GroundLayerMask;
    CollisionController _collisionController;
    public float castDistance;

    bool correctionNeeded;
    void Awake()
    {
        _collisionController = GetComponent<CollisionController>();
    }

    void Update()
    {
        correctionNeeded = false;
        var boxBounds = GetComponent<Collider2D>().bounds;
        castDistance = Mathf.Clamp(castDistance,0,boxBounds.extents.x);
        Vector2 size = new Vector2(castDistance,boxBounds.size.y);
        var hits = Physics2D.BoxCastAll(boxBounds.center,size,0,Vector2.right,boxBounds.extents.x,GroundLayerMask);
        if(hits.Count() != 0)
        {
            foreach(RaycastHit2D hit in hits)
            {
                float treshold = 1-(castDistance/boxBounds.size.x);
                float correction = (treshold-hit.fraction)/2;
                if(hit.fraction < treshold)
                {
                    correctionNeeded = true;
                }
            }      
        }  
    }

    void OnDrawGizmos()
    {
        var boxBounds = GetComponent<Collider2D>().bounds;     
        Vector2 origin = (Vector2)boxBounds.center + Vector2.right * boxBounds.size.x/2;
        Vector2 size = new Vector2(castDistance,boxBounds.size.y);
        Gizmos.color = correctionNeeded ? Color.red : Color.green;
        Gizmos.DrawWireCube(origin,size);
    }
}
