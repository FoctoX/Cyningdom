using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
