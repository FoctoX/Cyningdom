using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject playerSprite;
    private SpriteRenderer sprite;
    private Animator anim;

    float directionX;
    public float movementSpeed;
    public float jumpForce;
    private Vector2 playerLocation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = playerSprite.GetComponent<SpriteRenderer>();
        anim = playerSprite.GetComponent<Animator>();
    }

    // FixedUpdate digunakan untuk menggerakkan objek dengan Rigidbody
    void FixedUpdate()
    {
        PlayerMove();
        CameraMove();
        PlayerAnimation();
    }

    private void PlayerMove()
    {
        directionX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(directionX * movementSpeed, rb.velocity.y);

        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); 
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
    }

    private void CameraMove()
    {
        playerLocation = transform.position;
        Camera.main.transform.position = new Vector3(playerLocation.x, playerLocation.y, Camera.main.transform.position.z);
    }
}
