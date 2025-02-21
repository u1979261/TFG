using UnityEngine;

public class Weapon : MonoBehaviour
{
    [HideInInspector] public Animator anim;
    [HideInInspector] public Slot slotEquippedOn;
    private Player player;

    public ItemSO weaponData;

    [Header("Aiming")]
    public float aimSpeed = 10;
    public Vector3 hipPos;
    public Vector3 aimPos;
    public bool isAiming;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        anim = GetComponentInChildren<Animator>();
        transform.localPosition = hipPos;
    }

    private void Update()
    {
        if (weaponData.itemType == ItemSO.ItemType.Weapon)
        {
            UpdateAiming();
        }
        else if (weaponData.itemType == ItemSO.ItemType.MeleeWeapon)
        {

        }
    }

    //FIRE WEAPONS FUNCTIONS
    public void UpdateAiming()
    {
        if (Input.GetButton("Fire2") && !player._isRunning)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, aimPos, aimSpeed * Time.deltaTime);
        }
        else 
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, hipPos, aimSpeed * Time.deltaTime);
        }
    }

    ////////////
    
    public void Equip(Slot slot)
    {
        gameObject.SetActive(true);

        slotEquippedOn = slot;
        transform.localPosition = hipPos;
    }
    public void UnEquip()
    {
        gameObject.SetActive (false);
        slotEquippedOn = null;
    }
}