using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public long lastUpdated;
    public int coinsCollected;
    public CharacterData.Stats statsUpgrades;

    // the values defined in this constructor will be the default values
    // the game starts with when there's no data to load
    public GameData()
    {
        this.coinsCollected = 0;
        statsUpgrades = new CharacterData.Stats
        {
            maxHealth = 0,
            recovery = 0,
            moveSpeed = 0,
            might = 0,
            speed = 0,
            duration = 0,
            magnet = 0f,
            cooldown = 0
        };
    }
}