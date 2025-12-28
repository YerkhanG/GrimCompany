using System;
using data;
using UnityEngine;
using entity;

public static class EnemyProgression
{
    public static float GetMultiplier(RunManager run, EnemyProgressionSettings s)
    {
        if (run == null || run.currentMapPath == null || run.currentMapPath.Length == 0)
            return 1f;

        var node = run.CurrentNode;
        if (node == null) return 1f;

        int row = node.row;

        // Find max row in the stored map (so we don't depend on generator height)
        int maxRow = 0;
        foreach (var n in run.currentMapPath)
            maxRow = Mathf.Max(maxRow, n.row);

        // +20% per row depth: row 0 => 1.0, row 1 => 1.2, row 2 => 1.4 ...
        float mult = 1f + row * s.percentPerRow;
        return Mathf.Clamp(mult, 1f, s.maxMultiplier);
    }

    public static bool IsExempt(Enemy enemy, RunManager run, EnemyProgressionSettings s)
    {
        if (enemy == null || s == null) return true;

        if (s.skipIfCurrentMapNodeIsBoss && run?.CurrentNode?.type == MapNodeType.Boss)
            return true;

        string nameA = enemy.entityName;
        string nameB = enemy.entityData != null ? enemy.entityData.entityName : null;

        foreach (var ex in s.exemptEntityNames)
        {
            if (!string.IsNullOrWhiteSpace(nameA) &&
                string.Equals(ex, nameA, StringComparison.OrdinalIgnoreCase))
                return true;

            if (!string.IsNullOrWhiteSpace(nameB) &&
                string.Equals(ex, nameB, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    public static EntityRuntimeStats BuildScaledStats(EntityData baseData, float mult)
    {
        int ScaleInt(int v) => Mathf.Max(1, Mathf.CeilToInt(v * mult));

        return new EntityRuntimeStats
        {
            maxHealth = ScaleInt(baseData.maxHealth),
            actionSpeed = ScaleInt(baseData.actionSpeed),
            baseDamage = ScaleInt(baseData.baseDamage),
            shieldDamageReduction = baseData.shieldDamageReduction,
            shieldDuration = baseData.shieldDuration,
        };
    }
}