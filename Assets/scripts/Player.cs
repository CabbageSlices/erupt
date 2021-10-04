using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    float horizontalInputDirectionScale = 1;

    public GameObject blackHole;
    public GameManager gameManager;
    public RotateMe rotateMe;
    public Collider2D collider;

    public Rigidbody2D body;

    RotatedVelocity rotatedVelocity;

    public Player guyWhoSquishedMe = null;

    public bool squished = false;

    public float timeBecameSquished = 0;
    public float howLongToStaySquished = 4f;

    public float originalScale = 1;

    public int playerNumber = 0;

    public SpriteRenderer sprite;

    public Text numberDisplay;

    private void Awake()
    {
        if (!rotateMe)
        {
            rotateMe = GetComponent<RotateMe>();
        }

        collider = GetComponent<Collider2D>();

        rotatedVelocity = GetComponent<RotatedVelocity>();
        body = GetComponent<Rigidbody2D>();

        originalScale = transform.localScale.y;

        sprite = GetComponent<SpriteRenderer>();
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

    public void setPlayerNumber(int num)
    {

        playerNumber = num;
        numberDisplay.text = num.ToString();
    }

    // Update is called once per frame
    void Update()
    {

        float actualSpeed = squished ? speed * 0.1f : speed;

        //want the right button to always go towards the right
        rotatedVelocity.velocityInLocalSpace.x = inputDirection.x * actualSpeed;

        if (!squished)
        {
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
        else
        {
            if (timeBecameSquished + howLongToStaySquished < Time.time)
            {
                doUnsquish();
            }
        }

        //don't flip if velocity is equal to zero, that way he reminas facing same dfriectio nwhen he sotps moving

        if (rotatedVelocity.velocityInLocalSpace.x < 0)
        {
            sprite.flipX = false;
        }

        if (rotatedVelocity.velocityInLocalSpace.x > 0)
        {
            sprite.flipX = true;
        }
    }

    public void onMovePressed(InputAction.CallbackContext context)
    {
        if (!rotateMe)
        {
            rotateMe = GetComponent<RotateMe>();
        }
        bool wasThereHorizontalMovementBefore = true;

        if (inputDirection.x == 0)
        {
            wasThereHorizontalMovementBefore = false;
        }
        inputDirection = context.ReadValue<Vector2>().normalized;

        if (inputDirection.x == 0)
        {
            return;
        }

        if (!wasThereHorizontalMovementBefore)
        {
            //horizontal movment just started, be sure to save direction of up vector so we can flip velocites as he moves to allwo for smooth movmeont
            horizontalInputDirectionScale = rotateMe.up.y >= 0 ? 1 : -1;

            if (Mathf.Abs(rotateMe.up.y) < 0.001 && rotateMe.right.y < 0)
            {
                horizontalInputDirectionScale = 1;
            }

            float angleOfRight = Mathf.Atan2(rotateMe.right.y, rotateMe.right.x) * Mathf.Rad2Deg;
            if (angleOfRight <= -90 && angleOfRight > -140)
            {
                horizontalInputDirectionScale = 1;
            }
        }

        inputDirection.x *= horizontalInputDirectionScale;
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
        jumpPressed = true;
    }

    public bool matchRotationOfGround()
    {
        RaycastHit2D[] results = new RaycastHit2D[3];

        int numRez = collider.Cast(-rotateMe.up, results, 0.3f);

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

            //player and other guy should be moving towards each other
            if (other.gameObject.tag == "player")
            {
                Vector2 otherVelocity = other.gameObject.GetComponent<RotatedVelocity>().velocityInLocalSpace;
                if (rotatedVelocity.velocityInLocalSpace.y - otherVelocity.y <= 0)
                {
                    other.gameObject.GetComponent<Player>().onSquish(this);
                }
            }
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

    public void onSquish(Player squisher)
    {
        if (!squished)
        {
            squished = true;
            guyWhoSquishedMe = squisher;

            doSquish();
        }
    }

    public void doSquish()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.3f, transform.localScale.z);
        timeBecameSquished = Time.time;
    }

    public void doUnsquish()
    {
        transform.localScale = new Vector3(transform.localScale.x, originalScale, transform.localScale.z);
        squished = false;
        guyWhoSquishedMe = null;
    }
}

