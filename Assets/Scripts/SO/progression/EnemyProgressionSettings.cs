using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "EnemyProgressionSettings",
    menuName = "Progression/Enemy Progression Settings"
)]
public class EnemyProgressionSettings : ScriptableObject
{
    [Header("Progression")]
    [Tooltip("Example: 0.2 = +20% per map row depth.")]
    [Range(0f, 2f)]
    public float percentPerRow = 0.2f;

    [Tooltip("Optional clamp so stats don't explode.")]
    [Range(1f, 20f)]
    public float maxMultiplier = 5f;

    [Header("What to scale")]
    public bool scaleHealth = true;
    public bool scaleDamage = true;

    [Tooltip("Scaling speed can break turn order; consider leaving off.")]
    public bool scaleActionSpeed = false;

    [Header("Exemptions (boss etc.)")]
    public bool skipIfCurrentMapNodeIsBoss = true;

    [Tooltip("Entity.entityName OR EntityData.entityName that should not scale.")]
    public List<string> exemptEntityNames = new();
}