using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
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

    [Header("Blood UI")]
    public Image bloodImage;
    private float r;
    private float g;
    private float b;
    private float a;
    private float minAlpha = 0f;
    private float maxAlpha = 0.9f;
    private float lastDamageTime = -100f;
    private bool recentlyDamaged = false;

    // Temporizadores internos
    private float hungerTimer = 0f;
    private float thirstTimer = 0f;
    private float hungerDamageTimer = 0f;
    private float thirstDamageTimer = 0f;

    public AudioClip damageSound;

    private void Start()
    {
        health = maxHealth;
        hunger = maxhunger;
        thirst = maxThirst;

        r = bloodImage.color.r;
        g = bloodImage.color.g;
        b = bloodImage.color.b;
        a = bloodImage.color.a;
    }

    private void Update()
    {
        UpdateStats();
        UpdateUI();

        if (recentlyDamaged && Time.time - lastDamageTime > 10f)
        {
            bloodImage.color = new Color(r, g, b, 0f);
            recentlyDamaged = false;
        }

        if (health <= 0f)
        {
            GetComponent<PlayerRespawn>().Die();
            health = maxHealth/2;
            hunger = maxhunger/2;
            thirst = maxThirst/2;
        }
    }

    private void UpdateUI()
    {
        healthBar.numberText.text = health.ToString("f0");
        healthBar.bar.fillAmount = health / maxHealth;

        hungerBar.numberText.text = hunger.ToString("f0");
        hungerBar.bar.fillAmount = hunger / maxhunger;

        thirstBar.numberText.text = thirst.ToString("f0");
        thirstBar.bar.fillAmount = thirst / maxThirst;

        if (recentlyDamaged)
        {
            float t = 1f - (health / maxHealth);
            float newAlpha = Mathf.Lerp(minAlpha, maxAlpha, t);
            bloodImage.color = new Color(r, g, b, newAlpha);
        }
    }

    public void OnTakeDamage()
    {
        GetComponent<AudioSource>().PlayOneShot(damageSound);
        lastDamageTime = Time.time;
        recentlyDamaged = true;
    }

    public void UpdateStats()
    {
        float delta = Time.deltaTime;

        // Mantener stats en rango válido
        health = Mathf.Clamp(health, 0f, maxHealth);
        hunger = Mathf.Clamp(hunger, 0f, maxhunger);
        thirst = Mathf.Clamp(thirst, 0f, maxThirst);

        // Acumular tiempo
        hungerTimer += delta;
        thirstTimer += delta;
        hungerDamageTimer += delta;
        thirstDamageTimer += delta;

        // Reducir hambre y sed cada 180 segundos (3 minutos)
        if (hunger > 0f && hungerTimer >= 180f)
        {
            hunger -= hungerReduction;
            hungerTimer = 0f;
        }

        if (thirst > 0f && thirstTimer >= 180f)
        {
            thirst -= thirstReduction;
            thirstTimer = 0f;
        }

        // Aplicar daño cada 60 segundos (1 minuto)
        if (hunger <= 0f && hungerDamageTimer >= 60f)
        {
            health -= hungerDamage;
            hungerDamageTimer = 0f;
        }

        if (thirst <= 0f && thirstDamageTimer >= 60f)
        {
            health -= thirstDamage;
            thirstDamageTimer = 0f;
        }
    }
}
