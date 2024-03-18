using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Mimic : Monster
{
    protected override IEnumerator AttackPlayer()
    {
     if ( !atkDelayOn )
        {
           
        Vector2 prePos = transform.position;
    Vector2 targetPos = ( Vector2 )transform.position + atkDir * 2f;
    float t = 0;
    float duration = 0.5f;

    atkDelayOn = true;
            Manager.Sound.PlaySFX(monsterData.soundAttack);
            animator.Play("AttackMimic");
            
            while (t< 1f )
            {
                transform.position = Vector2.Lerp(prePos, targetPos, t);
                t += Time.deltaTime / duration; 
                yield return null;

            }
Collider2D player = Physics2D.OverlapCircle(transform.position, monsterData.attackRange, playerLayer);
if ( player != null )
{
    SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
    Manager.Game.HpEvent -= monsterData.atk;
                Manager.Game.ShakeCam();
                spriteRenderer.material.color = Color.red;
    spriteRenderer.material.DOColor(Color.white, 1f);
    Manager.Sound.PlaySFX(monsterData.soundPlayerDamaged);
}

ChangeState(MonsterState.Idle);
            animator.SetBool("Move", false);    
yield return new WaitForSeconds(monsterData.atkDelay);
atkDelayOn = false;
ChangeState(MonsterState.Chase);
        }
    }

}
