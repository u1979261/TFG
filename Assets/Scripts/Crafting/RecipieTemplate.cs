using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class RecipieTemplate : MonoBehaviour, IPointerDownHandler
{

    private CraftingManager crafting;
    [HideInInspector] public CraftingRecipieSO recipie;

    public TextMeshProUGUI nameText;
    public Image icon;
    public TextMeshProUGUI requirementsText;
    public TextMeshProUGUI timeText;

    private void Start()
    {
        crafting = GetComponentInParent<CraftingManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            crafting.Craft(this);
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            crafting.Cancel(this);
        }
    }
}
