using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Weapon Data")]
public class WeaponData : ItemData
{
    [HideInInspector]
    public string behaviour;
    public Weapon.Stats baseStats;
    public Weapon.Stats[] linearGrowth;
    public Weapon.Stats[] randomGrowth; // jak nie zdefiniujemy wystarczajaco duzo w linearGrowth w stosunku do max lvl, to wybierze ktores z randomGrowth

    public override Item.LevelData GetLevelData(int level)
    {
        if (level <= 1) return baseStats;

        if (level - 2 < linearGrowth.Length)
            return linearGrowth[level - 2];

        if (randomGrowth.Length > 0)
            return randomGrowth[Random.Range(0, randomGrowth.Length)];

        Debug.LogWarning(string.Format("Weapon doesn't have its level up stats configured for Level {0}", level));

        return new Weapon.Stats();
    }
}
