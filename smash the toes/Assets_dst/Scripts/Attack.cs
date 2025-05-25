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
    AttackData currAttack;
    public bool facingRight;
    bool canAttack = true;
    public float finalDamage;
    public List<AttackData> staleQueue = new List<AttackData>();
    const int maxQueueSize = 9;
    PlayerLocomotion playerLocomotion;

    // Start is called before the first frame update
    void Start()
    {
        staleQueue.Clear();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = playerInput.actions["Move"].ReadValue<float>();

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

            Vector3 hitboxPos = new Vector3(
            (facingRight ? 1 : -1) * currAttack.offset.x,
            currAttack.offset.y,
            0);

            var hitbox = Instantiate(currAttack.hitboxPrefab, transform.position + hitboxPos, Quaternion.identity, this.transform);

            animator.SetTrigger("Attack");
            hitbox.GetComponent<InitializeHitbox>().Initialize(currAttack);
            finalDamage = currAttack.damage * GetStaleMultiplier(currAttack);
            staleQueue.Add(currAttack);

            if (staleQueue.Count > maxQueueSize)
                staleQueue.RemoveAt(0);

            Debug.Log(finalDamage);

            yield return new WaitForSeconds(currAttack.activeTime);

            Destroy(hitbox);

            yield return new WaitForSeconds(hitbox.GetComponent<InitializeHitbox>().cooldown);

            canAttack = true;

            playerLocomotion.speed = playerLocomotion.initSpeed;
        }
    }

    public float GetStaleMultiplier(AttackData move)
    {
        int lastIndex = staleQueue.FindLastIndex(x => x == move);

        if (lastIndex == -1)
        {
            Debug.Log("Move not found in stale queue — using fresh multiplier (1.0)");
            return 1f;
        }

        var distanceFromEnd = staleQueue.Count - 1 - lastIndex;

        float[] multipliers =
        {
         0.52f, 0.58f, 0.64f, 0.70f, 0.76f, 0.82f, 0.88f, 0.94f, 1.00f
        };

        distanceFromEnd = Mathf.Clamp(distanceFromEnd, 0, multipliers.Length - 1);

        Debug.Log($"Stale queue contents:");
        for (int i = 0; i < staleQueue.Count; i++)
            Debug.Log($"{i}: {staleQueue[i].name}");

        Debug.Log($"Current move: {move.name}, Distance from end: {distanceFromEnd}, Multiplier: {multipliers[distanceFromEnd]}");

        return multipliers[distanceFromEnd];
    }
}
