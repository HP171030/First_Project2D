using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeEffect : MonoBehaviour
{
    public Animator animator;
    private void Start()
    {
        ItemHealEffect.hpConsumeEvent += ConsumeEffecter;
    }
    public void ConsumeEffecter()
    {
        animator.SetTrigger("Healing");
        
    }
}
