using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RotatedVelocity))]
public class Player : MonoBehaviour
{
    public Vector2 inputDirection;
    bool jumpPressed = false;

    public float speed = 15;
    public float jumpSpeed = 15;

    public GameObject blackHole;
    public GameManager gameManager;
    public RotateMe rotateMe;

    public Rigidbody2D body;

    RotatedVelocity rotatedVelocity;

    bool isGrounded = false;

    private void Awake()
    {
        if (!rotateMe)
        {
            rotateMe = GetComponent<RotateMe>();
        }

        rotatedVelocity = GetComponent<RotatedVelocity>();
        body = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        blackHole = GameObject.FindWithTag("blackHole");
        gameManager = GameObject.FindWithTag("gameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        rotatedVelocity.velocityInLocalSpace.x = inputDirection.x * speed;
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
    }

    public void onJump()
    {
        if (isGrounded)
        {

            rotatedVelocity.velocityInLocalSpace.y = jumpSpeed;
            isGrounded = false;
            rotateMe.isRotationDisabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        var contact = other.GetContact(0);
        //collision happend on top, top is direction of black hole
        if (Vector2.Dot(contact.normal, other.gameObject.GetComponent<RotateMe>().toBlackhole) > 0.5)
        {

            transform.rotation = other.transform.rotation;
            // transform.position = other.transform.position + other.collider.bounds.extents.y * new Vector3(contact.normal.x, contact.normal.y, 0);
            rotatedVelocity.velocityInLocalSpace.y = 0;
            isGrounded = true;
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
