using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class PlayerControll : MonoBehaviour
{
   [SerializeField] SpriteRenderer sprite;

    [SerializeField] int power;
    [SerializeField] float moveSpeed;
    [SerializeField] float dashSpeed;
    [SerializeField] float Xdir;
    [SerializeField] float Ydir;
    [SerializeField] bool up;
    [SerializeField] bool left;
    [SerializeField] bool right;
    [SerializeField] bool down;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] bool dashOn;
    [SerializeField] bool atkOn = false;
    [SerializeField] TrailRenderer dashTrail;
    [SerializeField] Animator skillEffect;
    [SerializeField] Transform skillEffectLocation;
    [SerializeField] Image skillCoolTime;
    TrailRenderer dashTrailInstance;
    [SerializeField] LayerMask monster;

    [SerializeField] Animator animator;
    [SerializeField] protected float range;
    [SerializeField] Collider2D [] colliders = new Collider2D [5];
    [SerializeField] LayerMask targetLayer;
    [SerializeField] Transform effectDir;
    Vector2 lastMoveDirection;

    protected Coroutine coolDown;
    protected bool CoolChecker = false;
    float curMoveMag;






    private void Update()
    {
        Move();
        PlayerDir();

    }

    public void Move()
    {
        if ( atkOn )
        {
            moveSpeed = 5f;
        }

        if(Xdir>0 || Ydir > 0 || Xdir < 0 || Ydir < 0 )
        {

        lastMoveDirection = new Vector2(Xdir,Ydir);
        }

        animator.SetBool("Up", up ? true : false);
        animator.SetBool("Down", down ? true : false);
        animator.SetBool("Left", left ? true : false);
        animator.SetBool("Right", right ? true : false);
      
        sprite.flipX = left || Xdir < 0 ? true : false;
        
       

        transform.Translate(Xdir*moveSpeed*Time.deltaTime, Ydir*moveSpeed * Time.deltaTime, 0, Space.Self);
    }   
    public void DashOff()
    {
        animator.SetBool("Dash", false);
        rb.velocity = Vector3.zero;
        if ( dashTrailInstance != null && dashTrailInstance.gameObject != null )
        {
            Destroy(dashTrailInstance.gameObject);
        }


    }
    public IEnumerator DashCooltime(float coolTime)
    {
        float initial = coolTime;
        while ( coolTime >= 0f )
        {
            
            coolTime -= Time.deltaTime;
            skillCoolTime.fillAmount = coolTime / initial;
            yield return null;
        }
        dashOn = false;

    }
    public void Dash()
    {
        
        Vector2 moveDir = new Vector2(Xdir, Ydir);
        if(moveDir.magnitude > 0)
        {
            dashTrailInstance = Instantiate(dashTrail,transform.position,transform.rotation,transform);
            dashTrailInstance.enabled = true;
            dashOn = true;
           
        animator.SetBool("Dash", true);
            StartCoroutine(DashOn());
        rb.velocity = moveDir.normalized*dashSpeed;
        StartCoroutine(DashCooltime(1.5f));

        }
    }
    public IEnumerator DashOn()
    {
        yield return new WaitForSeconds(0.5f);
        DashOff();
    }
    public void OnMove(InputValue value)
    {
        Vector2 moveDir = value.Get<Vector2>();
         Xdir = moveDir.x;
         Ydir = moveDir.y;

        if(moveDir.magnitude > 0)
        {
            animator.SetBool("isMove", true);
            animator.SetFloat("Xdir", Xdir);
            animator.SetFloat("Ydir", Ydir);
        }
        else 
        {
            
            animator.SetBool("isMove", false);
            Xdir = 0;
            Ydir = 0;
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(skillEffectLocation.position, range);
    }
    public void Attack()
    {
       

        if ( !CoolChecker )
        {
            coolDown = StartCoroutine(CoolDown(0.3f));
            
            atkOn = true;
            StartCoroutine(AttackAnim());

            int size = Physics2D.OverlapCircleNonAlloc(skillEffectLocation.position, range, colliders,monster);

            if ( size > 0 )
            {

                for ( int i = 0; i < size; i++ )
                {

                    if ( targetLayer.Contain(colliders [i].gameObject.layer) )
                    {
                        
                       Monster monster = colliders [i].gameObject.GetComponent<Monster>();
                        if ( monster != null )
                        {
                            Vector2 AttackedDir = ( colliders [i].transform.position - skillEffectLocation.position ).normalized;
                            Rigidbody2D rb = colliders [i].GetComponent<Rigidbody2D>();

                            monster.TakeDamage(power);
                            StartCoroutine(MonsterDamaged(0.2f, rb, AttackedDir));
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("WaitCoolDown");
        }
    }

    public IEnumerator MonsterDamaged(float delay,Rigidbody2D rb,Vector2 dir )
    {
        
        rb.AddForce(dir * 14f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(delay);
        if(rb !=null )
        rb.velocity = Vector2.zero;

    }
    public void OnAttack(InputValue value)
    {
       if( value.isPressed&&!atkOn )
        {
            Attack();
            DashOff();
         


        }
       else if ( atkOn )
        {
            Debug.Log("WaitAtk");
        }

    }

    public void OnDash(InputValue value)
    {
        if ( value.isPressed&&!dashOn )
        {
            Dash();
            
        }
        else if ( dashOn )
        {
            Debug.Log("WaitCool");
        }


    }

    protected IEnumerator CoolDown(float coolTime)
    {
        CoolChecker = true;
        yield return new WaitForSeconds(coolTime);
        CoolChecker = false;

    }
    protected IEnumerator AttackAnim()
    {
        skillEffectLocation.position = effectDir.position;
        float angle = Mathf.Atan2(lastMoveDirection.x, -lastMoveDirection.y) * Mathf.Rad2Deg;
        skillEffectLocation.rotation = Quaternion.Euler(0, 0, angle);


        animator.SetBool("Attack", true);
        skillEffect.Play("Slash");
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("Attack",false);
        atkOn = false;
        moveSpeed = 10f;
    }

    public void PlayerDir()
    {
        
        if ( Xdir > 0 )
        {
            right = true;
            down = false;
            left = false;
            up = false;

            if ( Ydir >0 )
            {

                effectDir.position = new Vector2(transform.position.x + 1, transform.position.y + 1);
             
            }
            else if (Ydir == 0 )
            {
                effectDir.position = new Vector2(transform.position.x + 1, transform.position.y);
            }
            else if ( Ydir < 0 )
            {
               effectDir.position =  new Vector2(transform.position.x + 1, transform.position.y - 1);
            }
        }
        else if ( Xdir < 0 )
        {
            left = true;
            right = false;
            up = false;
            down = false;

            if ( Ydir > 0 )
            {

                effectDir.position = new Vector2(transform.position.x - 1, transform.position.y + 1);
            }
            else if ( Ydir == 0 )
            {
                effectDir.position = new Vector2(transform.position.x - 1, transform.position.y);
            }
            else if ( Ydir < 0 )
            {
                effectDir.position = new Vector2(transform.position.x - 1, transform.position.y - 1);
            }
        }

        if(Ydir > 0 )
        {
            up = true;
            down = false;
            left = false;
            right = false;

            if (Xdir > 0 )
            {

                effectDir.position = new Vector2(transform.position.x + 1, transform.position.y + 1);
            }
            else if ( Xdir == 0 )
            {
                effectDir.position = new Vector2(transform.position.x, transform.position.y+1);
            }
            else if ( Xdir < 0 )
            {
                effectDir.position = new Vector2(transform.position.x - 1, transform.position.y + 1);
            }
        }
        else if( Ydir < 0 )
        {
            up = false;
          down = true;
            right = false;
            left = false;

            if ( Xdir > 0 )
            {

                effectDir.position = new Vector2(transform.position.x + 1, transform.position.y - 1);
            }
            else if ( Xdir == 0 )
            {
                effectDir.position = new Vector2(transform.position.x, transform.position.y - 1);
            }
            else if ( Xdir < 0 )
            {
                effectDir.position = new Vector2(transform.position.x - 1, transform.position.y - 1);
            }
        }

        
    }


  
}



