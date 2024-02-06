using System.Collections;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    public static PlayerMoveScript Instance { get; private set; }

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private Collider2D playerColl;
    [SerializeField] private LayerMask platformMask;

    private int maxJumps = 2;
    private int jumpsRemaining;
    private float jumpTimer;
    [SerializeField] private AnimationCurve jumpCurve;

    float directionX;
    public float movementSpeed;
    public float jumpForce;
    public float dashSpeed;
    public int attackState;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask enemies;

    [SerializeField] private int playerDemage;
    [SerializeField] private TextMesh textDemage;
    public int totalDemage;
    private float critChance;
    [SerializeField] private GameObject critParticleObject;
    private ParticleSystem critParticle;
    [SerializeField] private float knockbackPower;

    private bool canAttack = true;
    private bool canMove = true;

    private enum MovementState { idle, running, jumping, falling };

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        playerColl = GetComponent<Collider2D>();
        critParticle = critParticleObject.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        PlayerMove();
        PlayerAnimation();
    }

    private void PlayerMove()
    {
        if (canMove)
        {
            directionX = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(directionX * movementSpeed, rb.velocity.y);

            if (Grounded())
            {
                jumpsRemaining = maxJumps;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                jumpTimer = .25f;
                jumpsRemaining--;
                rb.velocity = new Vector2(jumpForce, rb.velocity.y);
            }

            if (Input.GetKey(KeyCode.W) && jumpsRemaining > 0 && jumpTimer >= 0f)
            {
                jumpTimer -= Time.deltaTime;
                float jumpHeight = Mathf.Lerp(jumpForce, 0f, 1 - (jumpTimer/.25f));
                rb.velocity += Vector2.up * jumpHeight;
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
        MovementState state;

        if (directionX < 0)
        {
            state = MovementState.running;
            transform.localScale = new Vector3(-7, transform.localScale.y, transform.localScale.z); 
        }
        else if (directionX > 0)
        {
            state = MovementState.running;
            transform.localScale = new Vector3(7, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);

        if (canAttack && Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("attack");
        }
    }

    private IEnumerator PlayerCombatRoutine()
    {
        Collider2D[] hittedEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, enemies);

        foreach (Collider2D enemy in hittedEnemies)
        {
            critChance = Random.Range(0f,1f);
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;
            EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
            Rigidbody2D rbEnemy = enemy.GetComponent<Rigidbody2D>();
            if (critChance < .1f)
            {
                Debug.Log("Critical" + critChance);
                totalDemage = playerDemage * 2;
                enemyScript.health -= totalDemage;
                HealthBarScript.Instance.HealthbarSystem();
                CinemachineControllerScript.Instance.CameraShake(5f, .25f);
                critParticle.Play();
                TextCriticalDemageSystem();
                enemyScript.FlashHitted();
                enemyScript.EnemyCondition();
                Time.timeScale = 0f;
                yield return new WaitForSecondsRealtime(0.25f);
                if (rbEnemy != null)
                {
                    rbEnemy.AddForce(Vector2.up * knockbackPower * 2, ForceMode2D.Impulse);
                    rbEnemy.AddForce(knockbackDir * knockbackPower * 2, ForceMode2D.Impulse);
                }
                Time.timeScale = 1f;
            }
            else
            {
                totalDemage = playerDemage;
                enemyScript.health -= playerDemage;
                HealthBarScript.Instance.HealthbarSystem();
                CinemachineControllerScript.Instance.CameraShake(1f, .1f);
                TextDemageSystem();
                enemyScript.FlashHitted();
                enemyScript.EnemyCondition();
                Time.timeScale = 0;
                yield return new WaitForSecondsRealtime(0.1f);
                if (rbEnemy != null)
                {
                    rbEnemy.AddForce(Vector2.up * knockbackPower, ForceMode2D.Impulse);
                    rbEnemy.AddForce(knockbackDir * knockbackPower, ForceMode2D.Impulse);
                }
                Time.timeScale = 1;
            }
        }

        yield return null;
    }

    public void AttackStart()
    {
        canMove = false;
        canAttack = false;
    }

    public void AttackEnd()
    {
        canMove = true;
        canAttack = true;
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

    private bool Grounded()
    {
        return Physics2D.BoxCast(playerColl.bounds.center, playerColl.bounds.size, 0f, Vector2.down, 0.1f, platformMask);
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
