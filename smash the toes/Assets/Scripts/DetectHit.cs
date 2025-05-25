using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectHit : MonoBehaviour
{
    PlayerLocomotion playerLocomotion;
    PlayerLocomotion playerHitLocomotion;
    public InitializeHitbox initializeHitbox;
    SetPercentageText playerHitPercentageText;
    Attack attack;

    public float knockbackFormula;
    public float decayRate = 3f;

    Rigidbody2D playerHitRigidbody;

    GameObject playerHit;

    Vector2 dir;

    private void Start()
    {
        attack = transform.parent.GetComponent<Attack>();
        playerLocomotion = transform.parent.GetComponent<PlayerLocomotion>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && collision.transform != this.transform.parent.transform)
        {
            Debug.Log(collision.name);

            playerHit = collision.transform.gameObject;
            playerHitRigidbody = playerHit.GetComponent<Rigidbody2D>();

            dir = playerLocomotion.facingRight ? Vector2.right : Vector2.left;

            playerHitLocomotion = playerHit.GetComponent<PlayerLocomotion>();
            playerHitPercentageText = playerHit.GetComponent<SetPercentageText>();

            playerHitLocomotion.percentage += attack.finalDamage;

            knockbackFormula = (playerHitLocomotion.percentage / 10)  +
                ((playerHitLocomotion.percentage * attack.finalDamage) / 20)
                * (playerHitLocomotion.weight)
                * (1.4f)
                + (18f) * (initializeHitbox.knockbackScaling)
                + initializeHitbox.baseKnockback;

            Debug.Log(playerHitRigidbody);

            //playerHitRigidbody.velocity = knockbackFormula * dir;
            playerHitLocomotion.wasHit = true;
            playerHitLocomotion.hitVector = knockbackFormula * dir;

            playerHitLocomotion.ApplyKnockback((knockbackFormula * dir) / 90);
            playerHitPercentageText.TookDamage();
        }
    }
}
