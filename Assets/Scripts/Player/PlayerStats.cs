using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    public CharacterData.Stats Stats
    {
        get { return actualStats; }
        set
        {
            actualStats = value;
        }
    }

    float health;

    # region Current Stats Properties

    public float CurrentHealth
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = value;
                UpdateHealthBar();
            }
        }
    }
    # endregion

    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    [Header("I-Frames")]
    public float invincibilityDuration;
    float invicibilityTimer;
    bool isInvincible;

    public List<LevelRange> levelRanges;
    public int weaponIndex;
    public int passiveItemIndex;

    PlayerCollector collector;
    PlayerInventory inventory;

    [Header("UI")]
    public Image healthBar;
    public Image expBar;
    public TMP_Text levelText;

    [Header("Damage Feedback")]
    public Color damageColor = new Color(1, 0, 0, 1);
    public float damageFlashDuration = 0.2f;

    Color originalColor;
    SpriteRenderer sr;
    AudioManager audioManager;

    void Awake()
    {
        characterData = CharacterSelector.GetData();

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        baseStats = actualStats = characterData.stats;
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;

        sr = GetComponent<SpriteRenderer>();
        Debug.Log(characterData.Icon);
        sr.sprite = characterData.Icon;
        originalColor = sr.color;
    }

    void Start()
    {
        inventory.Add(characterData.StartingWeapon);
        experienceCap = levelRanges[0].experienceCapIncrease;
        audioManager.PlayBackgroundMusic(audioManager.backgroundGameMusic);
        GameManager.instance.AssignChosenCharacterUI(characterData);

        UpdateHealthBar();
        UpdateLevelText();
        UpdateExpBar();
    }

    void Update()
    {
        if (isInvincible)
        {
            invicibilityTimer -= Time.deltaTime;
            if (invicibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }

        Recover();
    }

    public void RecalculateStats()
    {
        actualStats = baseStats;
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
        collector.SetRadius(actualStats.magnet);
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;
        LevelUpChecker();

        UpdateExpBar();
    }


    void LevelUpChecker()
    {
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;
            int experienceCapIncrease = 0;
            foreach (LevelRange levelRange in levelRanges)
            {
                if (level >= levelRange.startLevel && level <= levelRange.endLevel)
                {
                    experienceCapIncrease = levelRange.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;

            UpdateLevelText();

            if (audioManager != null && audioManager.levelUpSound != null)
                audioManager.PlaySFX(audioManager.levelUpSound);

            GameManager.instance.StartLevelUp(false);

            if (experience >= experienceCap) LevelUpChecker();
        }
    }

    void UpdateExpBar()
    {
        expBar.fillAmount = (float)experience / experienceCap;
    }

    void UpdateLevelText()
    {
        levelText.text = "LVL " + level.ToString();
    }

    public void TakeDamage(float damage)
    {
        if (!isInvincible)
        {
            if (damage > 0)
            {
                CurrentHealth -= damage;
                if (audioManager != null)
                    audioManager.PlaySFX(audioManager.hurtSound);
                // if(damageEffect) Destroy(Instantiate(damageEffect, transform.position, Quaternion.identity), 5f);

                if (CurrentHealth <= 0)
                {
                    Kill();
                }
            }

            invicibilityTimer = invincibilityDuration;
            isInvincible = true;



            StartCoroutine(DamageFlash());
        }
    }

    IEnumerator DamageFlash()
    {
        sr.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    void UpdateHealthBar()
    {
        healthBar.fillAmount = CurrentHealth / actualStats.maxHealth;
    }

    public void Kill()
    {
        if (!GameManager.instance.IsGameOver)
        {
            if (audioManager != null && audioManager.deathSound != null)
                audioManager.PlaySFX(audioManager.deathSound);
            GameManager.instance.AssignLevelReachedUI(level);
            GameManager.instance.AssignChosenCharacterUI(characterData);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += amount;
            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }

        }
    }

    void Recover()
    {
        if (CurrentHealth < actualStats.maxHealth)
        {
            CurrentHealth += Stats.recovery * Time.deltaTime;

            if (CurrentHealth > actualStats.maxHealth)
            {
                CurrentHealth = actualStats.maxHealth;
            }
        }
    }
}
