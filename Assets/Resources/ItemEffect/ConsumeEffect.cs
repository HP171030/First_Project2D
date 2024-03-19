using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumeEffect : MonoBehaviour
{
    public Animator animator;
    public AudioClip consumeSound;
    private void OnEnable()
    {
        ItemHealEffect.hpConsumeEvent += ConsumeEffecter;
        Manager.Sound.PlaySFX(consumeSound);
    }
    private void OnDisable()
    {
        ItemHealEffect.hpConsumeEvent -= ConsumeEffecter;
    }
    public void ConsumeEffecter()
    {
        animator.SetTrigger("Healing");
        
    }
}
