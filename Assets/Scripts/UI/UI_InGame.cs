using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PlayerStats playerStats;
    
    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image flaskImage;

    [SerializeField] private TextMeshProUGUI currentSouls;
    
    private SkillManager skills;

    void Start()
    {
        if (playerStats != null)
            playerStats.onHealthChanged += UpdateHealthUI;
        
        skills = SkillManager.instance;
        dashImage.transform.parent.gameObject.SetActive(false);
        parryImage.transform.parent.gameObject.SetActive(false);
        crystalImage.transform.parent.gameObject.SetActive(false);
        swordImage.transform.parent.gameObject.SetActive(false);
        blackholeImage.transform.parent.gameObject.SetActive(false);
        flaskImage.transform.parent.gameObject.SetActive(false);
    }

    #region Unlock Skills
    public void UnlockDash()
    {
        dashImage.transform.parent.gameObject.SetActive(true);
    }

    public void UnlockParry()
    {
        parryImage.transform.parent.gameObject.SetActive(true);
    }

    public void UnlockCrystal()
    {
        crystalImage.transform.parent.gameObject.SetActive(true);
    }   

    public void UnlockSword()
    {
        swordImage.transform.parent.gameObject.SetActive(true);
    }

    public void UnlockFlask(bool _unlock)
    {
        flaskImage.transform.parent.gameObject.SetActive(_unlock);
    }

    public void UnlockBlackhole()
    {
        blackholeImage.transform.parent.gameObject.SetActive(true);
    }
    #endregion

    void Update()
    {
        currentSouls.text = PlayerManager.instance.GetCurrency().ToString("#,#");

        if (Input.GetKeyDown(KeyCode.LeftShift))
            SetCooldownOf(dashImage);

        if (Input.GetKeyDown(KeyCode.Q))
            SetCooldownOf(parryImage);

        if (Input.GetKeyDown(KeyCode.R))
            SetCooldownOf(blackholeImage);

        if(Input.GetKeyDown(KeyCode.F))
            SetCooldownOf(crystalImage);

        if(Input.GetKeyDown(KeyCode.Mouse1))
            SetCooldownOf(swordImage);
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
            SetCooldownOf(flaskImage);

        if (skills.dash.dashUnlocked)
            CheckCooldownOf(dashImage, skills.dash.cooldown);

        CheckCooldownOf(parryImage, skills.parry.cooldown);
        CheckCooldownOf(blackholeImage, skills.blackhole.cooldown);
        CheckCooldownOf(swordImage, skills.sword.cooldown);
        CheckCooldownOf(crystalImage, skills.crystal.cooldown);
        CheckCooldownOf(flaskImage, Inventory.instance.FlaskCooldown());
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.currentHealth;
    }

    private void SetCooldownOf(Image _image)
    {
        if (_image.fillAmount <= 0)
        {
            _image.fillAmount = 1;
        }
    }

    private void CheckCooldownOf(Image _image, float _cooldown)
    {
        if (_image.fillAmount > 0)
            _image.fillAmount -= 1 / _cooldown * Time.deltaTime;
    }
}