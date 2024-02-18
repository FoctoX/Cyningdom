using UnityEngine;

public class ChestScript : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    [SerializeField] Sprite[] sprites;
    [SerializeField] GameObject obtainingPanel;
    private PlayerMoveScript playerMoveScript;
    private int isOpened = 0;
    [SerializeField] private bool forced = true;
    [SerializeField] private float forcePower;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer.sprite = isOpened == 0 ? sprites[0] : sprites[1];
        playerMoveScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (forced)
        {
            rb.AddForce(transform.up * forcePower, ForceMode2D.Impulse);
        }
    }

    public void Open()
    {
        if (isOpened == 0)
        {
            spriteRenderer.sprite = sprites[1];
            isOpened = 1;
            GameManager.Instance.WeaponObtaining();
        }
    }
}
