using System.Collections;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float health;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Material normal;
    [SerializeField] private Material flash;
    private Coroutine flashCoroutine;
    public bool canHitted;
    [SerializeField] private GameObject textDemage;
    [SerializeField] private float textDemageFix;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        GameObject textDemageInstance = Instantiate(textDemage, transform.position, Quaternion.identity);
        textDemageInstance.transform.parent = transform;
        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);
        Vector3 randomOffset = new Vector3(randomX, randomY + textDemageFix, 0f);
        textDemageInstance.transform.localPosition = randomOffset;

        yield return new WaitForSeconds(0.125f);

        spriteRenderer.material = normal;

        canHitted = true;
        flashCoroutine = null;
    }
}
