using UnityEngine;

public class BreakableTerrainBehaviour : MonoBehaviour
{
    public ParticleSystem BreakParticles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Break()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        BreakParticles.Play();
    }

}
