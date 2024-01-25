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
    private float jumpCooldown = 0.1f; 
    private float jumpCooldownTimer = 0f;
    private int maxJumps = 2;
    private int jumpsRemaining;

    float directionX;
    public float movementSpeed;
    public float jumpForce;
    private Vector2 playerLocation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        playerColl = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        PlayerMove();
        CameraMove();
        PlayerAnimation();
        UpdateJumpCooldown();
    }

    private void PlayerMove()
    {
        directionX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(directionX * movementSpeed, rb.velocity.y);

        if (Grounded())
        {
            jumpsRemaining = maxJumps;
            isJumping = false;
        }

        if (Input.GetKeyDown(KeyCode.W) && jumpsRemaining > 0 && !isJumping)
        {
            jumpsRemaining--;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            jumpCooldownTimer = jumpCooldown;
        }
    }

    private void UpdateJumpCooldown()
    {
        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.fixedDeltaTime;
        }
        else
        {
            isJumping = false;
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
        if (rb.velocity.y > 0.1)
        {
            anim.SetBool("Jump", true);
        }
        else
        {
            anim.SetBool("Jump", false);
        }
    }

    private void CameraMove()
    {
        playerLocation = transform.position;
        Camera.main.transform.position = new Vector3(playerLocation.x, playerLocation.y, Camera.main.transform.position.z);
    }

    private bool Grounded()
    {
        return Physics2D.BoxCast(playerColl.bounds.center, playerColl.bounds.size, 0f, Vector2.down, 0.5f, platformMask);
    }
}
