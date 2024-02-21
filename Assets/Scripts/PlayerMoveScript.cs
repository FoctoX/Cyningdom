using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMoveScript : MonoBehaviour
{
    public int experienceLevel;
    public float health, maxHealth, energy, maxEnergy, exp, maxExp;
    public float energyRegenRate; 

    private SpriteRenderer sprite;
    [SerializeField] private Material normal;
    [SerializeField] private Material white;
    private Coroutine takeHitRoutine;

    private Rigidbody2D rb;
    public Animator anim;
    private Collider2D playerColl;

    [SerializeField] private LayerMask platformMask, enemies, chestMask;

    [SerializeField] private int jumpsRemaining;
    private float jumpTimer;

    private float directionX;

    public float movementSpeed;
    public float jumpForce;

    private Transform attackPoint;
    private float attackRadius;

    private int playerDemage;
    [SerializeField] private TextMesh textDemage;
    public int totalDemage;
    private float critChance;
    [SerializeField] private Transform critParticleObject;
    [SerializeField] private ParticleSystem critParticle;
    private float knockbackPower;

    private float shakeIntens, shakeTime;

    public bool canDo = true;
    public bool life = true;

    private enum MovementState { idle, running, jumping, falling };

    public float currentWeapon;
    public float hadWeapon = 0;

    private bool wKey = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        playerColl = GetComponent<Collider2D>();
        attackPoint = transform.Find("Attack Radius");
        critParticleObject = transform.Find("Critical Particle");
        critParticle = critParticleObject.GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        LevelUp();
        health = PlayerPrefs.GetFloat("health");
        energy = PlayerPrefs.GetFloat("energy");
        exp = PlayerPrefs.GetFloat("exp");
        if (health <= 0)
        {
            PlayerPrefs.SetFloat("health", maxHealth);
            PlayerPrefs.SetFloat("energy", maxEnergy);
            PlayerPrefs.SetFloat("exp", maxExp);
            health = PlayerPrefs.GetFloat("health");
            energy = PlayerPrefs.GetFloat("energy");
        }
        else
        {
            PlayerPrefs.SetFloat("health", health);
            PlayerPrefs.SetFloat("energy", maxEnergy);
            PlayerPrefs.SetFloat("exp", exp);
            health = PlayerPrefs.GetFloat("health");
            energy = PlayerPrefs.GetFloat("energy");
            exp = PlayerPrefs.GetFloat("exp");
        }
        LevelUp();
    }

    private void Update()
    {
        PlayerWeapon();
        PlayerAnimation();
        Interact();
        directionX = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            jumpTimer = .25f;
            jumpsRemaining -= 1;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            wKey = true;
        }
        else
        {
            wKey = false;
        }
    }

    private void FixedUpdate()
    {
        PlayerMove();
        PlayerStatusUpdate();
    }

    private void PlayerStatusUpdate()
    {
        energy = Mathf.Clamp(energy + energyRegenRate, 0f, maxEnergy);
    }


    public void LevelUp()
    {
        if (experienceLevel == 0)
        {
            experienceLevel = 1;
        }
        switch (experienceLevel)
        {
            case 1:
                maxHealth = 250f;
                maxEnergy = 25f;
                energyRegenRate = .01f;
                maxExp = 100f;
                break;
            case 2:
                maxHealth = 500f;
                maxEnergy = 50f;
                energyRegenRate = .025f;
                maxExp = 250f;
                break;
            case 3:
                maxHealth = 1000f;
                maxEnergy = 100f;
                energyRegenRate = .1f;
                maxExp = 500f;
                break;
            case 4:
                maxHealth = 1500f;
                maxEnergy = 150f;
                energyRegenRate = .5f;
                maxExp = 750f;
                break;
            case 5:
                maxHealth = 3000f;
                maxEnergy = 500f;
                energyRegenRate = 1f;
                maxExp = 1000f;
                break;
        }
        PlayerPrefs.SetInt("level", experienceLevel);
    }

    private void PlayerMove()
    {
        if (canDo)
        {
            rb.velocity = new Vector2(directionX * movementSpeed, rb.velocity.y);

            if (Grounded())
            {
                jumpsRemaining = 2;
            }

            if (wKey && jumpsRemaining > 0 && jumpTimer > 0)
            {
                jumpTimer -= .01f;
                float jumpHeight = Mathf.Lerp(jumpForce, 0f, 1f - (jumpTimer/.25f));
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
            }
        }

        if (transform.localScale.x > 0)
        {
            critParticleObject.transform.localScale = new Vector2(1, critParticleObject.transform.localScale.y);
        }
        else
        {
            critParticleObject.transform.localScale = new Vector2(-1, critParticleObject.transform.localScale.y);
        }
    }

    private void PlayerAnimation()
    {
        MovementState moveState;

        if (directionX < 0)
        {
            moveState = MovementState.running;
            if (rb.velocity.x < 1) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, transform.localScale.z);
        }
        else if (directionX > 0)
        {
            moveState = MovementState.running;
            if (rb.velocity.x > 1) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            moveState = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            moveState = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            moveState = MovementState.falling;
        }

        anim.SetInteger("state", (int)moveState);
    }

    private void Interact()
    {
        Collider2D chestNearby = Physics2D.OverlapCircle(transform.position, 2, chestMask);
        if (chestNearby != null) 
        {
            ChestScript chestScript = chestNearby.GetComponent<ChestScript>();
            if (chestNearby && Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Chest Opened");
                chestScript.Open();
            }
        }
    }

    private void PlayerWeapon()
    {
        if (canDo)
        {
            if (Input.GetKeyDown(KeyCode.Q) && hadWeapon > 0)
            {
                currentWeapon = Mathf.Repeat(currentWeapon, hadWeapon) + 1;
                GameManager.Instance.IconChange();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                switch (currentWeapon)
                {
                    case 1:
                        if (energy >= 1f)
                        {
                            anim.SetTrigger("atkSmall");
                            attackPoint.transform.localPosition = new Vector2(.17f, -.03f);
                            attackRadius = 1.1f;
                            playerDemage = 25;
                            shakeIntens = .5f;
                            shakeTime = .05f;
                            knockbackPower = 5f;
                            energy -= 1f;
                        }
                        break;
                    case 2:
                        if (energy >= 5f)
                        {
                            anim.SetTrigger("atk");
                            attackPoint.transform.localPosition = new Vector2(.3f, 0f);
                            attackRadius = 1.75f;
                            playerDemage = 100;
                            shakeIntens = 1f;
                            shakeTime = .1f;
                            knockbackPower = 20f;
                            energy -= 10f;
                        }
                        break;
                    case 3:
                        if (energy >= 25f)
                        {
                            anim.SetTrigger("atkBig");
                            attackPoint.transform.localPosition = new Vector2(.37f, .15f);
                            attackRadius = 2.75f;
                            playerDemage = 500;
                            shakeIntens = 5f;
                            shakeTime = .15f;
                            knockbackPower = 100f;
                            energy -= 25f;
                        }
                        break;
                }
            }
        }
    }

    private IEnumerator PlayerCombatRoutine()
    {
        Collider2D[] hittedEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemies);

        foreach (Collider2D enemy in hittedEnemies)
        {
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
            EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
            Rigidbody2D rbEnemy = enemy.GetComponent<Rigidbody2D>();
            critChance = Random.Range(0f,1f);
            if (enemyScript.life)
            {
                if (critChance < .1f)
                {
                    if (enemyScript.anim != null)
                    {
                        enemyScript.anim.SetTrigger("takeHit");
                        enemyScript.attackState = 0;
                    }
                    totalDemage = playerDemage * 2;
                    enemyScript.health = Mathf.Clamp(enemyScript.health - totalDemage, 0f, enemyScript.maxHealth);
                    if (!enemyScript.boss)
                    {
                        HealthBarScript healthbarScript = enemy.gameObject.transform.parent.gameObject.transform.Find("Healthbar").gameObject.GetComponent<HealthBarScript>();
                        if (healthbarScript != null) healthbarScript.HealthbarSystem();
                    }
                    CinemachineControllerScript.Instance.CameraShake(shakeIntens * 2, shakeTime * 2);
                    critParticle.Play();
                    TextCriticalDemageSystem();
                    enemyScript.FlashHitted();
                    enemyScript.EnemyCondition();
                    Time.timeScale = 0f;
                    if (rbEnemy != null)
                    {
                        rbEnemy.AddForce(transform.up * knockbackPower * 2, ForceMode2D.Impulse);
                        rbEnemy.AddForce(knockbackDir * knockbackPower * 2, ForceMode2D.Impulse);
                    }
                    yield return new WaitForSecondsRealtime(shakeTime);
                    Time.timeScale = 1f;
                }
                else if (enemyScript.life)
                {
                    totalDemage = playerDemage;
                    if ((totalDemage / enemyScript.maxHealth) >= .25f && enemyScript.anim != null)
                    {
                        enemyScript.anim.SetTrigger("takeHit");
                        enemyScript.attackState = 0;
                    }
                    enemyScript.health = Mathf.Clamp(enemyScript.health - totalDemage, 0f, enemyScript.maxHealth);
                    if (!enemyScript.boss)
                    {
                        HealthBarScript healthbarScript = enemy.gameObject.transform.parent.gameObject.transform.Find("Healthbar").gameObject.GetComponent<HealthBarScript>();
                        if (healthbarScript != null) healthbarScript.HealthbarSystem();
                    }
                    CinemachineControllerScript.Instance.CameraShake(shakeIntens, shakeTime);
                    TextDemageSystem();
                    enemyScript.FlashHitted();
                    enemyScript.EnemyCondition();
                    Time.timeScale = 0;
                    if (rbEnemy != null)
                    {
                        rbEnemy.AddForce(transform.up * knockbackPower, ForceMode2D.Impulse);
                        rbEnemy.AddForce(knockbackDir * knockbackPower, ForceMode2D.Impulse);
                    }
                    yield return new WaitForSecondsRealtime(shakeTime);
                    Time.timeScale = 1;
                }
            }
        }

        yield return null;
    }

    public void TakeDemage()
    {
        if (takeHitRoutine != null) { StopCoroutine(TakeDemageRoutine()); }
        StartCoroutine(TakeDemageRoutine());
    }

    public IEnumerator TakeDemageRoutine()
    {
        sprite.material = white;

        yield return new WaitForSecondsRealtime(.1f);

        sprite.material = normal;

        if (health <= 0f)
        {            
            life = false;
            canDo = false;
            anim.SetTrigger("dead");
        }

        yield return null;
    }

    private void CannotMove()
    {
        canDo = false;
    }

    private void CanMove()
    {
        canDo = true;
    }

    public void TextDemageSystem()
    {
        textDemage.color = Color.white;
        textDemage.fontSize = 100;
        textDemage.text = totalDemage.ToString();
    }

    public void TextCriticalDemageSystem()
    {
        textDemage.color = Color.red;
        textDemage.fontSize = 150;
        textDemage.text = totalDemage.ToString();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Trap")
        {
            health = 0f;
            GameManager.Instance.PlayerConditionUI();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Trap")
        {
            health = 0f;
            GameManager.Instance.PlayerConditionUI();
        }
    }

    private bool Grounded()
    {
        return Physics2D.BoxCast(playerColl.bounds.center, playerColl.bounds.size, 0f, Vector2.down, 0.01f, platformMask);
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null) { return; }
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
