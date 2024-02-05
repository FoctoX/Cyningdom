using UnityEngine;

public class HealthBarScript : MonoBehaviour
{
    public static HealthBarScript Instance { get; private set; }

    private EnemyScript enemyScript;
    private Vector3 currentScale;
    private Vector3 dissappearScale = new Vector3(0f,0f,0f);
    private float currentHealth;
    private float maxHealth;
    [SerializeField] Transform healthBar;

    private void Awake()
    {
        Instance = this;
        currentScale = transform.localScale;
        enemyScript = GetComponentInParent<EnemyScript>();
        currentHealth = enemyScript.health;
        maxHealth = enemyScript.health;
        HealthbarSystem();
    }

    public void HealthbarSystem()
    {
        currentHealth = enemyScript.health;
        healthBar.localScale = new Vector2(currentHealth / maxHealth, healthBar.localScale.y);
        if (currentHealth == maxHealth)
        {
            transform.localScale = dissappearScale;
        }
        else
        {
            transform.localScale = currentScale;
        }
    }
}
