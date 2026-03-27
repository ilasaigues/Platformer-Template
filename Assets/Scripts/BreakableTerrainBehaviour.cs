using UnityEngine;

public class BreakableTerrainBehaviour : MonoBehaviour
{
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
        gameObject.SetActive(false);
    }

}
