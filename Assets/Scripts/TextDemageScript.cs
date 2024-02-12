using UnityEngine;

public class TextDemageScript : MonoBehaviour
{
    private int totalDemage;
    private TextMesh textMesh;

    private void Awake()
    {
        textMesh = GetComponent<TextMesh>();
    }

    public void DestroyText()
    {
        Destroy(gameObject);
    }
}
