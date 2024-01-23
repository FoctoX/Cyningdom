using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public float movementSpeed;
    private Vector2 playerLocation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // FixedUpdate digunakan untuk menggerakkan objek dengan Rigidbody
    void FixedUpdate()
    {
        PlayerMove();
        CameraMove();
    }

    private void PlayerMove()
    { 
        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity = Vector2.up * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = Vector2.left * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = Vector2.right * movementSpeed * Time.deltaTime;
        }
    }

    private void CameraMove()
    {
        playerLocation = transform.position;
        Camera.main.transform.position = new Vector3(playerLocation.x, playerLocation.y, Camera.main.transform.position.z);
    }
}
