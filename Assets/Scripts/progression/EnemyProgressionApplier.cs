using UnityEngine;
using entity;

[DisallowMultipleComponent]
[DefaultExecutionOrder(100)]
public class EnemyProgressionApplier : MonoBehaviour
{
    [SerializeField] private EnemyProgressionSettings settings;

    private bool _applied;

    private void Start()
    {
        if (_applied) return;

        var enemy = GetComponent<Enemy>();
        if (enemy == null || settings == null || enemy.entityData == null) return;

        var run = RunManager.Instance;

        if (EnemyProgression.IsExempt(enemy, run, settings))
            return;

        float mult = EnemyProgression.GetMultiplier(run, settings);

        var stats = EnemyProgression.BuildScaledStats(enemy.entityData, mult);

        if (!settings.scaleHealth) stats.maxHealth = enemy.entityData.maxHealth;
        if (!settings.scaleDamage) stats.baseDamage = enemy.entityData.baseDamage;
        if (!settings.scaleActionSpeed) stats.actionSpeed = enemy.entityData.actionSpeed;

        enemy.ApplyRuntimeStats(stats, refillHealth: true);

        _applied = true;
    }
}