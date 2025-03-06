using UnityEngine;

public class BuildingHandler : MonoBehaviour
{
    public Slot slotInUse;
    public Transform offGroundPoint;
    [Space]
    public float range = 5f;
    public Color allowed;
    public Color blocked;
    [Space]
    public BuildReference buildReference;
    public bool cantBuild;
    private float currentRotation = 0f;

    private void Update()
    {
        UpdateBuilding();
        HandleRotation();
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
                if (buildReference.buildPrefab.CompareTag("Foundation"))
                {
                    AlignToGrid(hit.point, hit.normal, 0.825f, 0f);
                }
                else if (buildReference.buildPrefab.CompareTag("Wall"))
                {
                    AlignToGrid(hit.point, hit.normal, 0.825f, 0.25f);//0.80f;
                }
                else if (buildReference.buildPrefab.CompareTag("Box"))
                {
                    AlignToGrid(hit.point, hit.normal, 0.825f, 0.25f);
                }

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

        Debug.Log($"CantBuild: {cantBuild}, BuildPrefab: {buildReference.buildPrefab}");

        if (Input.GetMouseButtonDown(0) && cantBuild && buildReference.canBuild && buildReference.buildPrefab != null)
        {
            slotInUse.stackSize--;
            slotInUse.UpdateSlot();
            Instantiate(buildReference.buildPrefab, buildReference.transform.position, buildReference.transform.rotation);
        }
    }

    private void AlignToGrid(Vector3 position, Vector3 normal, float gridSize,float up)
    {
        Vector3 alignedPosition = new Vector3(
            Mathf.Round(position.x / gridSize) * gridSize,
            Mathf.Round(position.y / gridSize) * gridSize,
            Mathf.Round(position.z / gridSize) * gridSize
        );
        alignedPosition.y += up;
        buildReference.transform.position = alignedPosition;
        buildReference.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
    }

    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentRotation += 90f;
            if (currentRotation >= 360f)
            {
                currentRotation = 0f;
            }
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

            if (colliders.Length > 1)
            {
                cantBuild = false;
            }
        }
    }
}
