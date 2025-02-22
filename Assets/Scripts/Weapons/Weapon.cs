using UnityEngine;

public class Weapon : MonoBehaviour
{
    
    [HideInInspector] public Animator anim;
    [HideInInspector] public Slot slotEquippedOn;

    private Player player;
    private AudioSource audioS;

    public ItemSO weaponData;
    public bool isAutomatic;

    [Space]
    public Transform shootPoint;
    public LayerMask shootableLayers;

    [Header("Aiming")]
    public float aimSpeed = 10;
    public Vector3 hipPos;
    public Vector3 aimPos;
    public bool isAiming;


    [HideInInspector] public bool isReloading;
    [HideInInspector] public bool hasTakenOut;

    private float currentFireRate;
    private float fireRate;
    private void Start()
    {
        player = GetComponentInParent<Player>();
        audioS = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        transform.localPosition = hipPos;
        fireRate = weaponData.fireRate;
        currentFireRate = weaponData.fireRate;
    }

    private void Update()
    {
        UpdateAnimations();
        if (weaponData.itemType == ItemSO.ItemType.Weapon)
        {
            if (currentFireRate < fireRate) 
            {
                currentFireRate += Time.deltaTime;
            }

            UpdateAiming();
            if (isAutomatic)
            {
                if (Input.GetButton("Fire1"))
                {
                    Shoot();
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Shoot();
                }
            }
        }
        else if (weaponData.itemType == ItemSO.ItemType.MeleeWeapon)
        {

        }
    }

    //FIRE WEAPONS FUNCTIONS

    public void Shoot()
    {
        if(currentFireRate<fireRate || isReloading || hasTakenOut || player._isRunning || slotEquippedOn.stackSize <= 0)
        {
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(shootPoint.position, shootPoint.forward, out hit, weaponData.range, shootableLayers))
        {
            Debug.Log($"Hitted : {hit.transform.name}");
        }


        anim.CrossFadeInFixedTime("Shoot_BASE", 0.015f);
        GetComponentInParent<CameraLook>().RecoilCamera(Random.Range(weaponData.minVerticalRecoil, weaponData.maxVerticalRecoil), Random.Range(-weaponData.horizontalRecoil, weaponData.horizontalRecoil));
        //audioS.PlayOneShot(weaponData.shootSound);

        currentFireRate = 0;
        slotEquippedOn.stackSize--;
        slotEquippedOn.UpdateSlot();
    }
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
    
    public void UpdateAnimations()
    {
        anim.SetBool("Running", player._isRunning);
    }

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