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

    // Start is called before the first frame update
    void Start()
    {
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

        if (playerInput.actions["Jab"].triggered)
        {
            currAttack = jab;
            PerformAttack();
            playerLocomotion.speed = 0;
        }
        else if (playerInput.actions["Neutral Special"].triggered)
        {
            currAttack = neutralSpecial;
            PerformAttack();
            playerLocomotion.speed /= 5;
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

            yield return new WaitForSeconds(currAttack.hitboxPrefab.GetComponent<InitializeHitbox>().startUp);

            hitboxPos = new Vector3(
            (facingRight ? 1 : -1) * currAttack.offset.x,
            currAttack.offset.y,
            0);

            hitbox = Instantiate(currAttack.hitboxPrefab, transform.position + hitboxPos, Quaternion.identity, this.transform);

            animator.SetTrigger("Attack");

            if (currAttack == neutralSpecial)
            {
                exclamationMark.SetActive(true);
            }

            initializeHitbox = hitbox.GetComponent<InitializeHitbox>();

            initializeHitbox.Initialize(currAttack);

            yield return new WaitForSeconds(currAttack.activeTime);

            Destroy(hitbox);
            exclamationMark.SetActive(false);

            yield return new WaitForSeconds(initializeHitbox.cooldown);

            canAttack = true;

            playerLocomotion.speed = playerLocomotion.initSpeed;
        }
    }
}
