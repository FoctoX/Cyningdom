using UnityEngine;

public class HealthBarScript : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject spriteGFX;
    [SerializeField] private EnemyScript enemyScript;
    private float healthbarPositionYFix;
    private Vector3 currentScale;
    private Vector3 dissappearScale = new Vector3(0f,0f,0f);
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private Transform healthBar;

    private void Awake()
    {
        healthbarPositionYFix = transform.position.y;
        parent = gameObject.transform.parent.gameObject;
        spriteGFX = parent.transform.Find("Sprite").gameObject;
        currentScale = transform.localScale;
        enemyScript = spriteGFX.GetComponent<EnemyScript>();
        currentHealth = enemyScript.health;
        maxHealth = enemyScript.health;
        healthBar = gameObject.transform.Find("Health");
        HealthbarSystem();
    }

    public void Update()
    {
        transform.position = new Vector2(spriteGFX.transform.position.x, spriteGFX.transform.position.y + healthbarPositionYFix);
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
