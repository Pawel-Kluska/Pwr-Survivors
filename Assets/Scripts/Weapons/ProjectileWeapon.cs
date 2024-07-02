using UnityEngine;

public class ProjectileWeapon : Weapon
{
    protected float currentAttackInterval;
    protected int currentAttackCount;

    protected void Start()
    {
        currentAttackCount = currentStats.number;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (currentAttackInterval > 0f)
        {
            currentAttackInterval -= Time.deltaTime;
            if (currentAttackInterval <= 0) Attack(currentAttackCount);
        }
    }

    public override bool CanAttack()
    {
        if (currentAttackCount > 0) return true;
        return base.CanAttack();
    }

    protected override bool Attack(int attackCount = 1)
    {
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been set for {0}", name));
            ActivateCooldown();
            return false;
        }

        if (!CanAttack()) return false;

        float spawnAngle = GetSpawnAngle();

        Projectile prefab = Instantiate(currentStats.projectilePrefab, owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
        Quaternion.Euler(0, 0, spawnAngle));

        prefab.weapon = this;
        prefab.owner = owner;

        if (currentCooldown <= 0)
            ActivateCooldown();

        attackCount--;

        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = currentStats.projectileInterval;
        }

        return true;

    }

    protected virtual float GetSpawnAngle()
    {
        return Mathf.Atan2(movement.lastMovedVector.y, movement.lastMovedVector.x) * Mathf.Rad2Deg;
    }

    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, 0) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax));
    }
}
