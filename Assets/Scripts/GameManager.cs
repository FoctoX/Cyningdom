using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerMoveScript playerMoveScript;

    [SerializeField] private Image weaponIcon;
    [SerializeField] private Sprite[] icon;

    private GameObject obtainPanel;
    private Text weaponName;
    private Image VFX;
    private Image weaponImage;
    private float weaponHad;

    [SerializeField] private Image parentHealthbar, parentEnergybar, parentExpbar;
    [SerializeField] private Image healthbar, energybar, expbar;
    private float maxHealth, maxEnergy, maxExp;

    [SerializeField] private GameObject diedPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        playerMoveScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoveScript>();
        maxHealth = playerMoveScript.health;
        maxEnergy = playerMoveScript.energy;
        maxExp = playerMoveScript.exp;
        obtainPanel = transform.Find("Canvas").transform.Find("Item Obtained").gameObject;
        weaponName = obtainPanel.transform.Find("Text").transform.Find("Weapon Name").GetComponent<Text>();
        VFX = obtainPanel.transform.Find("Image").transform.Find("VFX").GetComponent<Image>();
        weaponImage = obtainPanel.transform.Find("Image").transform.Find("Weapon").GetComponent<Image>();
        PlayerCondition();
        IconChange();
    }

    private void FixedUpdate()
    {
        PlayerCondition();
        Debug.Log(weaponHad);
    }

    public void IconChange()
    {
        switch (playerMoveScript.currentWeapon)
        {
            case 0:
                weaponIcon.sprite = null;
                break;
            case 1:
                weaponIcon.sprite = icon[0];
                break;
            case 2:
                weaponIcon.sprite = icon[1];
                break;
            case 3:
                weaponIcon.sprite = icon[2];
                break;
        }
    }

    public void PlayerCondition()
    {
        healthbar.fillAmount = playerMoveScript.health / maxHealth;
        energybar.fillAmount = playerMoveScript.energy / maxEnergy;
        expbar.fillAmount = playerMoveScript.exp / maxExp;

        playerMoveScript.energy += 0.1f;
        playerMoveScript.energy = Mathf.Clamp(playerMoveScript.energy, 0f, maxEnergy);
        playerMoveScript.health = Mathf.Clamp(playerMoveScript.health, 0f, maxHealth);

        if (playerMoveScript.health <= 0f)
        {
            diedPanel.SetActive(true);
        }
    }

    public void WeaponObtaining()
    {
        switch (weaponHad)
        {
            case 0:
                weaponName.text = "Knife";
                weaponImage.sprite = icon[0];
                VFX.sprite = null;
                playerMoveScript.hadWeapon += 1;
                break;
            case 1:
                weaponName.text = "Sword";
                weaponImage.sprite = icon[1];
                VFX.sprite = null;
                playerMoveScript.hadWeapon += 1;
                break;
            case 2:
                weaponName.text = "The Greatsword";
                weaponImage.sprite = icon[2];
                VFX.sprite = icon[3];
                playerMoveScript.hadWeapon += 1;
                break;
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
