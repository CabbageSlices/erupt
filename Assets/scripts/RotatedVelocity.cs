using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RotateMe))]
[RequireComponent(typeof(Rigidbody2D))]
public class RotatedVelocity : MonoBehaviour
{
    public GameObject blackHole;
    public GameManager gameManager;
    public RotateMe rotateMe;

    public Rigidbody2D body;

    public Vector2 velocityInLocalSpace;

    public float maxSpeed = 50;

    public Vector2 right
    {
        get
        {

            return new Vector2(transform.right.x, transform.right.y).normalized;
        }
    }

    public Vector2 up
    {
        get
        {
            return new Vector2(transform.up.x, transform.up.y).normalized;
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        if (!rotateMe)
        {
            rotateMe = GetComponent<RotateMe>();
        }

        body = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        blackHole = GameObject.FindWithTag("blackHole");
        gameManager = GameObject.FindWithTag("gameManager").GetComponent<GameManager>();
    }

    void Update()
    {

        velocityInLocalSpace.y -= gameManager.currentGravity * Time.deltaTime;
        Vector2 velocityInWorldSpace = velocityInLocalSpace.x * right + velocityInLocalSpace.y * up;

        if (velocityInLocalSpace.magnitude > maxSpeed)
        {
            velocityInLocalSpace = velocityInLocalSpace.normalized * maxSpeed;
        }
        body.velocity = velocityInWorldSpace;
    }
}
