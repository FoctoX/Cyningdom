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
        fade = transform.Find("Canvas").transform.Find("Player Died").GetComponent<Animator>();
        diedPanel = fade.transform.Find("Panel").gameObject;
        obtainPanel = transform.Find("Canvas").transform.Find("Item Obtained").gameObject;
        weaponName = obtainPanel.transform.Find("Text").transform.Find("Weapon Name").GetComponent<Text>();
        VFX = obtainPanel.transform.Find("Image").transform.Find("VFX").GetComponent<Image>();
        weaponImage = obtainPanel.transform.Find("Image").transform.Find("Weapon").GetComponent<Image>();
        playerMoveScript.experienceLevel = PlayerPrefs.GetInt("exp");
        
    }

    private void Start()
    {
        IconChange();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Time.timeScale = 0;
            transform.Find("Canvas").transform.Find("Cheat Panel").gameObject.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        PlayerConditionUI();
        BossUI();
        BossSpawning();
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

    public void PlayerConditionUI()
    {
        healthbar.fillAmount = playerMoveScript.health / playerMoveScript.maxHealth;
        energybar.fillAmount = playerMoveScript.energy / playerMoveScript.maxEnergy;
        expbar.fillAmount = playerMoveScript.exp / playerMoveScript.maxExp;

        if (playerMoveScript.health <= 0f)
        {
            StartCoroutine("DiedAnimation");
        }
        if (playerMoveScript.exp >= playerMoveScript.maxExp)
        {
            if (playerMoveScript.experienceLevel < 5)
            {
                playerMoveScript.experienceLevel += 1;
            }
            playerMoveScript.LevelUp();
            playerMoveScript.exp = 0f;
            playerMoveScript.health = playerMoveScript.maxHealth;
            playerMoveScript.energy = playerMoveScript.maxEnergy;
        }
    }

    public void PlayerConditionRescan()
    {

    }

    private IEnumerator DiedAnimation()
    {
        transform.Find("Canvas").transform.Find("Player Died").gameObject.SetActive(true);
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
                playerMoveScript.hadWeapon = 1;
                playerMoveScript.currentWeapon = 1;
                break;
            case 1:
                weaponName.text = "Sword";
                weaponImage.sprite = icon[1];
                VFX.sprite = null;
                VFX.color = new Color(0, 0, 0, 0);
                playerMoveScript.hadWeapon = 2;
                playerMoveScript.currentWeapon = 2;
                break;
            case 2:
                weaponName.text = "The Greatsword";
                weaponImage.sprite = icon[2];
                VFX.sprite = icon[3];
                VFX.color = new Color(1, 1, 1, 1);
                playerMoveScript.hadWeapon = 3;
                playerMoveScript.currentWeapon = 3;
                break;
        }
        obtainPanel.SetActive(true);
        IconChange();
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
        Time.timeScale = 1;
        InputField input;
        input = transform.Find("Canvas").transform.Find("Cheat Panel").transform.Find("Input").GetComponent<InputField>();
        if (input.text.ToLower() == "hesoyam")
        {
            playerMoveScript.hadWeapon = 3;
            playerMoveScript.currentWeapon = 3;
            weaponName.text = "The Greatsword";
            weaponImage.sprite = icon[2];
            VFX.sprite = icon[3];
            VFX.color = new Color(1, 1, 1, 1);
            playerMoveScript.hadWeapon = 3;
            playerMoveScript.currentWeapon = 3;
            obtainPanel.SetActive(true);
        }
        transform.Find("Canvas").transform.Find("Cheat Panel").gameObject.SetActive(false);
    }

    public void BossUI()
    {
        if (GameObject.FindGameObjectWithTag("Boss").GetComponent<EnemyScript>().enabled)
        {
            GameObject bossHealth = transform.Find("Canvas").transform.Find("Boss Health").gameObject;
            transform.Find("Canvas").transform.Find("Player Conditions").gameObject.transform.localPosition = new Vector3(0, -450, 0);
            bossHealth.SetActive(true);
            EnemyScript enemyScriptBoss = GameObject.FindGameObjectWithTag("Boss").GetComponent<EnemyScript>();
            bossHealth.transform.Find("Name").GetComponent<Text>().text = enemyScriptBoss.bossName;
            bossHealth.transform.Find("Name Behind").GetComponent<Text>().text = enemyScriptBoss.bossName;
            bossHealth.transform.Find("Health").transform.Find("Current Health").GetComponent<Image>().fillAmount = enemyScriptBoss.health / enemyScriptBoss.maxHealth;
        }
        else
        {
            transform.Find("Canvas").transform.Find("Boss Health").gameObject.SetActive(false);
            transform.Find("Canvas").transform.Find("Player Conditions").gameObject.transform.localPosition = Vector3.zero;
        }
    }

    private void BossSpawning()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (allEnemies.Length <= 0)
        {
            GameObject.FindGameObjectWithTag("Boss").GetComponent<EnemyScript>().enabled = true;
            GameObject.FindGameObjectWithTag("Boss").GetComponent<SpriteRenderer>().enabled = true;
            GameObject.FindGameObjectWithTag("Boss").transform.Find("Boss Indicator").gameObject.SetActive(true);
        }
    }
}
