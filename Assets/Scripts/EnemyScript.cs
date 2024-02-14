using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyScript : MonoBehaviour
{
    public float health;
    public float maxHealth;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform detectPoint;
    [SerializeField] private Transform detectGround;

    [SerializeField] private float demage = 20;
    [SerializeField] private float knockbackPower;

    [SerializeField] private float attackRadius;
    [SerializeField] private float detectRadius;
    [SerializeField] private float groundRadius;

    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask platformMask;
    [SerializeField] private LayerMask borderMask;

    public Animator anim;
    private enum MovementState { idle, running };

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material normal;
    [SerializeField] private Material flash;
    private Coroutine flashCoroutine;
    public bool canHitted;
    [SerializeField] private GameObject textDemage;
    [SerializeField] private float textDemageFix;

    [SerializeField] private GameObject targetObject;
    [SerializeField] private Transform target;
    [SerializeField] private float speed;
    [SerializeField] private float sprintSpeed;

    private bool canDo = true;
    public bool life = true;

    private PlayerMoveScript playerMoveScript;

    [SerializeField] private bool Boss;

    private void Awake()
    {
        targetObject = GameObject.FindGameObjectWithTag("Player");
        target = targetObject.transform;
        attackPoint = transform.Find("Attack Radius");
        detectPoint = transform.Find("Detect Radius");
        detectGround = transform.Find("Ground Check");
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        playerMoveScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveScript>();
        maxHealth = health;
    }

    private void Start()
    {

    }

    private void FixedUpdate()
    {
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

        if (canDo && isPlayerOnArea() && playerMoveScript.life)
        {
            sprintSpeed = speed * 2;
            if (target.position.x - 1 >= transform.position.x)
            {
                rb.velocity = new Vector2(Mathf.Abs(sprintSpeed), transform.position.y);
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
            }
            else if (target.position.x + 1 <= transform.position.x)
            {
                rb.velocity = new Vector2(Mathf.Abs(sprintSpeed) * -1, transform.position.y);
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
        }

        else if (canDo && !isPlayerOnArea() && Grounding())
        {
            if (rb.velocity.x > 1f)
            {
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
            }
            else if (rb.velocity.x < -1f)
            {
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
            if (edgeGround())
            {
                rb.velocity = new(speed, transform.position.y);
            }
            else
            {
                speed *= -1;
                rb.velocity = new(speed, transform.position.y);
            }
            if (!BorderCheck())
            {
                rb.velocity = new(speed, transform.position.y);
            }
            else
            {
                speed *= -1;
                rb.velocity = new(speed, transform.position.y);
            }
        }

        anim.SetInteger("state", (int)state);
    }

    private void AttackOnAnim()
    {
        if (Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerMask) && canDo && playerMoveScript.life)
        {
            anim.SetTrigger("attack");
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
        float timer = 1;
        float restOfTimer = 0;
        float value;

        anim.SetTrigger("dead");

        yield return new WaitForSeconds(1f);

        while (restOfTimer < timer)
        {
            restOfTimer += Time.deltaTime;
            value = (restOfTimer / timer);
            spriteRenderer.color = new Color(1,1,1, value);
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
        Collider2D playerOnArea = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerMask);
        if (playerOnArea != null)
        {
            PlayerMoveScript playerMoveScript = playerOnArea.GetComponent<PlayerMoveScript>();
            Rigidbody2D rbPlayer = playerOnArea.GetComponent<Rigidbody2D>();
            Vector2 knockbackDirX = (playerOnArea.transform.position - transform.position).normalized;
            if (playerMoveScript != null)
            {
                playerMoveScript.TakeDemage();
                playerMoveScript.health -= demage;
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

    private void BossConditions()
    {
        if (Boss)
        {
            if (health / maxHealth < .25f && Boss == true)
            {
                Boss = false;
                speed *= 2;
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

    private bool isPlayerOnArea()
    {
        return Physics2D.CircleCast(detectPoint.position, detectRadius, Vector2.zero, 0.1f, playerMask);
    }

    private bool Grounding()
    {
        return Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, 0.1f, platformMask);
    }

    private bool edgeGround()
    {
        return Physics2D.CircleCast(detectGround.position, groundRadius, Vector2.one, 0.1f, platformMask);
    }

    private bool BorderCheck(){
        return Physics2D.CircleCast(detectGround.position, groundRadius, Vector2.one, 0.1f, borderMask);
    }

    private void OnDrawGizmos()
    {
        if (attackPoint == null || detectPoint == null || detectGround == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        Gizmos.DrawWireSphere(detectPoint.position, detectRadius);
        Gizmos.DrawWireSphere(detectGround.position, groundRadius);
    }
}
