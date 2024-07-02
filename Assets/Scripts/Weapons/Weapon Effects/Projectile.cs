using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    public enum DamageSource { projectile, owner };
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0, 0, 0);

    protected Rigidbody2D rb;
    protected int piercing;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();

        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.velocity = transform.right * stats.speed * weapon.Owner.Stats.speed;
        }

        float area = stats.area == 0 ? 1 : stats.area;
        transform.localScale = new Vector3(area * Mathf.Sign(transform.localScale.x), area * Mathf.Sign(transform.localScale.y), 1);

        piercing = stats.piercing;

        if (stats.lifespan > 0) Destroy(gameObject, stats.lifespan);
        if (hasAutoAim) AcquireAutoAimFacing();
    }

    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle;

        EnemyStats[] targets = FindObjectsOfType<EnemyStats>();

        if (targets.Length > 0)
        {
            EnemyStats selectedTarget = targets[Random.Range(0, targets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
    }

    protected virtual void FixedUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            transform.position += transform.right * stats.speed * weapon.Owner.Stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats es = other.GetComponent<EnemyStats>();
        BossStats bs = other.GetComponent<BossStats>();
        BreakableProps p = other.GetComponent<BreakableProps>();

        if (bs)
        {
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;

            bs.TakeDamage(GetDamage(), source);

            piercing--;
        }
        else if (es)
        {
            Vector3 source = damageSource == DamageSource.owner && owner ? owner.transform.position : transform.position;

            es.TakeDamage(GetDamage(), source);

            Weapon.Stats stats = weapon.GetStats();
            piercing--;

            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }

        }
        else if (p)
        {
            p.TakeDamage(GetDamage());
            piercing--;

            Weapon.Stats stats = weapon.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }

        if (piercing <= 0) Destroy(gameObject);
    }
}
