using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat/Attack")]
public class AttackData : ScriptableObject
{
    public float activeTime;
    public float cooldown;
    public float startUp;
    public float damage;
    public float baseKnockback;
    public float knockbackScaling;
    public Vector2 hitboxSize;
    public Vector2 offset;
    public GameObject hitboxPrefab;
}
