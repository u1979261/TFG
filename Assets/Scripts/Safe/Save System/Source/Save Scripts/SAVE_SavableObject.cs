using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SAVE_SavableObject
{
    //Default
    public int Id;
    public Vector3 position;
    public Quaternion rotation;

    //DROP BAG
    public int itemID;
    public int itemStack;

    //STORAGE
    public int[] StorageItemsData;
    public int[] StorageItemsStack;

}
