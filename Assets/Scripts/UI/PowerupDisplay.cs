using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PowerupDisplay : MonoBehaviour
{
    public Animator Animator { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Animator = gameObject.GetOrAddComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
