using UnityEngine;

public class ColliderController : MonoBehaviour
{
    public Bounds ColliderBounds => _capsuleCollider.bounds;

    [SerializeField]
    private CapsuleCollider2D _capsuleCollider;

    [SerializeField]
    private BoxCollider2D _topCollider;

    [SerializeField]
    private BoxCollider2D _bottomCollider;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
