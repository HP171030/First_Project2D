using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Skill : MonoBehaviour

{
    [SerializeField] protected Animator animator;
    [SerializeField] protected Image skillUI;


    public abstract void SkillStart();
   

}

