using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu (fileName = "New Item", menuName = "The Island/Inventory/New Item")]
public class ItemSO : ScriptableObject
{
    public enum ItemType { Generic, Consumable, Weapon, MeleeWeapon}

    [Header("General")]

    public ItemType itemType;
    public Sprite itemIcon;
    public string itemName = "New item";
    public string itemDescription = "New item description";

    [Space]
    public bool isStackable;
    public int maxStack = 1;

    [Header("Weapon")]
    public float damage = 15f;
    public float range = 300f;

    [Space]
    public int magSize = 20;
    public float fireRate = 0.1f;

    [Header("Consumable")]
    public float healthStats = 10f;
    public float thirstStats = 10f;
    public float hungerStats = 10f;

}
