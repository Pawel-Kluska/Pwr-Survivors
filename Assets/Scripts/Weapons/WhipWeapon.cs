using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhipWeapon : ProjectileWeapon
{
    int currentSpawnCount;
    float currentSpawnYOffset;

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been set for {0}", name));
            currentCooldown = currentStats.cooldown;
            return false;
        }

        if (!CanAttack()) return false;

        if (currentCooldown <= 0)
        {
            currentSpawnCount = 0;
            currentSpawnYOffset = 0;
        }

        float spawnDir = Mathf.Sign(movement.lastMovedVector.x) * (currentSpawnCount % 2 == 0 ? 1 : -1);

        Vector2 spawnOffset = new Vector2(
            spawnDir * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            currentSpawnYOffset
        );

        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            transform.position + (Vector3)spawnOffset,
            Quaternion.identity
        );
        prefab.owner = owner;

        if (spawnDir < 0)
        {
            prefab.transform.localScale = new Vector3(
                -Mathf.Abs(prefab.transform.localScale.x),
                prefab.transform.localScale.y,
                prefab.transform.localScale.z
            );
        }

        prefab.weapon = this;
        currentCooldown = currentStats.cooldown;
        attackCount--;

        currentSpawnCount++;
        if (currentSpawnCount > 1 && currentSpawnCount % 2 == 0)
        {
            currentSpawnYOffset += 1;
        }

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = currentStats.projectileInterval;
        }

        return true;
    }
}