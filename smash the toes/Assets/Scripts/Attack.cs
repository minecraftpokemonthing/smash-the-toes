using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    public PlayerInput playerInput;
    public AttackData jab;
    public AttackData neutralSpecial;
    public Animator animator;
    public AttackData currAttack;
    public bool facingRight;
    bool canAttack = true;
    public float finalDamage;
    PlayerLocomotion playerLocomotion;
    public GameObject exclamationMark;
    float moveInput;
    Vector3 hitboxPos;
    GameObject hitbox;
    InitializeHitbox initializeHitbox;
    public float bufferTimer;
    public float maxBufferTime;
    InputAction jabAction;
    InputAction neutralSpecialAction;
    InputAction currAction;
    public bool bufferHit;
    AttackData currBufferAttack;


    // Start is called before the first frame update
    void Start()
    {
        jabAction = playerInput.actions["Jab"];
        neutralSpecialAction = playerInput.actions["Neutral Special"];
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = playerInput.actions["Move"].ReadValue<float>();

        if (Mathf.Abs(moveInput) > 0)
        {
            facingRight = moveInput > 0;
        }

        if (jabAction.triggered)
        {
            currAction = jabAction;
            currAttack = jab;
            PerformAttack();
            playerLocomotion.speed = 0;
        }
        else if (neutralSpecialAction.triggered)
        {
            currAction = neutralSpecialAction;
            currAttack = neutralSpecial;
            PerformAttack();
            playerLocomotion.speed /= 5;
        }

        if (!canAttack)
        {
            if (jabAction.triggered)
            {
                currBufferAttack = jab;
                bufferHit = true;
                bufferTimer = maxBufferTime;
            }
            else if (neutralSpecialAction.triggered)
            {
                currBufferAttack = neutralSpecial;
                bufferHit = true;
                bufferTimer = maxBufferTime;
            }
        }

        if (bufferHit)
            bufferTimer -= Time.deltaTime;

        if (bufferTimer <= 0)
        {
            bufferHit = false;
            bufferTimer = maxBufferTime;
            currBufferAttack = null;
        }

        if (canAttack)
        {
            if (currBufferAttack != null)
            {
                if (currBufferAttack == jab)
                {
                    currAction = jabAction;
                    currAttack = jab;
                    PerformAttack();
                    playerLocomotion.speed = 0;
                }
                else if (currBufferAttack == neutralSpecial)
                {
                    currAction = neutralSpecialAction;
                    currAttack = neutralSpecial;
                    PerformAttack();
                    playerLocomotion.speed /= 5;
                }

                currBufferAttack = null;
            }
        }
    }

    private void PerformAttack()
    {
        StartCoroutine(nameof(AttackRoutine));
    }

    IEnumerator AttackRoutine()
    {
        if (canAttack)
        {
            canAttack = false;
            playerLocomotion.canMove = false;
            playerLocomotion.rb.velocity = new Vector2(0, playerLocomotion.rb.velocity.y);
            playerLocomotion.rb.isKinematic = true;

            hitboxPos = new Vector3(
            (facingRight ? 1 : -1) * currAttack.offset.x,
            currAttack.offset.y,
            0);

            yield return new WaitForSeconds(currAttack.hitboxPrefab.GetComponent<InitializeHitbox>().startUp);

            hitbox = Instantiate(currAttack.hitboxPrefab, transform.position + hitboxPos, Quaternion.identity, this.transform);

            animator.SetTrigger("Attack");

            if (currAttack == neutralSpecial)
            {
                exclamationMark.SetActive(true);
            }

            initializeHitbox = hitbox.GetComponent<InitializeHitbox>();

            initializeHitbox.Initialize(currAttack);

            yield return new WaitForSeconds(currAttack.activeTime);

            playerLocomotion.canMove = true;
            playerLocomotion.rb.isKinematic = false;
            Destroy(hitbox);
            exclamationMark.SetActive(false);

            yield return new WaitForSeconds(initializeHitbox.cooldown);

            canAttack = true;

            playerLocomotion.speed = playerLocomotion.initSpeed;
        }
    }
}
