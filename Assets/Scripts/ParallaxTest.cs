using UnityEngine;

public class ParallaxTest : MonoBehaviour
{
    public Transform target;
    public float parallaxFactor;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y,0) * parallaxFactor;
    }
}
