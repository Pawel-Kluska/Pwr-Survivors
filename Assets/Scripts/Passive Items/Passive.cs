using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passive : Item
{
    [SerializeField] CharacterData.Stats currentBoosts;

    [System.Serializable]
    public class Modifier : LevelData
    {
        public CharacterData.Stats boosts;
    }

    public virtual void Initialise(PassiveData data)
    {
        base.Initialise(data);
        this.data = data;
        currentBoosts = data.baseStats.boosts;
    }

    public virtual CharacterData.Stats GetBoosts()
    {
        return currentBoosts;
    }

    public override bool DoLevelUp()
    {
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to Level {1}, max level of {2} already reached.", name, currentLevel, data.maxLevel));
            return false;
        }

        currentBoosts += ((Modifier)data.GetLevelData(++currentLevel)).boosts;
        return true;
    }
}
