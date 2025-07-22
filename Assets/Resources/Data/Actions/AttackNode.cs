using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[NodeRunnerFor(typeof(ActionNode),nameof(AttackNode))]
[NodeName(nameof(AttackNode))]
public class AttackNode : ActionNodeRunner
{
        
    public AttackNode( Node node, Monster monster ) : base(node, monster)
    {

    }

    public override IEnumerator<NodeState> Execute()
    {
        Monster.MoveOn = false;

        Vector2 prePos = Monster.transform.position;
        Vector2 targetPos = prePos + Monster.atkDir * 2f;

        //TODO : 에디터에서 공격속도도 가져올 수 있도록 변경할것
        float duration = 0.5f;
        float elapsed = 0f;

        Manager.Sound.PlaySFX(Monster.monsterData.soundAttack);
        Monster.animator.Play("AttackMonster");
        Debug.Log("Attack");
        // 공격 애니메이션 이동
        while ( elapsed < duration )
        {
            float t = elapsed / duration;
            Monster.transform.position = Vector2.Lerp(prePos, targetPos, t);
            elapsed += Time.deltaTime;
            yield return NodeState.Running;
        }

        Monster.transform.position = targetPos; 

        // 플레이어 히트 판정
        Collider2D player = Physics2D.OverlapCircle(Monster.transform.position, Monster.monsterData.attackRange, Monster.playerLayer);
        if ( player != null )
        {
            SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
            Manager.Game.HpEvent -= Monster.monsterData.atk;
            Manager.Game.ShakeCam();
            spriteRenderer.material.color = Color.red;
            spriteRenderer.material.DOColor(Color.white, 1f);
            Manager.Sound.PlaySFX(Monster.monsterData.soundPlayerDamaged);
        }

        Monster.ChangeState(Monster.MonsterState.Idle);
        Monster.animator.SetBool("Move", false);

        // 공격 후 딜레이
        float delay = Monster.monsterData.atkDelay;
        while ( delay > 0f )
        {
            delay -= Time.deltaTime;
            yield return NodeState.Running;
        }


        yield return NodeState.True; 
    }
}
