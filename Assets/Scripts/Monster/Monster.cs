using DG.Tweening;
using System.Collections;
using UnityEngine;


public class Monster : MonoBehaviour, Idamagable
{

    public Animator animator;
    [SerializeField] public Collider2D [] colliders;
    protected Vector3 playerPos;
    [SerializeField] public SpriteRenderer spriteRenderer;
    public Vector2 moveDir;
    public bool MoveOn;
    public bool CRSwitch;
    public Collider2D thisCollider;
    public Vector2 atkDir;
    public bool atkDelayOn;
    


    public Collider2D player;
    [SerializeField] public LayerMask Obstacle;
    [SerializeField] public Rigidbody2D rb;
    public Collider2D hitWall;
    public bool flipXed;
    [SerializeField] public Animator damagedEffect;
    [SerializeField] public Transform damagedEffectPos;

    [SerializeField] public LayerMask playerLayer;
    [SerializeField] public PooledObject monsterPool;
    [SerializeField] public MonsterData monsterData;
    bool dead = false;
    public float localX;
    public float localY;
    public bool onBossAtk = false;
    public float thisMonsterHP;
    public float thisMonsterMaxHp;
    public GameObject parent;
    public GameObject dropItem;
    public GameObject Target { get => target; }

    GameObject target;
    


    protected virtual void Start()
    {
        ChangeState(MonsterState.Idle);
        thisMonsterHP = monsterData.hp;
        thisMonsterMaxHp = monsterData.hp;
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
    public void SetTarget(GameObject target) => this.target = target;
    

    protected virtual void Targeting()
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
        else if ( size < 1 )
        {
            ChangeState(MonsterState.Idle);

            animator.SetBool("Move", false);
        }




    }
    protected virtual void Moving()
    {
        if ( MoveOn )
        {


           // transform.Translate(moveDir * monsterData.speed / 100);
        }
    }
    protected virtual void Update()
    {

        if ( moveDir.x < 0 && !onBossAtk )
        {

            flipXed = true;
            spriteRenderer.flipX = true;



        }
        else if ( moveDir.x > 0 && !onBossAtk )
        {

            flipXed = false;
            spriteRenderer.flipX = false;


        }
        switch ( curState )
        {
            case MonsterState.Idle:



                IdleStates();
                break;

            case MonsterState.Chase:
                /*ChasePattern();
                player = Physics2D.OverlapCircle(transform.position, monsterData.attackRange, playerLayer);
                if ( player != null )
                {
                    atkDir = ( player.transform.position - transform.position ).normalized;
                    ChangeState(MonsterState.Attack);
                    MoveOn = false;
                    break;
                }
                break;*/

            case MonsterState.Attack:

                StartCoroutine(AttackPlayer());
                break;

            case MonsterState.Dead:

                DeadState();

                break;
        }
    }
    public enum MonsterState { Idle, Chase, Attack, Dead }
    protected virtual void DeadState()
    {

        if ( !dead )
        {
            dead = true;
            thisCollider.enabled = false;
            animator.Play("Dead");
            StartCoroutine(ThisDestroy());
        }

    }
    protected virtual void IdleStates()
    {
        rb.velocity = Vector2.zero;
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
    public virtual IEnumerator AttackPlayer()
    {
        MoveOn = false;
        yield return null;
    }
    public virtual IEnumerator ThisDestroy()
    {

        Manager.Sound.PlaySFX(monsterData.soundMonsterDead);
        int ran = Random.Range(0, 2);
        if ( ran > 0 )
        {
            Animator Effector = damagedEffect.GetComponent<Animator>();
            Effector.SetTrigger("Hit");
            Effector.Play("CoinDrop");
            Manager.Game.GoldEvent += monsterData.dropGold;
        }

        Debug.Log("Coin");
        yield return new WaitForSeconds(1f);
        Manager.Quest.HandleMonsterDied(monsterData.name);
        int Ran = Random.Range(1, 2);
        if ( Ran > 0 )
        {
            GameObject dropIns = Instantiate(dropItem, transform.position, Quaternion.identity);
            dropIns.GetComponent<Fielditem>().SetItem(monsterData.dropItem);
            dropIns.tag = "FieldItem";

        }
        Destroy(gameObject);


    }


    public IEnumerator MoveMonster()
    {

        if ( !CRSwitch )
        {
            CRSwitch = true;

            animator.SetBool("Move", true);
            yield return new WaitForSeconds(monsterData.moveDelay);

            CRSwitch = false;



        }
    }

    public void MoveSwitch()
    {
        animator.SetBool("Move", true);
        MoveOn = true;
    }
    public void MoveSwitchOff()
    {
        animator.SetBool("Move", false);
        MoveOn = false;
    }

    public MonsterState curState;
    public void ChangeState( MonsterState nextState )
    {
        if ( curState != MonsterState.Dead )
            curState = nextState;

    }

    public void TakeDamage( int damage )
    {
        if ( curState != MonsterState.Dead )
        {
            float actualDmg = Mathf.Min(damage, thisMonsterHP);
            thisMonsterHP -= actualDmg;
            spriteRenderer.material.color = Color.red;
            spriteRenderer.material.DOColor(Color.white, 1f);
            damagedEffectPos.position = transform.position + Random.insideUnitSphere * 2f;
            Animator Effector = damagedEffect.GetComponent<Animator>();

            Effector.SetTrigger("Hit");

            if ( thisMonsterHP <= 0 )
            {
                MoveOn = false;
                ChangeState(MonsterState.Dead);
            }
            else
            {
                if ( !MoveOn && curState != MonsterState.Attack )
                {
                    animator.Play("Damaged");
                }

            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, monsterData.Blackboard.Get<float>("range"));
    }
}
