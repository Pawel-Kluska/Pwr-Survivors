using UnityEngine;

[CreateAssetMenu(fileName = "Passive Data", menuName = "Passive Data")]
public class PassiveData : ItemData
{
    public Passive.Modifier baseStats;
    public Passive.Modifier[] growth;

    public override Item.LevelData GetLevelData(int level)
    {
        if (level <= 1) return baseStats;

        if (level - 2 < growth.Length)
            return growth[level - 2];

        Debug.LogWarning(string.Format("Passive doesn't have its level up stats configured for level {0}", level));
        return new Passive.Modifier();
    }
}
