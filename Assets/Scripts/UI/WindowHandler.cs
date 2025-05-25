using UnityEngine;

public class WindowHandler : MonoBehaviour
{
    [HideInInspector] public InventoryManager inventory;
    [HideInInspector] public CraftingManager crafting;
    [HideInInspector] public StorageUI storage;
    [HideInInspector] public BuildingHandler building;
    [HideInInspector] public GameMenu gameMenu;
    public GameObject bloodImage;
    public bool isOpen;

    private CameraLook _camera;
    
    private void Start()
    {
        _camera = GetComponentInChildren<CameraLook>();
        storage = GetComponentInChildren<StorageUI>();
        inventory = GetComponentInChildren<InventoryManager>();
        crafting = GetComponentInChildren<CraftingManager>();
        building = GetComponentInChildren<BuildingHandler>();
        gameMenu = FindAnyObjectByType<GameMenu>();
    }

    private void Update()
    {
        if (isOpen)
        {
            _camera.lockCursor = false;
            _camera.canMove = false;
        }
        else
        {
            _camera.lockCursor = true;
            _camera.canMove = true;
        }
        if (gameMenu != null)
        {

            if (inventory.opened || gameMenu.opened)
            {
                isOpen = true;
                bloodImage.SetActive(false);
            }
            else
            {
                isOpen = false;
                bloodImage.SetActive(true);
            }
        }
        else
        {
            if (inventory.opened)
            {
                isOpen = true;
                bloodImage.SetActive(false);
            }
            else
            {
                isOpen = false;
                bloodImage.SetActive(true);
            }
        }
    }
}
