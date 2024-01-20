using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCursorScript : MonoBehaviour
{
    Vector3 mousePosition;
    public GameObject light;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Input.mousePosition;
        mousePosition.z = 10.0f;
        light.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
