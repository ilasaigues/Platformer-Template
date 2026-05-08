using UnityEngine;

public class FollowHorizontal : MonoBehaviour
{

    public Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            var pos = transform.position;
            pos.x = target.position.x;
            transform.position = pos;
        }
    }
}
