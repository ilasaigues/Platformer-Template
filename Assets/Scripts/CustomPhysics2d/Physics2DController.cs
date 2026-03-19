using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Physics2DController : MonoBehaviour
{
    private static Physics2DController _instance;

    public static Physics2DController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject("Physics2DController").AddComponent<Physics2DController>();
            }
            return _instance;
        }
    }

    List<IPhysics2DObject> _physics2DObjects = new();

    public void Subscribe(IPhysics2DObject physObject)
    {
        if (physObject != null && !_physics2DObjects.Contains(physObject))
        {
            _physics2DObjects.Add(physObject);
        }
    }

    public void Unsubscribe(IPhysics2DObject physObject)
    {
        _physics2DObjects.Remove(physObject);
    }


    void FixedUpdate()
    {
        _physics2DObjects = _physics2DObjects.OrderBy(o => o.Priority).ToList();

        foreach (var physObject in _physics2DObjects)
        {
            physObject.SimulatePhisics2D();
        }

    }

}

public interface IPhysics2DObject
{
    public int Priority { get; }
    public void SimulatePhisics2D();
}