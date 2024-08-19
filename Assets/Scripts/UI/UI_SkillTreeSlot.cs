using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SkillTreeSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISaveManager
{
    protected UI ui;
    private Image skillImage;

    [SerializeField] private int skillPrice;
    [SerializeField] private string skillName;
    [TextArea]
    [SerializeField] private string skillDescription;
    [SerializeField] private Color lockedSkillColor;


    public bool unlocked;
    public bool startActive;

    [SerializeField] private UI_SkillTreeSlot[] shouldBeUnlocked;
    [SerializeField] private UI_SkillTreeSlot[] shouldBeLocked;

    [SerializeField] private UI_SkillTreeSlot[] unlocks;
    [SerializeField] private UI_SkillTreeSlot[] locks;


    public event Action OnFullyFilled;

    private void OnValidate()
    {
        gameObject.name = "SkillTreeSlot_UI - " + skillName;
        transform.parent.gameObject.name = skillName;
        skillImage = transform.parent.gameObject.GetComponent<Image>();
    }

    private void Awake()
    {
        
    }

    private void Start()
    {
        if (!startActive)
            gameObject.SetActive(false);
        
        if (startActive)
            skillImage.color = lockedSkillColor;

        ui = GetComponentInParent<UI>();

        if(unlocked)
        {
            bool noCost = true;
            UnlockSkillSlot(noCost);
        }
    }

    public void UnlockSkillSlot(bool noCost = false)
    {
        if(!noCost)
        {
            if (!PlayerManager.instance.HaveEnoughMoney(skillPrice))
                return;
        }
            
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (!shouldBeUnlocked[i].unlocked)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unlocked)
            {
                Debug.Log("Cannot unlock skill");
                return;
            }
        }

        unlocked = true;
        OnFullyFilled?.Invoke();

        skillImage.color = Color.white;
        GetComponentInChildren<UI_SkillTreeSlot>().gameObject.SetActive(false);

        for (int i = 0; i < unlocks.Length; i++)
        {
            unlocks[i].gameObject.SetActive(true);
            unlocks[i].startActive = true;
            unlocks[i].skillImage.color = lockedSkillColor;
        }

         for (int i = 0; i < locks.Length; i++)
        {
            locks[i].gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ui.skillTooltip.ShowToolTip(skillName, skillDescription, skillPrice);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.skillTooltip.HideToolTip();
    }

    public void LoadData(GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName, out bool value))
        {
            unlocked = value;
        }
    }

    public void SaveData(ref GameData _data)
    {
        if(_data.skillTree.TryGetValue(skillName, out bool value))
        {
            _data.skillTree.Remove(skillName);
            _data.skillTree.Add(skillName, unlocked);
        }
        else
        {
            _data.skillTree.Add(skillName, unlocked);
        }
    }
}
