using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{

    int numCollidedObject;

    public Vector3 initialScale;

    public AudioClip onDestroySfx;

    public AudioSource audioPlayer;

    // Start is called before the first frame update
    void Start()
    {
        numCollidedObject = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        audioPlayer = GetComponent<AudioSource>();
        initialScale = transform.localScale;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(other.gameObject);
        numCollidedObject += 1;
        audioPlayer.PlayOneShot(onDestroySfx);

        transform.localScale += new Vector3(numCollidedObject, numCollidedObject, 0);
    }

    public void reset()
    {
        numCollidedObject = 0;
        transform.localScale = initialScale;
    }
}
