using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int numRegionsForRotation = 8;

    public float initialGravity = 8;

    public float currentGravity = 8;

    public float rotationRegionSizeInDegrees
    {
        get
        {
            return 360.0f / numRegionsForRotation;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        currentGravity = initialGravity;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
