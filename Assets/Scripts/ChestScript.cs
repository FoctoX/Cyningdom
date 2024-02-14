using UnityEngine;

public class ChestScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] sprites;
    [SerializeField] GameObject obtainingPanel;
    private PlayerMoveScript playerMoveScript;
    private int isOpened = 0;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = isOpened == 0 ? sprites[0] : sprites[1];
        playerMoveScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveScript>();
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
        if (collision.tag == "Player" && isOpened == 0 && playerMoveScript.hadWeapon < 3)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                spriteRenderer.sprite = sprites[1];
                isOpened = 1;
                GameManager.Instance.WeaponObtaining();
            }
        }
    }
}
