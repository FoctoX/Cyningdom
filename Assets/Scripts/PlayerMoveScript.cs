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

    private bool isJumping = false;
    private int maxJumps = 2;
    [SerializeField] private int jumpsRemaining;

    float directionX;
    public float movementSpeed;
    public float jumpForce;
    public float dashSpeed;
    public int attackState;

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

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            rb.velocity = new Vector2(directionX * dashSpeed, rb.velocity.y);
            anim.SetInteger("Attack", attackState++);
            if (attackState >= 3)
            {
                attackState = 0;
            }
        }
    }

    private void PlayerAnimation()
    {
        if (rb.velocity.x < 0)
        {
            sprite.flipX = true;
            anim.SetBool("Run", true);
        }
        if (rb.velocity.x > 0)
        {
            sprite.flipX = false;
            anim.SetBool("Run", true);
        }
        if (rb.velocity.x == 0)
        {
            anim.SetBool("Run", false);
        }
        if (rb.velocity.y > 1)
        {
            anim.SetBool("Jump", true);
        }
        else
        {
            anim.SetBool("Jump", false);
        }

    }

    private bool Grounded()
    {
        return Physics2D.BoxCast(playerColl.bounds.center, playerColl.bounds.size, 0f, Vector2.down, 0.2f, platformMask);
    }
}
