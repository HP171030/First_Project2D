using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CoolTimeManager : Singleton<CoolTimeManager>   
{
    
    [SerializeField] Image skill1Cool;
    [SerializeField] Image skill2Cool;

    [Serializable]
    public class SkillCooldownInfo
    {
        public string skillName;
        public float cooldownTime;
        public float remainingTime;
        
    }

    [SerializeField] List<SkillCooldownInfo> skillCooldowns;                // 1.��ų�̸����� ����Ʈ ����

    void Start()
    {
        skill1Cool = Manager.UICanvas.Skill1Cool;
        skill2Cool = Manager.UICanvas.Skill2Cool;
        StartCoroutine(UpdateSkillCooldowns());
    }

    IEnumerator UpdateSkillCooldowns()
    {
        while ( true )
        {
            foreach ( var skillCooldown in skillCooldowns )
            {
                if ( skillCooldown.remainingTime > 0 )
                {
                    skillCooldown.remainingTime -= Time.deltaTime;
                }
                if ( skillCooldown.skillName == "FireBall" )
                {
                    UpdateCoolTimeImage(skillCooldown.remainingTime / skillCooldown.cooldownTime, skill1Cool);
                }
                else if ( skillCooldown.skillName == "Shark" )
                {
                    UpdateCoolTimeImage(skillCooldown.remainingTime / skillCooldown.cooldownTime, skill2Cool);
                }
            }
            yield return null;
        }
    }
    void UpdateCoolTimeImage( float fillAmount, Image coolImage )
    {
        coolImage.fillAmount = fillAmount;
    }
    public void UseSkill( string skillName )                                        //2.��ų ���(�̸�)
    {
        SkillCooldownInfo skillCooldown = GetSkillCooldown(skillName);                  // ->3
        if ( skillCooldown != null && skillCooldown.remainingTime <= 0 )
        {
            skillCooldown.remainingTime = skillCooldown.cooldownTime;                       //4.���Ϲ��� �ʵ尪 ���
            StartCoroutine(StartSkillCooldown(skillCooldown));                              //5.�ڷ�ƾ������
            Debug.Log("Using skill: " + skillName);
        }
        else
        {
            Debug.Log(skillName);
            Debug.Log(skillCooldown.remainingTime);
            Debug.Log($"Skill {skillName} is on {skillCooldown.remainingTime} cooldown. ");
        }
    }

    SkillCooldownInfo GetSkillCooldown( string skillName )                     
    {
        foreach ( var skillCooldown in skillCooldowns )
        {
            if ( skillCooldown.skillName == skillName ) //3.���� �����س��� ��ų�̸��̶� ������
            {
                return skillCooldown;                       // 4.����Ʈ�� �ִ� �� ����
            }
        }
        return null;
    }
    public bool IsSkillCool( string skillName )
    {
        
        foreach ( var skillCooldown in skillCooldowns )
        {
            if ( skillCooldown.skillName == skillName )
            {
                
                return skillCooldown.remainingTime <= 0;
            }
        }
       
        return false;
    }
    IEnumerator StartSkillCooldown( SkillCooldownInfo skillCooldown )
    {
        yield return new WaitForSeconds(skillCooldown.cooldownTime);
        skillCooldown.remainingTime = 0;
    }
}
