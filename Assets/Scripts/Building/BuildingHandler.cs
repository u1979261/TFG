using UnityEngine;
using System.Collections.Generic;

public class BuildingHandler : MonoBehaviour
{
    public Slot slotInUse;
    public Transform offGroundPoint;
    [HideInInspector] public WindowHandler windowHandler;
    [Space]
    public float range = 5f;
    public Color allowed;
    public Color blocked;
    [Space]
    public BuildReference buildReference;
    public bool cantBuild;
    private float currentRotation = 0f;

    private readonly Dictionary<string, Vector2> buildOffsets = new Dictionary<string, Vector2>()
    {
        { "Foundation", new Vector2(0f, 0f) },
        { "Wall",       new Vector2(0.26f, 0.104f) },
        { "Box",        new Vector2(0.25f, 0.1f) },
        { "Door",       new Vector2(0.26f, 0.1f) },
        { "DoorWay",    new Vector2(0.21f, 0.101f) },
        { "Floor",      new Vector2(0.5f, 0f) },
        { "Furnace",    new Vector2(0f, 0f) },
        { "Campfire",   new Vector2(0.26f, 0.104f) },
        { "Sleeping",   new Vector2(0f, 0f)},
    };

    public void Start()
    {
        windowHandler = GetComponentInParent<WindowHandler>();
    }

    private void Update()
    {
        if (!windowHandler.isOpen)
        {
            UpdateBuilding();
            HandleRotation();
        }
    }

    public void UpdateColors()
    {
        MeshRenderer[] renderers = buildReference.GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in renderers)
        {
            for (int i = 0; i < renderer.materials.Length; i++)
            {
                renderer.materials[i].color = cantBuild ? allowed : blocked;
            }
        }
    }

    public void UpdateBuilding()
    {
        if (slotInUse == null || slotInUse.stackSize <= 0 || slotInUse.data == null)
        {
            if (buildReference != null)
            {
                Destroy(buildReference.gameObject);
            }
            return;
        }

        if (buildReference == null)
        {
            buildReference = Instantiate(slotInUse.data.buildReference, offGroundPoint.position, Quaternion.Euler(0, currentRotation, 0));
        }

        UpdateColors();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            if (hit.transform.GetComponent<BuildBlocked>() == null)
            {
                string tag = buildReference.buildPrefab.tag;
                float up = 0f;
                float forward = 0f;

                if (buildOffsets.ContainsKey(tag))
                {
                    up = buildOffsets[tag].x;
                    forward = buildOffsets[tag].y;
                }

                if (tag == "Furnace")
                {
                    Collider[] nearby = Physics.OverlapSphere(hit.point, 0.3f);
                    foreach (var col in nearby)
                    {
                        if (col.CompareTag("Foundation"))
                        {
                            up += 0.1f;
                            break;
                        }
                    }
                }

                AlignToGrid(hit.point, hit.normal, 0.825f, up, forward);
                cantBuild = true;
            }
            else
            {
                buildReference.transform.position = offGroundPoint.position;
                buildReference.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
                cantBuild = false;
            }
        }
        else
        {
            buildReference.transform.position = offGroundPoint.position;
            buildReference.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
            cantBuild = false;
        }

        CheckOverlap();

        if (Input.GetMouseButtonDown(0) && cantBuild && buildReference.canBuild && buildReference.buildPrefab != null)
        {
            slotInUse.stackSize--;
            slotInUse.UpdateSlot();
            Quaternion finalRotation = buildReference.transform.rotation;

            if (buildReference.buildPrefab.CompareTag("Furnace"))
            {
                finalRotation = Quaternion.Euler(0, 180f, 0);
            }

            Instantiate(buildReference.buildPrefab, buildReference.transform.position, finalRotation);
        }
    }

    private void AlignToGrid(Vector3 position, Vector3 normal, float gridSize, float up, float forwardOffset)
    {
        Vector3 alignedPosition = new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize,
            Mathf.Round(position.z / gridSize) * gridSize
        );

        alignedPosition.y += up;

        Vector3 forward = Quaternion.Euler(0, currentRotation, 0) * Vector3.forward;
        alignedPosition += forward * forwardOffset;

        buildReference.transform.position = alignedPosition;
        buildReference.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
    }

    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRotation += 90f;
            if (currentRotation >= 360f) currentRotation = 0f;

            if (buildReference != null)
            {
                buildReference.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
            }
        }
    }

    private void CheckOverlap()
    {
        BoxCollider boxCollider = buildReference.GetComponent<BoxCollider>();
        if (boxCollider != null)
        {
            Collider[] colliders = Physics.OverlapBox(
                buildReference.transform.position + boxCollider.center,
                boxCollider.size / 2,
                buildReference.transform.rotation
            );

            if (colliders.Length > 5)
            {
                cantBuild = false;
            }
        }
    }
}
