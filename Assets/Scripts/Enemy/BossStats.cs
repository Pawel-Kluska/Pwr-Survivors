using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class BossStats : EnemyStats
{
    [Header("UI")]
    public Image healthBar;
    protected override void Start()
    {
        base.Start();
        UpdateHealthBar();
    }

    public override void TakeDamage(float dmg, Vector2 sourcePosition, float KnockbackForce = 0f, float knockbackDuration = 0f)
    {
        base.TakeDamage(dmg, sourcePosition, KnockbackForce, knockbackDuration);

    }

    protected override void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / enemyData.MaxHealth;
    }
}
