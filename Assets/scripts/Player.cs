using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RotatedVelocity))]
public class Player : MonoBehaviour
{
    public Vector2 inputDirection;

    bool jumpPressed = false;
    bool jumpHeld = false;

    float timeJumpHoldStarted;

    public float maxTimeJumpCanBeHeld = 0.5f;

    public float speed = 15;
    public float jumpSpeed = 15;

    public GameObject blackHole;
    public GameManager gameManager;
    public RotateMe rotateMe;
    public Collider2D collider;

    public Rigidbody2D body;

    RotatedVelocity rotatedVelocity;

    private void Awake()
    {
        if (!rotateMe)
        {
            rotateMe = GetComponent<RotateMe>();
        }

        collider = GetComponent<Collider2D>();

        rotatedVelocity = GetComponent<RotatedVelocity>();
        body = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        blackHole = GameObject.FindWithTag("blackHole");
        gameManager = GameObject.FindWithTag("gameManager").GetComponent<GameManager>();
    }

    private void FixedUpdate()
    {
        rotateMe.isRotationDisabled = matchRotationOfGround();
    }

    // Update is called once per frame
    void Update()
    {
        rotatedVelocity.velocityInLocalSpace.x = inputDirection.x * speed;

        if ((jumpPressed && rotatedVelocity.isGrounded) || (jumpHeld && timeJumpHoldStarted + maxTimeJumpCanBeHeld > Time.time))
        {
            rotatedVelocity.velocityInLocalSpace.y = jumpSpeed;

            if (!jumpHeld)
            {
                jumpHeld = true;
                timeJumpHoldStarted = Time.time;
            }

            jumpPressed = false;
        }
    }

    public void onMovePressed(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>().normalized;
    }

    public void onJumpPressed(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onJump();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            jumpHeld = false;
            jumpPressed = false;
        }
    }

    public void onJump()
    {
        if (!rotatedVelocity)
        {
            rotatedVelocity = GetComponent<RotatedVelocity>();
        }

        if (rotatedVelocity.isGrounded || (jumpHeld && timeJumpHoldStarted + maxTimeJumpCanBeHeld < Time.time))
        {
            jumpPressed = true;
        }
    }

    public bool matchRotationOfGround()
    {
        RaycastHit2D[] results = new RaycastHit2D[3];

        int numRez = collider.Cast(-rotateMe.up, results, 0.2f);

        for (int i = 0; i < numRez; ++i)
        {

            if (results[i].collider.gameObject.tag == "somethingToignore")
            {
                continue;
            }

            var rotator = results[i].collider.GetComponent<RotateMe>();
            Vector2 otherUpDirection = rotator.up;

            if (Vector2.Dot(results[i].normal, otherUpDirection) > 0.3)
            {
                transform.rotation = rotator.transform.rotation;
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

            transform.rotation = other.transform.rotation;
            rotateMe.isRotationDisabled = true;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        var contact = other.GetContact(0);
        //collision happend on top, top is direction of black hole
        if (Vector2.Dot(contact.normal, other.gameObject.GetComponent<RotateMe>().up) > 0.3)
        {

            transform.rotation = other.transform.rotation;
            rotateMe.isRotationDisabled = true;
        }
    }

    // private void OnCollisionExit2D(Collision2D other)
    // {

    //     var contact = other.GetContact(0);
    //     //collision happend on top, top is direction of black hole
    //     if (Vector2.Dot(contact.normal, rotateMe.toBlackhole) > 0.5)
    //     {

    //         transform.rotation = other.transform.rotation;
    //         // transform.position = other.transform.position + other.collider.bounds.extents.y * new Vector3(contact.normal.x, contact.normal.y, 0);
    //         Debug.Log("POINT: " + contact.point);
    //         velocityInLocalSpace.y = 0;
    //         isGrounded = true;
    //         rotateMe.isRotationDisabled = true;
    //     }
    // }
}
