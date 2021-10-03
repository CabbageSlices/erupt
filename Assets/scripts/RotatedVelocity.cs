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

    public Collider2D collider;

    public float maxSpeed = 80;

    public bool isGrounded
    {
        get;
        private set;
    } = false;

    // Start is called before the first frame update
    private void Awake()
    {
        if (!rotateMe)
        {
            rotateMe = GetComponent<RotateMe>();
        }

        body = GetComponent<Rigidbody2D>();

        collider = GetComponent<Collider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        blackHole = GameObject.FindWithTag("blackHole");
        gameManager = GameObject.FindWithTag("gameManager").GetComponent<GameManager>();
    }

    void FixedUpdate()
    {
        isGrounded = updateIsGrounded();
    }

    void Update()
    {
        velocityInLocalSpace.y -= gameManager.currentGravity * Time.deltaTime;

        if (isGrounded && velocityInLocalSpace.y < -1)
        {
            velocityInLocalSpace.y = 0;
        }

        Vector2 velocityInWorldSpace = velocityInLocalSpace.x * rotateMe.right + velocityInLocalSpace.y * rotateMe.up;

        body.velocity = velocityInWorldSpace;
    }

    public bool updateIsGrounded()
    {
        if (!collider)
        {
            return false;
        }

        RaycastHit2D[] results = new RaycastHit2D[4];

        int numRez = collider.Cast(-rotateMe.up, results, 0.2f);

        for (int i = 0; i < numRez; ++i)
        {

            if (results[i].collider.gameObject.tag == "somethingToignore")
            {
                continue;
            }

            var rotator = results[i].collider.GetComponent<RotateMe>();
            Vector2 otherUpDirection = rotator.up;

            float dott = Vector2.Dot(results[i].normal, otherUpDirection);

            if (dott > 0.3)
            {
                return true;
            }
        }

        return false;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        var contact = other.GetContact(0);
        //collision happend on top, top is direction of black hole
        if (Vector2.Dot(contact.normal, other.gameObject.GetComponent<RotateMe>().up) > 0.3)
        {
            velocityInLocalSpace.y = 0;
            isGrounded = true;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        var contact = other.GetContact(0);
        //collision happend on top, top is direction of black hole
        if (Vector2.Dot(contact.normal, other.gameObject.GetComponent<RotateMe>().up) > 0.3)
        {
            isGrounded = true;
        }
    }
}
