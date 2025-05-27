using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerLocomotion : MonoBehaviour
{
    public Animator animator;

    public Transform spawnPoint;
    public Transform deadPoint;

    InputAction jump;
    InputAction fallThroughPlatform;

    public PlayerInput playerInput;
    public Rigidbody2D rb;

    public RaycastHit2D platform;

    public float speed = 5f;
    public float currJumpForce;
    public float stoppedTime;
    public float bigJump = 20f;
    public float jumpTimer;
    public float smallJump = 7f;
    public float jumpMultiplier;
    public float fastFallRange;
    public float firstJumpForce;
    public float secondJumpForce;
    public float percentage = 0;
    public float weight = 5f;
    public float decaySpeed = 3f;
    public float jumpSquatTime = .05f;
    public float fastFallGravityScale = 5f;
    public float bonusGrav;
    public float initSpeed;
    public float runSpeed = 14f;
    public float duration = .33f;
    public float bufferTime = .2f;
    float moveInput;
    float runElapsed;
    float stopElapsed;
    float runT;
    float stopT;
    float t;

    public CheckGround checkGround;

    public Transform checkSphereTransform;

    public bool isJumping;
    public bool isGrounded;
    public bool wasGrounded;
    public bool timeToBigJump;
    public bool facingRight;
    public bool timeToSmallJump;
    public bool wasHit;
    public bool shortHopping;
    public bool onPlatform;
    public bool currPlatformShouldBeEnabled;
    bool dropDownPlatform;
    public bool canContiueTimer;

    int maxJumps = 2;
    public int currJumps;

    public Collider2D currPlatform;

    public Vector2 hitVector;
    Vector2 vel;
    Vector2 startVel;

    public LayerMask ground;
    public LayerMask platfrm;

    // Start is called before the first frame update
    void Start()
    {
        currJumps = maxJumps;
        initSpeed = speed;
        jump = playerInput.actions["Jump"];
        fallThroughPlatform = playerInput.actions["FallThroughPlatform"];
        spawnPoint = GameObject.Find("SpawnPoint").transform;
        deadPoint = GameObject.Find("DeadZone").transform;

        transform.position = spawnPoint.position;
    }

    // Update is called once per frame
    void Update()
    {
        platform = Physics2D.Raycast(transform.position, Vector2.up, 1.1f, LayerMask.GetMask("Platform"));

        moveInput = playerInput.actions["Move"].ReadValue<float>();

        if (transform.position.y < deadPoint.position.y)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            transform.position = spawnPoint.position;
            percentage = 0;
        }

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            facingRight = moveInput > 0;
        }

        if (rb.velocity.x != 0)
        {
            animator.SetBool("NotMoving", false);
        }
        else
        {
            animator.SetBool("NotMoving", true);
        }
    
        if (platform)
        {
            if (checkSphereTransform.position.y < platform.transform.position.y)
            {
                isGrounded = false;
            }

            if (rb.velocity.y > 0 && !isGrounded)
            {
                animator.SetBool("IsGrounded", false);
            }
        }

        if (isGrounded)
        {
            shortHopping = false;
            isJumping = false;
            animator.SetBool("IsGrounded", true);
        }
        else
        {
            animator.SetBool("IsGrounded", false);
        }

        if (jump.triggered)
        {
            isJumping = true;
            jumpTimer = 0;
            animator.SetTrigger("Jump");

            if (currJumps == 1)
            {
                animator.ResetTrigger("Jump");
                animator.SetTrigger("Jump");
                animator.Play("penguin_jump", -1, 0f);
                currJumps--;
                Jump(secondJumpForce);
            }
        }

        if (jump.IsPressed() && checkGround.offGroundButCanJump || playerInput.actions["Jump"].triggered && checkGround.offGroundButCanJump)
        {
            Jump(secondJumpForce);
        }

        if (rb.velocity.y < Mathf.Abs(.2f) && !isGrounded)
        {
            canContiueTimer = false;
            jumpTimer = 0;
        }
        else if (rb.velocity.y == 0 && isGrounded && playerInput.actions["Jump"].WasPressedThisFrame())
            canContiueTimer = true;

        if (jump.IsPressed())
        {
            if (jumpTimer <= jumpSquatTime + .1f && canContiueTimer)
            {
                jumpTimer += Time.deltaTime;
            }

            isJumping = true;

            if (jumpTimer > jumpSquatTime && isJumping && isGrounded)
            {
                animator.SetTrigger("Jump");
                isJumping = false;
                timeToBigJump = true;
                jumpTimer = 0;
            } else
            {
                timeToBigJump = false;
            }
        }

        if (facingRight)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else
        {
            transform.localScale = new Vector2(-1, 1);
        }

        if (jump.WasReleasedThisFrame() && canContiueTimer)
        {
            jumpTimer = 0;

            if (jumpTimer <= jumpSquatTime)
            {
                jumpTimer = 0;
                isJumping = true;
                timeToSmallJump = true;
                shortHopping = true;
            }
            else
            {
                timeToSmallJump = false;
                timeToBigJump = false;
                shortHopping = false;
            }
        }

        if (timeToBigJump && currJumps == 2)
        {
            timeToBigJump = false;
            currJumps--;
            jumpTimer = 0;
            Jump(bigJump);
        }
        else if (timeToSmallJump && currJumps == 2)
        {
            timeToSmallJump = false;
            shortHopping = true;
            isJumping = true;
            currJumps--;
            jumpTimer = 0;
            Jump(smallJump);
        }

        if (playerInput.actions["Small Hop"].triggered)
        {
            Jump(smallJump);
        }

        if (currPlatform && fallThroughPlatform.triggered && isGrounded)
        {
            StartCoroutine(DisablePlatformCollision());
        }

        if (!isGrounded && rb.velocity.y < Mathf.Abs(fastFallRange) || !isGrounded && rb.velocity.y < 0)
        {
            if (playerInput.actions["Fast Fall"].triggered)
            {
                rb.velocity = new Vector2(rb.velocity.x, fastFallGravityScale * -9.81f);
            }
        }

        if (!isGrounded)
        {
            vel = rb.velocity;
            vel.y -= bonusGrav * Time.deltaTime;

            rb.velocity = vel;
        }

        if (moveInput > .5f || moveInput < -.5f)
        {
            runElapsed = 0;
            runElapsed += Time.deltaTime;
            runT = runElapsed / duration;
            speed = Mathf.Lerp(speed, runSpeed, runT);

            if (runSpeed - speed < .2f)
                speed = runSpeed;
        } else
        {
            stopElapsed = 0;
            stopElapsed += Time.deltaTime;
            stopT = stopElapsed / duration;
            speed = Mathf.Lerp(speed, initSpeed, stopT);

            if (speed - initSpeed < .2f)
                speed = initSpeed;
        }
    }

    private IEnumerator DisablePlatformCollision()
    {
        // Temporarily disable collision
        Physics2D.IgnoreCollision(this.GetComponent<CapsuleCollider2D>(), currPlatform, true);
        yield return new WaitForSeconds(0.5f); // time for the player to fall through
        Physics2D.IgnoreCollision(this.GetComponent<CapsuleCollider2D>(), currPlatform, false);
    }

    private void Jump(float jumpForce)
    {
        if (jumpForce == bigJump)
            StartCoroutine(nameof(JumpSquat));
        else if (jumpForce == smallJump)
            shortHopping = true;

        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        if (jumpForce == secondJumpForce && rb.velocity.y < 0)
        {
            jumpForce += Mathf.Abs(rb.velocity.y);
        }

        jumpTimer = 0;
    }

    private IEnumerator JumpSquat()
    {
        yield return new WaitForSeconds(jumpSquatTime);
    }

    private void FixedUpdate()
    {
        if (!wasHit)
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    public void ApplyKnockback(Vector2 knockback)
    {
        StopAllCoroutines(); // stop any previous decay
        StartCoroutine(HandleKnockback(knockback));
    }

    private IEnumerator HandleKnockback(Vector2 knockback)
    {
        wasHit = true;
        rb.velocity = new Vector2(knockback.x, rb.velocity.y );
        /*
        if (!isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        */
        t = 0f;
        startVel = rb.velocity;

        while (t < 1f)
        {
            t += Time.deltaTime * decaySpeed;
            rb.velocity = Vector2.Lerp(startVel, new Vector2(0, rb.velocity.y), t);
            yield return null;
        }

        rb.velocity = new Vector2(0, rb.velocity.y);
        wasHit = false;

        if (isGrounded)
        {
            currJumps = 2;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if (collision.CompareTag("Hitbox") && collision.transform.parent != this.transform)
        {
            var dir = (collision.transform.parent.GetComponent<Attack>().facingRight ? Vector2.left : Vector2.right) * (collision.GetComponent<DetectHit>().knockbackFormula);

            Debug.Log(collision.transform.parent == this.transform);
            Debug.Log("Was hit");

            //rb.AddForce(dir, ForceMode2D.Impulse);
            rb.AddForce(Vector2.left * 10, ForceMode2D.Impulse);
        }
        */
    }
}
