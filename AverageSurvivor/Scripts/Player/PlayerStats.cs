using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    CharacterScriptableObject characterData;

    float currentHealth;
    float currentRecovery;
    float currentMoveSpeed;
    float currentMight;
    float currentProjectileSpeed;
    float currentMagnet;


    #region PLAYER STAT PROPERTIES
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if(currentHealth != value)
            {
                currentHealth = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.currentHealthDisplay.text = "Health: " + currentHealth;
                }
            }
        }
    }

    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set
        {
            if(currentRecovery != value)
            {
                currentRecovery = value;
            }
            if(GameManager.instance != null)
            {
                GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + currentRecovery;
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            if(currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
            }
            if(GameManager.instance != null)
            {
                GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + currentMoveSpeed;
            }
        }
    }

    public float CurrentMight
    {
        get { return currentMight; }
        set
        {
            if(currentMight != value)
            {
                currentMight = value;
            }
            if(GameManager.instance != null)
            {
                GameManager.instance.currentMightDisplay.text = "Might: " + currentMight;
            }
        }
    }

    public float CurrentProjectileSpeed
    {
        get { return currentProjectileSpeed; }
        set
        {
            if(currentProjectileSpeed != value)
            {
                currentProjectileSpeed = value;
            }
            if(GameManager.instance != null)
            {
                GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + currentProjectileSpeed;
            }
        }
    }

    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            if(currentMagnet != value)
            {
                currentMagnet = value;
            }
            if(GameManager.instance != null)
            {
                GameManager.instance.currentMagnetDisplay.text = "Magnet: " + currentMagnet;
            }
        }
    }
    #endregion

    [Header("Experience/Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }

    [Header("I-frames")]
    public float iFrameDuration;
    private float iFrameTimer;
    private bool isInvincible;

    public List<LevelRange> levelRanges;

    PlayerCollector collector;
    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

    public ParticleSystem hitEffect;

    [Header("UI")]
    public Image HealthBar;
    public Image ExperienceBar;

    public GameObject secondWeaponTest;
    public GameObject firstPassiveItemTest, secondPassiveItemTest;
    
    PlayerAnimator playerAnimator;

    private void Awake()
    {
        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<InventoryManager>();
        collector = GetComponentInChildren<PlayerCollector>();

        CurrentHealth = characterData.MaxHealth;
        CurrentRecovery = characterData.Recovery;
        CurrentMoveSpeed = characterData.MoveSpeed;
        CurrentMight = characterData.Might;
        CurrentProjectileSpeed = characterData.ProjectileSpeed;
        CurrentMagnet = characterData.Magnet;

        collector.SetRadius(CurrentMagnet);

        SpawnWeapon(characterData.StartingWeapon);

        playerAnimator = GetComponent<PlayerAnimator>();
        playerAnimator.SetAnimatorController(characterData.animatorController);
    }

    private void Start()
    {
        experienceCap = levelRanges[0].experienceCapIncrease;

        GameManager.instance.currentHealthDisplay.text = "Health: " + CurrentHealth;
        GameManager.instance.currentRecoveryDisplay.text = "Recovery: " + CurrentRecovery;
        GameManager.instance.currentMoveSpeedDisplay.text = "Move Speed: " + CurrentMoveSpeed;
        GameManager.instance.currentMightDisplay.text = "Might: " + CurrentMight;
        GameManager.instance.currentProjectileSpeedDisplay.text = "Projectile Speed: " + CurrentProjectileSpeed;
        GameManager.instance.currentMagnetDisplay.text = "Magnet: " + CurrentMagnet;

        GameManager.instance.AssignPlayerCharacter(characterData);

        updateHealthBar();
        updateExperienceBar();
    }

    private void Update()
    {
        if (iFrameTimer > 0)
        {
            iFrameTimer -= Time.deltaTime;
        }
        else
        {
            isInvincible = false;
        }
        updateHealthBar();
        updateExperienceBar();
        Recover();
    }

    public void IncreaseExperience(int amount)
    {
        experience += amount;

        LevelUpChecker();

        updateExperienceBar();
    }

    void LevelUpChecker()
    {
        if (experience >= experienceCap)
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges)
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;

            GameManager.instance.StartLevelUp();
        }
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible)
        {
            CurrentHealth -= dmg;

            if(hitEffect)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            iFrameTimer = iFrameDuration;
            isInvincible = true;

            if (CurrentHealth <= 0)
            {
                Kill();
            }

            updateHealthBar();
        }
    }

    void updateHealthBar()
    {
        HealthBar.fillAmount = CurrentHealth / characterData.MaxHealth;
    }

    void updateExperienceBar()
    {
        ExperienceBar.fillAmount = (float)experience / experienceCap;
    }

    public void Kill()
    {
        if(!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReached(level);
            GameManager.instance.AssignChosenWeapons(inventory.weaponUISlots, inventory.passiveItemUISlots);
            GameManager.instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        if(CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += amount;
            if (CurrentHealth > characterData.MaxHealth)
            {
                CurrentHealth = characterData.MaxHealth;
            }
        }

        updateHealthBar();
    }

    void Recover()
    {
        if (CurrentHealth < characterData.MaxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
            if (CurrentHealth > characterData.MaxHealth)
            {
                CurrentHealth = characterData.MaxHealth;
            }
        }

        updateHealthBar();
    }

    public void SpawnWeapon(GameObject weapon)
    {
        if(weaponIndex >= inventory.weaponSlots.Count - 1)
        {
            Debug.LogWarning("Inventory full.");
            return;
        }

        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform);
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>());

        weaponIndex++;
    }

    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveItemIndex >= inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogWarning("Inventory full.");
            return;
        }
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform);
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>());

        passiveItemIndex++;
    }
}
