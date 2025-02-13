using UnityEngine;

[CreateAssetMenu(fileName = "New Requirement", menuName = "The Island/Crafting/Requirement")]
public class CraftingRequirements : ScriptableObject
{
    public ItemSO data;
    public int amount;
}
