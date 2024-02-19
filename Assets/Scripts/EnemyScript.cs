using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyScript : MonoBehaviour
{
    public string bossName;

    public float health;
    public float maxHealth;
    [SerializeField] private float jumpForce;

    public float attackCooldown;
    private float attackCooldownTemp;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform detectPoint;
    [SerializeField] private Transform detectGround;
    [SerializeField] private Transform detectFrontGround;

    [SerializeField] private float demage = 20;
    private float demageBoss;
    [SerializeField] private float knockbackPower;

    [SerializeField] private Vector2 attackRadius;
    [SerializeField] private float detectRadius;
    [SerializeField] private float groundRadius;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask platformMask;
    [SerializeField] private LayerMask borderMask;

    public Animator anim;
    private enum MovementState { idle, running };
    public int attackState = 0;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material normal;
    [SerializeField] private Material flash;
    private Coroutine flashCoroutine;
    [HideInInspector] public bool canHitted;
    [SerializeField] private float distanceAgainstPlayer;
    [SerializeField] private GameObject textDemage;
    private float textDemageFix;

    [SerializeField] private GameObject targetObject;
    [SerializeField] private Transform target;
    [SerializeField] private float speed;
    private float sprintSpeed;

    [SerializeField] private GameObject chestReward;
    [SerializeField] private float chestPositionFixY;

    [SerializeField] private bool patrol;
    private bool canDo = true;
    public bool life = true;

    private PlayerMoveScript playerMoveScript;

    [SerializeField] private bool miniBoss;
    [SerializeField] public bool boss;

    private void Awake()
    {
        targetObject = GameObject.FindGameObjectWithTag("Player");
        target = targetObject.transform;
        attackPoint = transform.Find("Attack Radius");
        detectPoint = transform.Find("Detect Radius");
        detectGround = transform.Find("Ground Check");
        detectFrontGround = transform.Find("Front Platform Check");
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerMoveScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveScript>();
        maxHealth = health;
        attackCooldownTemp = attackCooldown;
    }

    private void FixedUpdate()
    {
        BossAbility();
        if (anim != null)
        {
            EnemyMove();
            AttackOnAnim();
        }
    }

    private void EnemyMove()
    {
        MovementState state;

        if (rb.velocity.x > .1f || rb.velocity.x < -.1f)
        {
            state = MovementState.running;
        }
        else
        {
            state = MovementState.idle;
        }

        if (canDo && Physics2D.CircleCast(detectPoint.position, detectRadius, Vector2.zero, 0.1f, playerMask) && playerMoveScript.life)
        {
            if (!boss && !miniBoss) sprintSpeed = speed * 2;
            if (target.position.x >= transform.position.x)
            {
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
            }
            else if (target.position.x <= transform.position.x)
            {
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
            if (Physics2D.Raycast(detectGround.position, Vector2.left, Mathf.Infinity, platformMask).distance < .05f && Physics2D.Raycast(detectFrontGround.position, Vector2.down, Mathf.Infinity, platformMask).collider == null)
            {
                Debug.Log("Stop");
                rb.velocity = Vector2.zero;
            }
            else if (target.position.x - distanceAgainstPlayer >= transform.position.x)
            {
                rb.velocity = new Vector2(Mathf.Abs(sprintSpeed), rb.velocity.y);
                if (Physics2D.BoxCast(detectFrontGround.position, new Vector2(.1f, .1f), 0f, Vector2.left, 1f, platformMask) && Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, platformMask))
                {
                    Debug.Log("touching");
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                } 
            }
            else if (target.position.x + distanceAgainstPlayer <= transform.position.x)
            {
                rb.velocity = new Vector2(Mathf.Abs(sprintSpeed) * -1, rb.velocity.y);
                if (Physics2D.BoxCast(detectFrontGround.position, new Vector2(.1f, .1f), 0f, Vector2.left, 1f, platformMask) && Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, platformMask))
                {
                    Debug.Log("touching");
                    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
            }
        }

        else if (canDo && !Physics2D.CircleCast(detectPoint.position, detectRadius, Vector2.zero, 0.1f, playerMask) && Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, platformMask) && patrol)
        {
            if (rb.velocity.x > 1f)
            {
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
            }
            else if (rb.velocity.x < -1f)
            {
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
            if (Physics2D.CircleCast(detectGround.position, groundRadius, Vector2.one, 0.1f, platformMask))
            {
                rb.velocity = new(speed, rb.velocity.y);
            }
            else
            {
                speed *= -1;
                rb.velocity = new(speed, rb.velocity.y);
            }
            if (!Physics2D.CircleCast(detectGround.position, groundRadius, Vector2.one, 0.1f, borderMask))
            {
                rb.velocity = new(speed, rb.velocity.y);
            }
            else
            {
                speed *= -1;
                rb.velocity = new(speed, rb.velocity.y);
            }
        }

        anim.SetInteger("state", (int)state);
    }

    private void AttackOnAnim()
    {
        attackCooldown -= Time.deltaTime;
        if (Physics2D.OverlapBox(attackPoint.position, attackRadius, 0f, playerMask) && canDo && playerMoveScript.life && attackCooldown < 0)
        {
            anim.SetTrigger("attack");
            attackCooldown = attackCooldownTemp;
        }
    }

    public void EnemyCondition()
    {
        if (health <= 0)
        {
            StartCoroutine("EnemyDied");
        }
    }

    private IEnumerator EnemyDied()
    {
        life = false;
        canDo = false;

        anim.SetTrigger("dead");
        if (boss)
        {
            Instantiate(chestReward, new Vector3(transform.position.x, transform.position.y + chestPositionFixY, transform.position.z), Quaternion.identity);
        }

        yield return new WaitForSeconds(1f);

        float elapsedTime = 0f;
        float fadeDuration = 2f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            spriteRenderer.color = new Color(1,1,1, Mathf.Lerp(0f, 1f, 1 - (elapsedTime / fadeDuration)));
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        Destroy(transform.parent.gameObject);

        yield return null;
    }

    public void FlashHitted()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(FlashRoutine());
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        canHitted = false;
        spriteRenderer.material = flash;

        GameObject textDemageInstance = Instantiate(textDemage, this.transform.position, Quaternion.identity);
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        Vector3 randomOffset = new Vector3(randomX, randomY + textDemageFix, 0f);
        randomOffset += this.transform.position;
        textDemageInstance.transform.localPosition = randomOffset;

        yield return new WaitForSeconds(0.125f);

        spriteRenderer.material = normal;

        canHitted = true;
        flashCoroutine = null;
    }

    private IEnumerator Attack()
    {
        Collider2D playerOnArea = Physics2D.OverlapBox(attackPoint.position, attackRadius, 0f, playerMask);
        if (playerOnArea != null)
        {
            PlayerMoveScript playerMoveScript = playerOnArea.GetComponent<PlayerMoveScript>();
            Rigidbody2D rbPlayer = playerOnArea.GetComponent<Rigidbody2D>();
            Vector2 knockbackDirX = (playerOnArea.transform.position - transform.position).normalized;
            if (playerMoveScript != null)
            {
                playerMoveScript.TakeDemage();
                if (!boss)
                {
                    playerMoveScript.health -= demage;
                }
                else if (transform.parent.name == "King" && boss)
                {
                    switch (attackState)
                    {
                        case 0:
                            demageBoss = demage;
                            attackPoint.localPosition = new Vector2(-0.2f, -0.25f);
                            attackRadius = new Vector2(7.08f, 2.6f);
                            anim.SetInteger("attackState", attackState);
                            attackState = 1;
                            break;
                        case 1:
                            demageBoss = demage * 1.5f;
                            attackPoint.localPosition = new Vector2(.07f, -.24f);
                            attackRadius = new Vector2(11, 3.8f);
                            anim.SetInteger("attackState", attackState);
                            attackState = 2;
                            break;
                        case 2:
                            demageBoss = demage * 2;
                            attackPoint.localPosition = new Vector2(-0.29f, -0.01f);
                            attackRadius = new Vector2(6.44f, 7.4f);
                            anim.SetInteger("attackState", attackState);
                            attackState = 0;
                            break;
                    }
                    playerMoveScript.health -= demageBoss;
                }

                if ((demage / playerMoveScript.health) >= .25f && anim != null)
                {
                    playerMoveScript.anim.SetTrigger("takeHit");
                }
                rbPlayer.AddForce(knockbackDirX * knockbackPower, ForceMode2D.Impulse);
                rbPlayer.AddForce(Vector2.up * knockbackPower / 1.5f, ForceMode2D.Impulse);
            }
        }
        yield return null;
    }

    private void BossAbility()
    {
        if (boss || miniBoss)
        {
            sprintSpeed = speed * 2;
            if (health / maxHealth < .5f)
            {
                sprintSpeed = speed * 3; 
            }
        }
    }

    private void CanDo()
    {
        canDo = true;
    }

    private void CannotDo()
    {
        canDo = false;
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null || detectPoint == null || detectGround == null)
        {
            return;
        }
        Gizmos.DrawWireCube(attackPoint.position, attackRadius);
        Gizmos.DrawWireSphere(detectPoint.position, detectRadius);
        Gizmos.DrawWireSphere(detectGround.position, groundRadius);
    }
}
