using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header ("Stats")]
    public float health;
    public float maxHealth = 100f;

    [Space]
    public float hunger;
    public float maxhunger = 100f;

    [Space]
    public float thirst;
    public float maxThirst = 100f;

    [Header("Stats Reductions")]
    public float hungerReduction = 0.5f;
    public float thirstReduction = 0.5f;

    [Header("Stats Damages")]
    public float hungerDamage = 0.5f;
    public float thirstDamage = 0.5f;

    [Header("Stats UI")]
    public StatsBar healthBar;
    public StatsBar hungerBar;
    public StatsBar thirstBar;
    private void Start()
    {
        health = maxHealth;
        hunger = maxhunger;
        thirst = maxThirst;
    }

    private void Update()
    {
        UpdateStats();
        UpdateUI();
    }
    private void UpdateUI()
    { 
        healthBar.numberText.text = health.ToString("f0");
        healthBar.bar.fillAmount = health / 100;

        hungerBar.numberText.text = hunger.ToString("f0");
        hungerBar.bar.fillAmount = hunger / 100;

        thirstBar.numberText.text = thirst.ToString("f0");
        thirstBar.bar.fillAmount = thirst / 100;

    }

    public void UpdateStats()
    {
        //--STATS--//
        if (health <= 0f)
            health = 0f;
        if (health >= maxHealth)
            health = maxHealth;

        if (hunger <= 0f)
            hunger = 0f;
        if (hunger >= maxhunger)
            hunger = maxhunger;

        if (thirst <= 0f)
            thirst = 0f;
        if (thirst >= maxThirst)
            thirst = maxThirst;

        //--REDUCTIONS--//

        if (hunger <= 0) {
            //EVERY 3 SEC//
            if (Time.time % 10f < Time.deltaTime)
            {
                health -= hungerDamage;
            }
            //health -= hungerDamage * Time.deltaTime;
        }
        if (thirst <= 0)
        {
            //EVERY 3 SEC//
            if (Time.time % 10f < Time.deltaTime)
            {
                health -= thirstDamage;
            }
            //health -= thirstDamage * Time.deltaTime;
        }

        //--DAMAGES--//
        if(hunger > 0)
        {
            //EVERY 5 SEC//
            if (Time.time % 60f < Time.deltaTime)
            {
                hunger -= hungerReduction;
            }
            //hunger -= hungerReduction * Time.deltaTime;
        }
        if (thirst > 0)
        {
            //EVERY 5 SEC//
            if (Time.time % 60f < Time.deltaTime)
            {
                thirst -= thirstReduction;
            }
            //thirst -= thirstReduction * Time.deltaTime;
        }

    }
}
