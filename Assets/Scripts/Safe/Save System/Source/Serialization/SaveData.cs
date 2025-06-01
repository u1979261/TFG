using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData _singleton;

    public static SaveData Singleton
    {
        get
        {
            if (_singleton == null)
            {
                _singleton = new SaveData();
            }

            return _singleton;
        }
    }

    public List<SAVE_SavableObject> savableObjects;

    [Header("Player Stats")]
    public float currentHealth;
    public float currentHunger;
    public float currentThirst;

    [Header("DayNight")]

    public float currentTime;

    [Header("Items")]
    public List<int> itemsIDs;
    public List<int> itemsStacks;

    public static bool Save()
    {
        return SerializationManager.Save("GameSave", SaveData.Singleton);
    }

    public static void Load()
    {
        _singleton = (SaveData)SerializationManager.Load(Application.dataPath + "/saves/GameSave.save");
    }

}
