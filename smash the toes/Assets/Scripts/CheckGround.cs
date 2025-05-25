using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheckGround : MonoBehaviour
{
    public bool ballsack = true;
    public float farts = 6f;
    public int quantummechanics = 1;
    public PlayerLocomotion playerLocomotion;
    PlayerInput playerInput;
    public float offGroundTimer;
    public bool offGroundButCanJump = true;
    public float offGroundMaxTime = .2f;
    bool startTimer;

    //private void Update()
    //{
    // if (ballsack == true && farts * quantummechanics == 6)
    //{
    // Transform.Destroy(GameObject.Destroy(GameObject.Destroy("deezNuts")));

    //        }
    //  }

    private void Start()
    {
        playerInput = playerLocomotion.playerInput;
    }

    private void Update()
    {
        if (startTimer)
        {
            offGroundTimer += Time.deltaTime;
        }

        if (offGroundTimer < offGroundMaxTime && startTimer)
        {
            offGroundButCanJump = true;
        }
        else
        {
            startTimer = false;
            offGroundButCanJump = false;
            offGroundTimer = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            playerLocomotion.isGrounded = true;
            playerLocomotion.currJumps = 2;

            playerLocomotion.timeToSmallJump = false;

            offGroundButCanJump = false;
            startTimer = false;
        }
        else if (collision.gameObject.layer == 7 && transform.parent.position.y - 1 > collision.transform.position.y)
        {
            playerLocomotion.onPlatform = true;
            playerLocomotion.currPlatform = collision.GetComponent<BoxCollider2D>();

           // playerLocomotion.isGrounded = true;
            //playerLocomotion.currJumps = 2;

            playerLocomotion.timeToSmallJump = false;
            playerLocomotion.timeToBigJump = false;

            offGroundButCanJump = false;
            startTimer = false;

            if (playerInput.actions["Fast Fall"].triggered)
            {
                collision.enabled = false;
            }

            if (playerLocomotion.rb.velocity.y > 0 && !playerLocomotion.isGrounded)
            {
                playerLocomotion.isGrounded = false;
                playerLocomotion.animator.SetBool("IsGrounded", false);
            }
        }

        if (collision.gameObject.layer == 7 && playerLocomotion.rb.velocity.y < 0 || collision.gameObject.layer == 7 && playerLocomotion.rb.velocity.y == 0)
        {
            playerLocomotion.isGrounded = true;
            playerLocomotion.currJumps = 2;

            playerLocomotion.timeToSmallJump = false;
            playerLocomotion.timeToBigJump = false;

            offGroundButCanJump = false;
            startTimer = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 && playerLocomotion.rb.velocity.y < 0)
        {
            startTimer = true;
            playerLocomotion.isGrounded = false;
        }
        else if (collision.gameObject.layer == 6)
        {
            playerLocomotion.isGrounded = false;
        }
        else if (collision.gameObject.layer == 7)
            playerLocomotion.isGrounded = false;
    }
}
