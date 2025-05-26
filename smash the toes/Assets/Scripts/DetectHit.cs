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
    public AttackData attackData;

    public float knockbackFormula;
    public float decayRate = 3f;
    public float finalDamage;

    Rigidbody2D playerHitRigidbody;

    GameObject playerHit;

    Vector2 dir;

    private void Start()
    {
        attack = transform.GetComponentInParent<Attack>();
        playerLocomotion = transform.GetComponentInParent<PlayerLocomotion>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && collision.transform != this.transform.parent.transform)
        {
            Debug.Log(collision.name);

            playerHit = collision.transform.gameObject;
            playerHitRigidbody = playerHit.GetComponent<Rigidbody2D>();

            dir = playerLocomotion.facingRight ? Vector2.right : Vector2.left;

            finalDamage = attackData.damage; //* attack.GetStaleMultiplier(attack.currAttack);

            playerHitLocomotion = playerHit.GetComponent<PlayerLocomotion>();
            playerHitPercentageText = playerHit.GetComponent<SetPercentageText>();

            playerHitLocomotion.percentage += finalDamage;

            knockbackFormula = (playerHitLocomotion.percentage / 10)  +
                ((playerHitLocomotion.percentage * finalDamage) / 20)
                * (playerHitLocomotion.weight)
                * (1.4f)
                + (18f) * (attackData.knockbackScaling)
                + attackData.baseKnockback;

            Debug.Log(playerHitRigidbody);

            //playerHitRigidbody.velocity = knockbackFormula * dir;
            playerHitLocomotion.wasHit = true;
            playerHitLocomotion.hitVector = knockbackFormula * dir;

            playerHitLocomotion.ApplyKnockback((knockbackFormula * dir) / 90);
            playerHit.GetComponent<SetPercentageText>().TookDamage();
        }
    }
}
