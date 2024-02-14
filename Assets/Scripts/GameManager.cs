using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = UnityEngine.Color;

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

    private GameObject diedPanel;

    private Animator fade;
    private Animator wasted;

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
        fade = transform.Find("Canvas").transform.Find("Player Died").GetComponent<Animator>();
        diedPanel = fade.transform.Find("Panel").gameObject;
        obtainPanel = transform.Find("Canvas").transform.Find("Item Obtained").gameObject;
        weaponName = obtainPanel.transform.Find("Text").transform.Find("Weapon Name").GetComponent<Text>();
        VFX = obtainPanel.transform.Find("Image").transform.Find("VFX").GetComponent<Image>();
        weaponImage = obtainPanel.transform.Find("Image").transform.Find("Weapon").GetComponent<Image>();
        PlayerCondition();
        IconChange();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            transform.Find("Canvas").transform.Find("Cheat Panel").gameObject.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        PlayerCondition();
    }

    public void IconChange()
    {
        switch (playerMoveScript.currentWeapon)
        {
            case 0:
                weaponIcon.sprite = null;
                weaponIcon.color = new Color(0,0,0,0);
                break;
            case 1:
                weaponIcon.sprite = icon[0];
                weaponIcon.color = new Color(255, 255, 255, 255);
                break;
            case 2:
                weaponIcon.sprite = icon[1];
                weaponIcon.color = new Color(255, 255, 255, 255);
                break;
            case 3:
                weaponIcon.sprite = icon[2];
                weaponIcon.color = new Color(255, 255, 255, 255);
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
            StartCoroutine("DiedAnimation");
        }
    }

    private IEnumerator DiedAnimation()
    {
        fade.SetTrigger("died");

        yield return new WaitForSecondsRealtime(3.5f);

        ReloadScene();

        yield return null;
    }

    public void WeaponObtaining()
    {
        Time.timeScale = 0f;
        switch (playerMoveScript.hadWeapon)
        {
            case 0:
                weaponName.text = "Knife";
                weaponImage.sprite = icon[0];
                VFX.sprite = null;
                VFX.color = new Color(0,0,0,0);
                playerMoveScript.hadWeapon += 1;
                break;
            case 1:
                weaponName.text = "Sword";
                weaponImage.sprite = icon[1];
                VFX.sprite = null;
                VFX.color = new Color(0, 0, 0, 0);
                playerMoveScript.hadWeapon += 1;
                break;
            case 2:
                weaponName.text = "The Greatsword";
                weaponImage.sprite = icon[2];
                VFX.sprite = icon[3];
                VFX.color = new Color(1, 1, 1, 1);
                playerMoveScript.hadWeapon += 1;
                break;
        }
        obtainPanel.SetActive(true);
    }

    public void NormalTimeScale()
    {
        Time.timeScale = 1;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CheatPanelSubmit()
    {
        InputField input;
        input = transform.Find("Canvas").transform.Find("Cheat Panel").transform.Find("Input").GetComponent<InputField>();
        if (input.text.ToLower() == "hesoyam")
        {
            playerMoveScript.hadWeapon = 3;
            playerMoveScript.currentWeapon = 3;
            WeaponObtaining();
            IconChange();
        }
        transform.Find("Canvas").transform.Find("Cheat Panel").gameObject.SetActive(false);
    }
}
