using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int health = 100;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material normal;
    [SerializeField] private Material flash;
    private Coroutine flashCoroutine;
    public bool canHitted;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnemyCondition()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
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

        yield return new WaitForSeconds(0.125f);

        spriteRenderer.material = normal;

        canHitted = true;
        flashCoroutine = null;
    }
}
