using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private Collider2D playerColl;
    [SerializeField] private LayerMask platformMask;

    private int maxJumps = 2;
    private int jumpsRemaining;

    float directionX;
    public float movementSpeed;
    public float jumpForce;
    public float dashSpeed;
    public int attackState;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask enemies;

    [SerializeField] private int playerDemage;
    private float critChance;
    private bool canAttack = true;
    private bool canMove = true;

    private enum MovementState { idle, running, jumping, falling };

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        playerColl = GetComponent<Collider2D>();
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

            if (Input.GetKeyDown(KeyCode.W) && jumpsRemaining > 0)
            {
                jumpsRemaining--;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
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
            EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
            if (critChance < .1f)
            {
                Debug.Log("Critical" + critChance);
                enemyScript.health -= playerDemage * 2;
                CinemachineControllerScript.Instance.CameraShake(5f, .25f);
                enemyScript.FlashHitted();
                Time.timeScale = 0f;
                yield return new WaitForSecondsRealtime(0.25f);
                Time.timeScale = 1f;
            }
            else
            {
                enemyScript.health -= playerDemage;
                CinemachineControllerScript.Instance.CameraShake(1f, .1f);
                enemyScript.FlashHitted();
                Time.timeScale = 0;
                yield return new WaitForSecondsRealtime(0.1f);
                Time.timeScale = 1;
            }
            enemyScript.EnemyCondition();
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
