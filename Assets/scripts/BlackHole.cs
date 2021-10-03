using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{

    int numCollidedObject;

    // Start is called before the first frame update
    void Start()
    {
        numCollidedObject = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other){
        Destroy(other.gameObject);
        numCollidedObject += 1;
        
        transform.localScale += new Vector3(numCollidedObject, numCollidedObject, 0);
    }
}
