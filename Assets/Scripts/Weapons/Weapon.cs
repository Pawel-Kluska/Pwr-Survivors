using UnityEngine;

public abstract class Weapon : Item
{
    [System.Serializable]
    public class Stats : LevelData
    {
        [Header("Visuals")]
        public Projectile projectilePrefab;
        public Aura auraPrefab;
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan;
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, piercing, maxInstances;

        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats
            {
                name = s2.name ?? s1.name,
                description = s2.description ?? s1.description,
                projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab,
                auraPrefab = s2.auraPrefab ?? s1.auraPrefab,
                hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect,
                spawnVariance = s2.spawnVariance,
                lifespan = s1.lifespan + s2.lifespan,
                damage = s1.damage + s2.damage,
                damageVariance = s1.damageVariance + s2.damageVariance,
                area = s1.area + s2.area,
                speed = s1.speed + s2.speed,
                cooldown = s1.cooldown + s2.cooldown,
                number = s1.number + s2.number,
                piercing = s1.piercing + s2.piercing,
                projectileInterval = s1.projectileInterval + s2.projectileInterval,
                knockback = s1.knockback + s2.knockback
            };

            return result;
        }

        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }

    protected Stats currentStats;
    protected float currentCooldown;
    protected PlayerMovement movement;

    public virtual void Initialise(WeaponData data)
    {
        base.Initialise(data);
        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<PlayerMovement>();
        ActivateCooldown();
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f)
        {
            Attack(currentStats.number);
        }
    }

    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to level {1}, max level or {2} already reached", name, currentLevel, maxLevel));
            return false;
        }

        currentStats += (Stats)data.GetLevelData(++currentLevel);
        return true;
    }

    public virtual bool CanAttack()
    {
        return currentCooldown <= 0;
    }

    protected virtual bool Attack(int attackCount = 1)
    {
        if (CanAttack())
        {
            ActivateCooldown();
            return true;
        }
        return false;
    }

    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.Stats.might;
    }

    public virtual Stats GetStats() { return currentStats; }

    public virtual bool ActivateCooldown(bool strict = false)
    {
        if (strict && currentCooldown > 0) return false;

        float actualCooldown = currentStats.cooldown * Owner.Stats.cooldown;

        currentCooldown = Mathf.Min(actualCooldown, currentCooldown + actualCooldown);
        return true;
    }
}
