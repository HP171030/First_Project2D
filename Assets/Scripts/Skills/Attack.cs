using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Skill
{
    [SerializeField] protected float coolTime = 2f;
    [SerializeField] protected float range;
    [SerializeField] Collider2D [] colliders = new Collider2D [5];
    [SerializeField] LayerMask targetLayer;

    protected Coroutine coolDown;
    protected bool CoolChecker = false;


    public override void SkillStart()
    {
        if(!CoolChecker)
        {
       coolDown =  StartCoroutine(CoolDown());

             int size = Physics2D.OverlapCircleNonAlloc(transform.position, range,colliders);
        if (size > 0)
        {
           
             for ( int i = 0; i < size; i++ )
            {
                
                if ( targetLayer.Contain(colliders [i].gameObject.layer) )
                {
                    Debug.Log("hit");   
                }
            }
        }
                   else
                 {
            Debug.Log("noHit");
              }
           }

        else
        {
            Debug.Log("WaitCoolDown" + coolTime);
        }
        
    }

    protected IEnumerator CoolDown()
    {
        CoolChecker = true;
        yield return new WaitForSeconds(coolTime);
        CoolChecker = false ;

    }

}