using LDtkUnity;
using UnityEngine;

public class BreakableTerrainBehaviour : MonoBehaviour, ILDtkImportedFields
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

    public void OnLDtkImportFields(LDtkFields fields)
    {
        transform.localScale = Vector3.one;
        switch (fields.GetEnum<Orientation>("direction"))
        {
            case Orientation.Up:
            transform.rotation = Quaternion.Euler(Vector3.zero);
            break;
            case Orientation.Down:
            transform.rotation = Quaternion.Euler(new Vector3(0,0,180));
            break;
            case Orientation.Left:
            transform.rotation = Quaternion.Euler(new Vector3(0,0,90));
            break;
            case Orientation.Right:
            transform.rotation = Quaternion.Euler(new Vector3(0,0,270));
            break;
        }
    }
}
