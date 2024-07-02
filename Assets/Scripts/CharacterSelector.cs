using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CharacterSelector : MonoBehaviour, IDataPersistence
{
    public static CharacterSelector instance;

    public CharacterData characterData;
    public CharacterData.Stats loadedUpgrades;

    void Awake()
    {
        if (instance == null)
        {
            Debug.Log("Singleton " + this + " created");
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("Extra " + this + " deleted");
            Destroy(gameObject);
        }
    }

    public static CharacterData GetData()
    {
        if (instance && instance.characterData)
            return instance.characterData;
        else
        {
#if UNITY_EDITOR
            string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
            List<CharacterData> characters = new List<CharacterData>();

            foreach (string assetPath in allAssetPaths)
            {
                if (assetPath.EndsWith(".asset"))
                {
                    CharacterData characterData = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);
                    if (characterData != null)
                    {
                        characters.Add(characterData);
                    }
                }
            }
            if (characters.Count > 0) return characters[Random.Range(0, characters.Count)];
#endif
        }
        return null;
    }

    public void SelectCharacter(CharacterData character)
    {
        character.stats += loadedUpgrades;
        characterData = character;
    }

    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }

    public void LoadData(GameData data)
    {
        loadedUpgrades = data.statsUpgrades;
    }
    public void SaveData(GameData data)
    {
        // Nic nie robi, ale musi być, bo interfejs
    }
}
