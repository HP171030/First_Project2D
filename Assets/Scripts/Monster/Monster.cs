using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using UnityEngine;


public class Monster : MonoBehaviour, Idamagable
{
    
 protected Animator animator;
[SerializeField] protected  Collider2D [] colliders;
 protected  Vector3 playerPos;
[SerializeField] public  SpriteRenderer spriteRenderer;
 protected   Vector2 moveDir;
 protected   bool MoveOn;
 protected   bool CRSwitch;
 protected   Collider2D thisCollider;
 protected   Vector2 atkDir;
 protected   bool atkDelayOn;
    protected Collider2D player;
    [SerializeField] protected  Rigidbody2D rb;
    protected bool flipXed;
 [SerializeField]protected Animator damagedEffect;
[SerializeField] protected Transform damagedEffectPos;
    
    [SerializeField] protected LayerMask playerLayer;
   [SerializeField]protected PooledObject monsterPool;
   [SerializeField] protected MonsterData monsterData;

    float localX;
        float localY;

    protected float thisMonsterHP;

   [SerializeField] protected GameObject dropItem;
    protected virtual void Start()
    {
        ChangeState(MonsterState.Idle);
        thisMonsterHP = monsterData.hp;
        colliders = new Collider2D [20];
        animator = GetComponent<Animator>();
        thisCollider = GetComponent<Collider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        thisCollider.enabled = true;
        MoveOn = false;
        localX = transform.localScale.x;
        localY = transform.localScale.y;

    }

    public void ChasePattern()
    {
        Targeting();
        Moving();
    }

    
    public void Targeting()
    {
       
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, monsterData.range, colliders, playerLayer);
       
        
        if ( size > 0 && !CRSwitch )
        {
            for ( int i = 0; i < size; i++ )
            {
                if ( playerLayer.Contain(colliders [i].gameObject.layer) )
                {
                    playerPos = colliders [i].transform.position;
                    moveDir = ( playerPos - transform.position ).normalized;
                   
                    MoveOn = true;
                    StartCoroutine(MoveMonster());
                }
            }
        }
        else if (size <1)
        {
            ChangeState(MonsterState.Idle);
            Debug.Log("ChaIDle");
            animator.SetBool("Move", false);
        }
   
       
       
        
    }

 
    public IEnumerator MoveMonster()
    {   
        
        if ( !CRSwitch )
        {
            CRSwitch = true;
           
            animator.SetBool("Move",true);
            yield return new WaitForSeconds(monsterData.moveDelay);
            
            CRSwitch = false;
            
            

        }
    }

    public void Moving()
    {
        if ( MoveOn )
        {
            
            transform.Translate(moveDir * monsterData.speed/100);
        }
    }
    public void MoveSwitch()
    {
        animator.SetBool("Move", true);
        MoveOn = true;
    }
    public void MoveSwitchOff()
    {
        animator.SetBool("Move",false );
        MoveOn = false;
    }
    public enum MonsterState {Idle,Chase,Attack,Dead }

   public MonsterState curState;

   
    protected void Update()
    {
        
        if ( moveDir.x < 0 )
        {

            flipXed = true;
            transform.localScale = new Vector3(-localX, localY, 1);
           

        }
        else if ( moveDir.x > 0 )
        {

            flipXed = false;
            transform.localScale = new Vector3(localX,localY, 1);


        }
        switch ( curState )
        {
            case MonsterState.Idle:

                
                
                IdleStates();
                break;

            case MonsterState.Chase:
                ChasePattern();
                 player  = Physics2D.OverlapCircle(transform.position, monsterData.attackRange, playerLayer);
                if( player != null )
                {
                    atkDir =( player.transform.position - transform.position).normalized;
                    ChangeState(MonsterState.Attack);
                    MoveOn = false; 
                    break;
                }
                break;

            case MonsterState.Attack:
                
                StartCoroutine(AttackPlayer());
                break;

            case MonsterState.Dead:
                
                thisCollider.enabled = false;
                
                break;
        }
    }
    protected virtual void IdleStates()
    {
        
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, monsterData.range, colliders, playerLayer);

        if ( size > 0 )
        {
            for ( int i = 0; i < size; i++ )
            {
                if ( playerLayer.Contain(colliders [i].gameObject.layer) && !atkDelayOn )
                {

                    ChangeState(MonsterState.Chase); break;
                }
            }
        }
        else
        {
            MoveOn = false;
            rb.velocity = Vector2.zero;
           
            
        }
       
    }
    protected virtual IEnumerator AttackPlayer()
    {
        MoveOn = false;
        yield return null;
    }


    public void ChangeState(MonsterState nextState)
    {
        curState = nextState;
      
    }
    public void TakeDamage( int damage )
    {
        if ( curState != MonsterState.Dead ) {
            thisMonsterHP -= damage;
            spriteRenderer.material.color = Color.red;
            spriteRenderer.material.DOColor(Color.white, 1f);
            damagedEffectPos.position = transform.position + Random.insideUnitSphere * 1f;
            Animator Effector = damagedEffect.GetComponent<Animator>();
           
            Effector.SetTrigger("Hit");
            
            if ( thisMonsterHP <= 0 )
            {
                MoveOn = false;
                ChangeState(MonsterState.Dead);
                animator.Play("Dead");
                StartCoroutine(ThisDestroy());

            }
            else
            {
                if (!MoveOn&&curState != MonsterState.Attack )
                {
                    Debug.Log(curState);
                    animator.Play("Damaged");
                    
                }

            }
        }
    }

    protected virtual IEnumerator ThisDestroy()
    {
        
        Manager.Sound.PlaySFX(monsterData.soundMonsterDead);
        int ran = Random.Range(0, 2);
        if(ran > 0 )
        {
            Animator Effector = damagedEffect.GetComponent<Animator>();
            Effector.SetTrigger("Hit");
            Effector.Play("CoinDrop");
            Manager.Game.GoldEvent += monsterData.dropGold;
        }
        
        Debug.Log("Coin");
        yield return new WaitForSeconds(1f);
       QuestUIManager.Ins.HandleMonsterDied(monsterData.id);
        int Ran = Random.Range(1, 2);
        if(Ran > 0 )
        {
        GameObject dropIns = Instantiate(dropItem, transform.position, Quaternion.identity);
            dropIns.GetComponent<Fielditem>().SetItem(monsterData.dropItem);
            dropIns.tag = "FieldItem";
            
        }
        Destroy(gameObject);
       

    }

}
