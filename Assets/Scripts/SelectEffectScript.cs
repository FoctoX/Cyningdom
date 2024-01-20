using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Events; 
using UnityEngine.EventSystems; 

public class ColourChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private void Awake()
    {
        GetComponent<Text>().fontSize = 40;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Text>().fontSize = 45;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Text>().fontSize = 40;
    }
}
