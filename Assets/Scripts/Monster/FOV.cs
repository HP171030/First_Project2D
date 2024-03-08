using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.Scripting.APIUpdating;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FOV : MonoBehaviour
{
    [SerializeField] float Range;
    [SerializeField] Collider2D [] colliders;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] Vector3 playerPos;
    [SerializeField] float speed;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Vector2 moveDir;
    [SerializeField] public bool MoveOn;
    [SerializeField] bool CRSwitch;
    
    private void Start()
    {
        
        colliders = new Collider2D [20];
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        Targeting();
        Moving();

    }

    public void Targeting()
    {
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, Range, colliders, playerLayer);
        if ( moveDir.x < 0 )
        {

            spriteRenderer.flipX = true;
        }
        else if ( moveDir.x > 0 )
        {
            spriteRenderer.flipX = false;
        }
        if ( size > 0 && !CRSwitch )
        {
            Debug.Log("Find");

            for ( int i = 0; i < size; i++ )
            {
                if ( playerLayer.Contain(colliders [i].gameObject.layer) )
                {
                    playerPos = colliders [i].transform.position;
            moveDir = ( playerPos - transform.position ).normalized;
            StartCoroutine(MoveMonster());
                    return;
                }
            }

        }
        else
        {
            Debug.Log("No");
        }

    }

    public IEnumerator MoveMonster()
    {
        Animator animator = GetComponent<Animator>();
        CRSwitch = true;
        animator.Play("Move");
        
        yield return new WaitForSeconds(2f);
        CRSwitch = false;



    }

    public void Moving()
    {
       

        if ( MoveOn)
            {
            transform.Translate(moveDir * speed / 50);
        }
    }
    public void moveSwitch()
    {
        MoveOn = true;
    }
    public void moveSwitchOff()
    {
        MoveOn = false;
    }
}


