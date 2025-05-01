using UnityEngine;

public class Weapon : MonoBehaviour
{

    [HideInInspector] public Animator anim;
    [HideInInspector] public Slot slotEquippedOn;
    [HideInInspector] public WindowHandler windowHandler;

    //public GameObject bulletPrefab;
    private Player player;
    private AudioSource audioS;

    public ItemSO weaponData;
    public bool isAutomatic;
    public ParticleSystem muzzleFlash;

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
        windowHandler = GetComponentInParent<WindowHandler>();
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

            if (Input.GetKeyDown(KeyCode.R) && !windowHandler.isOpen)
            {
                StartReload();
            }


            UpdateAiming();

            if (isAutomatic)
            {
                if (Input.GetMouseButton(0) && !windowHandler.isOpen)
                {
                    if (!weaponData.shotgunFire)
                    {
                        Shoot();
                    }
                    else
                    {
                        ShootGunShoot();
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0) && !windowHandler.isOpen)
                {
                    if (!weaponData.shotgunFire)
                    {
                        Shoot();
                    }
                    else
                    {
                        ShootGunShoot();
                    }
                }
            }
        }
        else if (weaponData.itemType == ItemSO.ItemType.MeleeWeapon)
        {
            if (currentFireRate < fireRate)
            {
                currentFireRate += Time.deltaTime;
            }
            if (Input.GetMouseButton(0) && !windowHandler.isOpen)
            {
                Swing();
            }
        }
    }



    #region FIRE WEAPONS FUNCTIONS

    public void Shoot()
    {
        if (currentFireRate < fireRate || isReloading || !hasTakenOut || player.running || slotEquippedOn.stackSize <= 0)
        {
            return;
        }

        RaycastHit hit;
        Vector3 shootDir = shootPoint.forward;

        if (isAiming)
        {
            shootDir.x += Random.Range(-weaponData.aimSpread, weaponData.aimSpread);
            shootDir.y += Random.Range(-weaponData.aimSpread, weaponData.aimSpread);
        }
        else
        {
            shootDir.x += Random.Range(-weaponData.hipSpread, weaponData.hipSpread);
            shootDir.y += Random.Range(-weaponData.hipSpread, weaponData.hipSpread);
        }

        if (Physics.Raycast(shootPoint.position, shootDir, out hit, weaponData.range, shootableLayers))
        {
            //GameObject bulletHole = Instantiate(bulletPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            AnimalAI animal = hit.transform.GetComponent<AnimalAI>();
            if (animal != null)
            {
                animal.health -= weaponData.damage;
            }
        }
        if(muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        else
        {
            Debug.LogWarning("No Muzzle Flash");
        }

        anim.CrossFadeInFixedTime("Shoot_BASE", 0.015f);
        //GetComponentInParent<CameraLook>().RecoilCamera(Random.Range(-weaponData.horizontalRecoil, weaponData.horizontalRecoil), Random.Range(weaponData.minVerticalRecoil, weaponData.maxVerticalRecoil));
        audioS.PlayOneShot(weaponData.shootSound);

        currentFireRate = 0;
        slotEquippedOn.stackSize--;
        slotEquippedOn.UpdateSlot();
    }
    public void ShootGunShoot()
    {
        if (currentFireRate < fireRate || isReloading || !hasTakenOut || player.running || slotEquippedOn.stackSize <= 0)
        {
            return;
        }
        for (int i = 0; i < weaponData.pelletsPerShot; i++)
        {
            RaycastHit hit;
            Vector3 shootDir = shootPoint.forward;

            if (isAiming)
            {
                shootDir.x += Random.Range(-weaponData.aimSpread, weaponData.aimSpread);
                shootDir.y += Random.Range(-weaponData.aimSpread, weaponData.aimSpread);
            }
            else
            {
                shootDir.x += Random.Range(-weaponData.hipSpread, weaponData.hipSpread);
                shootDir.y += Random.Range(-weaponData.hipSpread, weaponData.hipSpread);
            }
            if (Physics.Raycast(shootPoint.position, shootDir, out hit, weaponData.range, shootableLayers))
            {
                //GameObject bulletHole = Instantiate(bulletPrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                AnimalAI animal = hit.transform.GetComponent<AnimalAI>();
                if (animal != null)
                {
                    animal.health -= weaponData.damage;
                }
            }
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        else
        {
            Debug.LogWarning("No Muzzle Flash");
        }
        anim.CrossFadeInFixedTime("Shoot_BASE", 0.015f);
        GetComponentInParent<CameraLook>().RecoilCamera(Random.Range(weaponData.minVerticalRecoil, weaponData.maxVerticalRecoil), Random.Range(-weaponData.horizontalRecoil, weaponData.horizontalRecoil));
        audioS.PlayOneShot(weaponData.shootSound);

        currentFireRate = 0;
        slotEquippedOn.stackSize--;
        slotEquippedOn.UpdateSlot();
    }
    public void UpdateAiming()
    {
        if (Input.GetButton("Fire2") && !player.running && !isReloading && !windowHandler.isOpen)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, aimPos, aimSpeed * Time.deltaTime);
            isAiming = true;
        }
        else
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, hipPos, aimSpeed * Time.deltaTime);
            isAiming = false;
        }
    }

    public void StartReload()
    {
        if (isReloading || slotEquippedOn.stackSize >= weaponData.magSize || player.running || !hasTakenOut || !CheckForBullets(weaponData.bulletData, weaponData.magSize) || windowHandler.isOpen)
            return;

        audioS.PlayOneShot(weaponData.reloadSound);
        anim.CrossFadeInFixedTime("Reload_BASE", 0.015f);
        isReloading = true;
    }

    public void FinishReload()
    {
        isReloading = false;

        TakeBullets(weaponData.bulletData, weaponData.magSize);

    }


    private bool CheckForBullets(ItemSO bulletData, int magSize)
    {
        InventoryManager inventory = GetComponentInParent<Player>().GetComponentInChildren<InventoryManager>();

        int amountfound = 0;

        for (int b = 0; b < inventory.inventorySlots.Length; b++)
        {
            if (!inventory.inventorySlots[b].isEmpty)
            {
                if (inventory.inventorySlots[b].data == bulletData)
                {
                    amountfound += inventory.inventorySlots[b].stackSize;
                }
            }
        }


        if (amountfound < 1)
            return false;

        return true;
    }

    public void TakeBullets(ItemSO bulletData, int magSize)
    {
        InventoryManager inventory = GetComponentInParent<Player>().GetComponentInChildren<InventoryManager>();

        int ammoToReload = weaponData.magSize - slotEquippedOn.stackSize;
        int ammoInTheInventory = 0;

        // CHECK FOR THE BULLETS
        for (int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            if (!inventory.inventorySlots[i].isEmpty)
            {
                if (inventory.inventorySlots[i].data == bulletData)
                {
                    ammoInTheInventory += inventory.inventorySlots[i].stackSize;
                }
            }
        }


        int ammoToTakeFromInventory = (ammoInTheInventory >= ammoToReload) ? ammoToReload : ammoInTheInventory;


        // TAKE THE BULLETS FROM THE INVENTORY
        for (int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            if (!inventory.inventorySlots[i].isEmpty && ammoToTakeFromInventory > 0)
            {
                if (inventory.inventorySlots[i].data == bulletData)
                {
                    if (inventory.inventorySlots[i].stackSize <= ammoToTakeFromInventory)
                    {
                        slotEquippedOn.stackSize += inventory.inventorySlots[i].stackSize;
                        ammoToTakeFromInventory -= inventory.inventorySlots[i].stackSize;
                        inventory.inventorySlots[i].Clean();
                    }
                    else if (inventory.inventorySlots[i].stackSize > ammoToTakeFromInventory)
                    {
                        slotEquippedOn.stackSize = weaponData.magSize;
                        inventory.inventorySlots[i].stackSize -= ammoToTakeFromInventory;
                        ammoToTakeFromInventory = 0;
                        inventory.inventorySlots[i].UpdateSlot();
                    }
                }
            }
        }


        slotEquippedOn.UpdateSlot();

    }
    #endregion


    #region Melee Weapon Functions
    public void Swing()
    {
        if (currentFireRate < fireRate || isReloading || !hasTakenOut || player.running || slotEquippedOn.stackSize <= 0)
        {
            return;
        }
        anim.SetTrigger("Swing");

        currentFireRate = 0;
    }

    public void CheckForHit() 
    {
        RaycastHit hit;
        if(Physics.SphereCast(shootPoint.position,0.5f,shootPoint.forward,out hit, weaponData.range, shootableLayers))
        {
           Hit();
        }
        else
        {
            Miss();
        }
    }

    public void Miss()
    {
        anim.SetTrigger("Miss");
    }

    public void Hit()
    {
        anim.SetTrigger("Hit");
    }   

    public void ExecuteHit()
    {
        RaycastHit hit;
        if (Physics.SphereCast(shootPoint.position, 0.2f, shootPoint.forward, out hit, weaponData.range, shootableLayers))
        {
            ResourceObject resourceObj = hit.transform.GetComponent<ResourceObject>();
            ResourceExtenssion resourceExt = hit.transform.GetComponent<ResourceExtenssion>();

            if (resourceObj != null)
                resourceObj.Recolect(weaponData, GetComponentInParent<WindowHandler>().inventory);

            if (resourceExt != null)
                resourceExt.Recolect(weaponData, GetComponentInParent<WindowHandler>().inventory);

        }
    }

    #endregion
    public void UpdateAnimations()
    {
        anim.SetBool("Running", player.running);
    }

    public void Equip(Slot slot)
    {
        gameObject.SetActive(true);
        GetComponentInParent<CameraFOV_Handler>().weapon = this;
        slotEquippedOn = slot;
        slot.weaponEquippedOn = this;
        transform.localPosition = hipPos;
    }
    public void UnEquip()
    {
        GetComponentInParent<CameraFOV_Handler>().weapon = null;
        slotEquippedOn.weaponEquippedOn = null;
        slotEquippedOn = null;

        isReloading = false;
        hasTakenOut = false;

        gameObject.SetActive (false);
    }
}