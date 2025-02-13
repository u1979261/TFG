using System.Diagnostics.Contracts;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public RecipieTemplate recipieTemplate;
    public CraftingRecipieSO[] recipies;

    public Transform contentHolder;

    private void Start()
    {
        RecepiesGenerator();
    }
    public void RecepiesGenerator()
    {
        for (int i = 0; i < recipies.Length; i++)
        {
            RecipieTemplate recipie = Instantiate(recipieTemplate.gameObject, contentHolder).GetComponent<RecipieTemplate>();

            recipie.icon.sprite = recipies[i].recipieIcon;
            recipie.nameText.text = recipies[i].recipieName;

            for (int j = 0; j < recipies[i].requirements.Length; j++)
            {
                if(j == 0)
                {
                    recipie.requirementsText.text = $"{recipies[i].requirements[j].data.itemName} {recipies[i].requirements[j].amount}";
                }
                else
                {
                    recipie.requirementsText.text = $"{recipie.requirementsText.text}, {recipies[i].requirements[j].data.itemName} {recipies[i].requirements[j].amount}";
                }
            }
        }
    }
}
