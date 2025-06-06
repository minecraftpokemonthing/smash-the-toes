using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeHitbox : MonoBehaviour
{
    public float activeTime;
    public float damage;
    public float baseKnockback;
    public float knockbackScaling;
    public float cooldown;
    public float startUp;
    public Vector2 hitboxSize;
    public Vector2 offset;
    public GameObject hitboxPrefab;
    public BoxCollider2D collider;
    public AttackData attackData;

    public void Initialize(AttackData attackData)
    {
        this.activeTime = attackData.activeTime;
        this.damage = attackData.damage;
        this.baseKnockback = attackData.baseKnockback;
        this.knockbackScaling = attackData.knockbackScaling;
        this.hitboxSize = attackData.hitboxSize;
        this.cooldown = attackData.cooldown;
        this.startUp = attackData.startUp;
        this.offset = attackData.offset;
        if (!collider)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                collider = transform.GetChild(i).GetComponent<BoxCollider2D>();
            }
        }
        else
        {
            collider.size = Vector2.one;
        }

        transform.localScale = hitboxSize;
    }
}
