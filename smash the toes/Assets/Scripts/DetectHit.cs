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
    float shortHopMultiplier;
    float[] multipliers;
    float grandStaleMultiplier = 1f;

    int lastIndex;
    int maxQueueSize = 9;

    public static List<AttackData> staleQueue = new List<AttackData>();

    Rigidbody2D playerHitRigidbody;

    GameObject playerHit;

    Vector2 dir;

    private void Start()
    {
        //staleQueue.Clear();

        multipliers = new float[]
        {
         .91f, .91f, .92f, .93f, .94f, .95f, .95f, .96f, .97f
        };

        attack = transform.GetComponentInParent<Attack>();
        playerLocomotion = transform.GetComponentInParent<PlayerLocomotion>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && collision.transform != this.transform.parent.transform)
        {
            if (staleQueue.Count > 0)
                lastIndex = staleQueue.FindLastIndex(n => n == attackData);

            staleQueue.Add(attackData);

            if (lastIndex == 9)
                lastIndex = 8;

            if (lastIndex == -1)
                lastIndex = 0;

            if (staleQueue.FindAll(n => n == attackData).Count > 1)
            {
                for (int i = 0; i < staleQueue.FindAll(n => n == attackData).Count; i++)
                {
                    if (i < staleQueue.Count && i != 9)
                    {
                        grandStaleMultiplier *= multipliers[i];
                    }
                }
            }

            if (staleQueue.Count > maxQueueSize)
                staleQueue.RemoveAt(0);

            Debug.Log($"lastindex: {lastIndex} multiplier: {grandStaleMultiplier}");

            playerHit = collision.transform.gameObject;
            playerHitRigidbody = playerHit.GetComponent<Rigidbody2D>();

            dir = playerLocomotion.facingRight ? Vector2.right : Vector2.left;

            shortHopMultiplier = playerLocomotion.shortHopping ? .85f : 1f;

            finalDamage = attackData.damage * grandStaleMultiplier; //* attack.GetStaleMultiplier(attack.currAttack);

            playerHitLocomotion = playerHit.GetComponent<PlayerLocomotion>();
            playerHitPercentageText = playerHit.GetComponent<SetPercentageText>();

            playerHitLocomotion.percentage += finalDamage;

            knockbackFormula = (playerHitLocomotion.percentage / 10) +
                ((playerHitLocomotion.percentage * finalDamage) / 20)
                * (playerHitLocomotion.weight)
                * (1.4f)
                + (18f) * (attackData.knockbackScaling)
                + attackData.baseKnockback;

            //playerHitRigidbody.velocity = knockbackFormula * dir;
            playerHitLocomotion.wasHit = true;
            playerHitLocomotion.hitVector = knockbackFormula * dir;

            playerHitLocomotion.ApplyKnockback((knockbackFormula * dir) / 90);
            playerHit.GetComponent<SetPercentageText>().TookDamage();
        }
    }
}
