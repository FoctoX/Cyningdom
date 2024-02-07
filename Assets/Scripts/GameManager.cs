using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Image weaponIcon;
    [SerializeField] private Sprite[] icon;

    private void Awake()
    {
        Instance = this;
        IconChange();
    }

    public void IconChange()
    {
        switch (PlayerMoveScript.Instance.currentWeapon)
        {
            case 0:
                weaponIcon.sprite = icon[0];
                break;
            case 1:
                weaponIcon.sprite = icon[1];
                break;
            case 2:
                weaponIcon.sprite = icon[2];
                break;
        }
    }
}
