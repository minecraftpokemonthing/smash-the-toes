using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectHit : MonoBehaviour
{
    PlayerLocomotion playerLocomotion;
    Attack attack;

    public float knockbackFormula;
    public float decayRate = 3f;

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

            var playerHit = collision.transform.gameObject;
            var playerHitRigidbody = playerHit.GetComponent<Rigidbody2D>();

            var dir = playerLocomotion.facingRight ? Vector2.right : Vector2.left;

            playerHit.GetComponent<PlayerLocomotion>().percentage += this.transform.parent.GetComponent<Attack>().finalDamage;

            knockbackFormula = (playerHit.GetComponent<PlayerLocomotion>().percentage / 10)  +
                ((playerHit.GetComponent<PlayerLocomotion>().percentage * this.transform.parent.GetComponent<Attack>().finalDamage) / 20)
                * (playerHit.GetComponent<PlayerLocomotion>().weight)
                * (1.4f)
                + (18f) * (this.GetComponent<InitializeHitbox>().knockbackScaling)
                + this.GetComponent<InitializeHitbox>().baseKnockback;

            Debug.Log(playerHitRigidbody);

            //playerHitRigidbody.velocity = knockbackFormula * dir;
            playerHit.GetComponent<PlayerLocomotion>().wasHit = true;
            playerHit.GetComponent<PlayerLocomotion>().hitVector = knockbackFormula * dir;

            playerHit.GetComponent<PlayerLocomotion>().ApplyKnockback((knockbackFormula * dir) / 90);
            playerHit.GetComponent<SetPercentageText>().TookDamage();
        }
    }
}
