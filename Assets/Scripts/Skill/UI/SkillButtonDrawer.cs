using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButtonDrawer : MonoBehaviour
{
    [SerializeField] Transform[] m_gridRoot;
    [SerializeField] SkillButton m_skillBtnPrefab;
    private Dictionary<SkillName, int> m_skillCollecteds;
    public SkillManager skillManager;
    public void DrawSkillButton()
    {
        Singleton<PlayerManager>.Instance.CheckPlayer(out int? state, out PlayerController player);
        skillManager = player.GetComponentInChildren<SkillManager>();
        m_skillCollecteds = skillManager.SkillCollecteds;
        if (m_skillCollecteds == null || m_skillCollecteds.Count <= 0) return;
        int index = -1;

        foreach (var skillCollected in m_skillCollecteds)
        {
            index++;
            if(index<=5)
            {
                
                Helper.ClearChilds(m_gridRoot[index]);
                var skillButtonClone = Instantiate(m_skillBtnPrefab);
                Helper.AssignToRoot(m_gridRoot[index], skillButtonClone.transform,
                    Vector3.zero, index == 5 ? Vector3.one * 1.2f : (index == 0 ? 0.9f * Vector3.one : 1f * Vector3.one));
                skillButtonClone.Initialize(skillCollected.Key);
                skillButtonClone.skillButtonType = skillButtonClone.m_skillButtonTypes[index];
                
                if (skillButtonClone.skillButtonType == SkillButtonTypes.Jump)
                {
                    skillButtonClone.m_skillIcon.transform.localScale = Vector3.one * 1.2f;
                    skillButtonClone.levelSkill = 1;
                }
                if (skillButtonClone.skillButtonType == SkillButtonTypes.Teleport)
                {
                    skillButtonClone.m_skillIcon.transform.localScale = Vector3.one * 1.5f;
                    skillButtonClone.levelSkill = 1;
                }
                if (skillButtonClone.skillButtonType == SkillButtonTypes.NormalAttack)
                {
                    skillButtonClone.m_skillIcon.transform.localScale = Vector3.one * 1.37f;
                    skillButtonClone.levelSkill = 1;
                }
            }
           
        }

    }
}
