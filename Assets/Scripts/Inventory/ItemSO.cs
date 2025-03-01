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

    [Header("Drop Settings")]
    public GameObject specificDropModel;

    [Space]
    public bool isStackable;
    public int maxStack = 1;

    [Header("Tool Settings")]
    public float toolMultiplier = 1f;

    [Header("Weapon")]
    public float damage = 15f;
    public float range = 300f;
    
    [Space]
    public float horizontalRecoil;
    public float minVerticalRecoil;
    public float maxVerticalRecoil;
    [Space]
    [Space]
    public float hipSpread = 0.04f;
    public float aimSpread = 0;
    public float zoomFOV = 60f;

    [Space]
    public bool shotgunFire;
    public int pelletsPerShot = 8;

    [Space]
    public int magSize = 20;
    public ItemSO bulletData;
    public float fireRate = 0.1f;


    [Space]
    [Space]
    [Space]

    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioClip takeoutSound;
    public AudioClip emptySound;

    [Header("Consumable")]
    public float healthStats = 10f;
    public float thirstStats = 10f;
    public float hungerStats = 10f;

}
