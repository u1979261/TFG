using UnityEngine;

public class ResourceExtenssion : MonoBehaviour
{
    public void Recolect(ItemSO toolUsed, InventoryManager invetory)
    {
        if(GetComponentInParent<ResourceObject>() == null)
        {
            return;
        }
        GetComponentInParent<ResourceObject>().Recolect(toolUsed, invetory);
    }
}
