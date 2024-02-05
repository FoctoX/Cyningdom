using UnityEngine;

public class HealthBarScript : MonoBehaviour
{
    public static HealthBarScript Instance { get; private set; }

    private EnemyScript enemyScript;
    private float currentHealth;
    private float maxHealth;
    [SerializeField] Transform healthBar;

    private void Awake()
    {
        Instance = this;
        enemyScript = GetComponentInParent<EnemyScript>();
        currentHealth = enemyScript.health;
        maxHealth = enemyScript.health;
    }

    public void HealthbarSystem()
    {
        currentHealth = enemyScript.health;
        healthBar.localScale = new Vector2 (currentHealth/maxHealth, healthBar.localScale.y);
    }
}
