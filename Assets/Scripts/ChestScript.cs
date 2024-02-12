using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
    [SerializeField] private GameObject interact;
    private float playerWeaponHad;
    private int isOpened = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        interact = gameObject.transform.Find("Interact").gameObject;
        spriteRenderer.sprite = isOpened == 0 ? sprites[0] : sprites[1];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && isOpened == 0)
        {
            interact.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E))
            {
                spriteRenderer.sprite = sprites[1];
                isOpened = 1;
                GameManager.Instance.WeaponObtaining();
            }
        }
    }
}
