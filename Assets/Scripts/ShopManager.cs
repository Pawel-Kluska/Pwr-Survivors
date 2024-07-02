using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopManager : MonoBehaviour, IDataPersistence
{
    private string[] polishUpgradeNames = { "Życie", "Regeneracja", "Prędkość ruchu", "Moc", "Prędkość pocisku", "Czas trwania", "Magnes", "Czas odnawiania" };
    public int maxUpgrades = 5;

    [System.Serializable]
    public class Upgrade
    {
        public string name;
        public int price;
        public int maxUpgrades;
        public int currentUpgrade;
        public float increments;
        public TMP_Text display;
        public string description;
        public Sprite icon;


    }

    public List<Upgrade> upgrades;
    public List<Button> upgradeOptions;
    private int coins;

    public TMP_Text coinsDisplay;

    [Header("Description Display")]
    public GameObject descriptionPanel;
    public TMP_Text descriptionName;
    public TMP_Text descriptionDisplay;
    public Button buyButton;
    public Image descriptionIcon;
    public TMP_Text priceDisplay;



    void Start()
    {
        descriptionPanel.SetActive(false);
        AssignUpgradesToButtons();
        UpdateUpgradeDisplays();
        UpdateCoinDisplay();
    }

    void Update()
    {
    }

    void AssignUpgradesToButtons()
    {
        for (int i = 0; i < upgradeOptions.Count; i++)
        {
            int tempVar = i;
            upgradeOptions[i].onClick.AddListener(() => ShowUpgradeDescription(tempVar));
        }
    }

    void ShowUpgradeDescription(int upgradeNum)
    {
        Upgrade upgrade = upgrades[upgradeNum];

        descriptionDisplay.text = upgrade.description;

        int price = upgrade.price + (int)(upgrade.price * upgrade.increments * upgrade.currentUpgrade);

        buyButton.onClick.RemoveAllListeners();
        if (price > coins || upgrade.currentUpgrade >= upgrade.maxUpgrades)
            buyButton.interactable = false;
        else
        {
            buyButton.interactable = true;
            buyButton.onClick.AddListener(() => BuyUpgrade(upgrade));
        }

        priceDisplay.text = price.ToString();
        descriptionName.text = polishUpgradeNames[upgradeNum];
        descriptionIcon.sprite = upgrade.icon;

        descriptionPanel.SetActive(true);
    }

    public void BuyUpgrade(Upgrade upgrade)
    {
        int startingPrice = upgrade.price;
        int price = startingPrice + (int)(startingPrice * upgrade.increments * upgrade.currentUpgrade);

        coins -= price;

        if (price > coins || upgrade.currentUpgrade >= upgrade.maxUpgrades)
            buyButton.interactable = false;
        UpdateCoinDisplay();

        upgrade.currentUpgrade++;
        upgrade.display.text = string.Format("{0}/{1}", upgrade.currentUpgrade, upgrade.maxUpgrades);

        price = startingPrice + (int)(startingPrice * upgrade.increments * upgrade.currentUpgrade);
        priceDisplay.text = price.ToString();

        DataPersistenceManager.instance.SaveGame(this);
    }

    void UpdateUpgradeDisplays()
    {
        foreach (Upgrade upgrade in upgrades)
        {
            upgrade.display.text = string.Format("{0}/{1}", upgrade.currentUpgrade, upgrade.maxUpgrades);
        }
    }

    void UpdateCoinDisplay()
    {
        coinsDisplay.text = coins.ToString();
    }

    public void LoadData(GameData data)
    {
        this.coins = data.coinsCollected;

        FieldInfo[] fields = typeof(CharacterData.Stats).GetFields(BindingFlags.Public | BindingFlags.Instance);

        int i = 0;
        foreach (var field in fields)
        {
            float val = (float)field.GetValue(data.statsUpgrades);
            upgrades[i].currentUpgrade = (int)Mathf.Floor(val / upgrades[i].increments);
            i++;
        }
    }

    public void SaveData(GameData data)
    {
        Upgrade healthUpgrade = upgrades.Find(x => x.name.Contains("Health"));
        Upgrade recoveryUpgrade = upgrades.Find(x => x.name.Contains("Recovery"));
        Upgrade moveSpeedUpgrade = upgrades.Find(x => x.name.Contains("Move Speed"));
        Upgrade mightUpgrade = upgrades.Find(x => x.name.Contains("Might"));
        Upgrade speedUpgrade = upgrades.Find(x => x.name == "Speed");
        Upgrade durationUpgrade = upgrades.Find(x => x.name.Contains("Duration"));
        Upgrade magnetUpgrade = upgrades.Find(x => x.name.Contains("Magnet"));
        Upgrade cooldownUpgrade = upgrades.Find(x => x.name.Contains("Cooldown"));

        data.coinsCollected = coins;

        data.statsUpgrades = new CharacterData.Stats
        {
            maxHealth = healthUpgrade.currentUpgrade * healthUpgrade.increments,
            recovery = recoveryUpgrade.currentUpgrade * recoveryUpgrade.increments,
            moveSpeed = moveSpeedUpgrade.currentUpgrade * moveSpeedUpgrade.increments,
            might = mightUpgrade.currentUpgrade * mightUpgrade.increments,
            speed = speedUpgrade.currentUpgrade * speedUpgrade.increments,
            duration = durationUpgrade.currentUpgrade * durationUpgrade.increments,
            magnet = magnetUpgrade.currentUpgrade * magnetUpgrade.increments,
            cooldown = cooldownUpgrade.currentUpgrade * cooldownUpgrade.increments
        };
    }

}
