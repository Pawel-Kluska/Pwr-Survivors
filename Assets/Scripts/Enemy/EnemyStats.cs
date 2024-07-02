using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    [HideInInspector]
    public float currentMoveSpeed;
    [HideInInspector]
    public float currentHealth;
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (currentHealth != value)
            {
                currentHealth = value;
                UpdateHealthBar();
            }
        }
    }
    [HideInInspector]
    public float currentDamage;

    public float despawnDistance = 20f;
    Transform player;

    [Header("Damage Feedback")]
    public Color damageColor = Color.white;
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;
    Color originalColor;
    private Shader shaderGUIText;
    private Shader shaderSpritesDefault;
    SpriteRenderer sr;
    EnemyMovement movement;
    AudioManager audioManager;

    protected void Awake()
    {
        currentMoveSpeed = enemyData.MoveSpeed;
        CurrentHealth = enemyData.MaxHealth;
        currentDamage = enemyData.Damage;
    }

    protected virtual void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        sr = GetComponent<SpriteRenderer>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        originalColor = sr.color;
        shaderGUIText = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = sr.material.shader;

        movement = GetComponent<EnemyMovement>();
    }

    protected void Update()
    {
        if (Vector2.Distance(transform.position, player.position) >= despawnDistance)
        {
            ReturnEnemy();
        }
    }

    public virtual void TakeDamage(float dmg, Vector2 sourcePosition, float KnockbackForce = 5f, float knockbackDuration = 0.2f)
    {
        CurrentHealth -= dmg;
        StartCoroutine(DamageFlash());
        if (audioManager != null && audioManager.hurtSound)
            audioManager.PlaySFX(audioManager.hurtSound);

        if (CurrentHealth <= 0)
        {
            Kill();

        }
        else
        {
            GameManager.GenerateFloatingText(Mathf.FloorToInt(dmg).ToString(), transform);
            if (knockbackDuration > 0)
            {
                Vector2 dir = (Vector2)transform.position - sourcePosition;
                movement.Knockback(dir.normalized * KnockbackForce, knockbackDuration);
            }
        }

    }

    IEnumerator DamageFlash()
    {

        sr.color = damageColor;
        sr.material.shader = shaderGUIText;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.material.shader = shaderSpritesDefault;
        sr.color = originalColor;
    }

    public void Kill()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        es.OnEnemyKilled();

        StartCoroutine(KillFade());
    }

    IEnumerator KillFade()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;

        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha);
        }
        if (audioManager != null && audioManager.deathSound != null)
            audioManager.PlaySFX(audioManager.deathSound);
        Destroy(gameObject);
    }

    public void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            PlayerStats player = col.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage);
        }
    }

    private void OnDestroy()
    {

    }
    protected virtual void UpdateHealthBar()
    {

    }
    void ReturnEnemy()
    {
        EnemySpawner es = FindObjectOfType<EnemySpawner>();
        transform.position = player.position + es.relativeSpawnPoints[Random.Range(0, es.relativeSpawnPoints.Count)].position;
    }
}
