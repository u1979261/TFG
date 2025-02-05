using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu (fileName = "New Item", menuName = "The Island/Inventory/New Item")]
public class ItemSO : ScriptableObject
{
    public enum ItemType { Resources, Consumable, Weapon, MeleeWeapon}

    [Header("General")]

    public ItemType itemType;
    public Sprite itemIcon;
    public string itemName = "New item";
    public string itemDescription = "New item description";

    [Space]
    public bool isStackable;
    public int maxStack = 1;
}
