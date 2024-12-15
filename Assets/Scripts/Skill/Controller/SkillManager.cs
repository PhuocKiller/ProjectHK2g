using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager : NetworkBehaviour
{
    [SerializeField] private SkillController[] m_skillControllers;
    private Dictionary<SkillName, int> m_skillCollecteds; //int là số lượng skills

    public Dictionary<SkillName, int> SkillCollecteds { get => m_skillCollecteds; set { } }
   [SerializeField] private SkillButtonDrawer m_skillBtnDrawer;
   bool isAwake;

    public override void Spawned()
    {
        base.Spawned();
        if(HasStateAuthority)
        {
            Initialize();
            DrawSkill();
        }
    }
    public void Initialize()
    {
        m_skillCollecteds = new Dictionary<SkillName, int>();
        if (m_skillControllers == null || m_skillControllers.Length <= 0) return;
        for (int i = 0; i < m_skillControllers.Length; i++)
        {
            var skillController = m_skillControllers[i];
            if (skillController == null) continue;
            skillController.LoadStat();
            skillController.OnStopWithType.AddListener(RemoveSkill);
            m_skillCollecteds.Add(skillController.skillName, 1);
        }
    }
    public void DrawSkill()
    {
        m_skillBtnDrawer = GameObject.Find("GridSkill").GetComponent<SkillButtonDrawer>();
        m_skillBtnDrawer?.DrawSkillButton();
    }
    public SkillController GetSkillController(SkillName type)
    {
        var findeds= m_skillControllers.Where(s=>s.skillName == type).ToArray();
        if (findeds == null || findeds.Length <= 0) return null;
        return findeds[0];
    }
    public int GetSkillAmount(SkillName type)
    {
        if (!IsSkillExist(type)) return 0;
        return m_skillCollecteds[type];
    }
    public void AddSkill(SkillName type, int amount=1)
    {
        if (IsSkillExist(type))
        {
            var currentAmount = m_skillCollecteds[type];
            currentAmount += amount;
            m_skillCollecteds[type]= currentAmount;
        } else
        {
            m_skillCollecteds.Add(type, amount);
        }
    }
    public void RemoveSkill(SkillName type, int amount = 1)
    {
        if (!IsSkillExist(type)) return;
        var currentAmount= m_skillCollecteds[type];
        currentAmount -= amount;
        m_skillCollecteds[type] = currentAmount;
        if (currentAmount > 0) return;
        m_skillCollecteds.Remove(type);
    }
    public bool IsSkillExist(SkillName type)
    {
        return m_skillCollecteds.ContainsKey(type);
    }
    public void StopSkill(SkillName type)
    {
        var skillController=GetSkillController(type);
        if (skillController == null) return;
        skillController.Stop(); 
    }
    public void StopAllSkills()
    {
        if (m_skillControllers==null ||m_skillControllers.Length<=0) return;
        foreach (var skillController in m_skillControllers)
        {
            if (skillController==null) continue;
            skillController.ForceStop();
        }
    }
}
