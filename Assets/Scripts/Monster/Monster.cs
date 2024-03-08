using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Monster : MonoBehaviour, Idamagable
{
    
    [SerializeField] string Name;
    [SerializeField] int atk;
    [SerializeField] int def;
    [SerializeField] int hp;
    bool dead;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
   Animator animator;
    FOV fov;
    private void Start()
    {
        animator = GetComponent<Animator>();
        fov = GetComponent<FOV>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();


    }
    private void Update()
    {
        
    }
    public void TakeDamage( int damage )
    {
        hp -= damage;
       

        if(hp <= 0 )
        {

            
            animator.Play("Dead"); //애니메이션 끝에 Destroy 할것
            StartCoroutine(ThisDestroy());

        }
        else
        {
            if ( !fov.MoveOn )
            {
                animator.Play("Damaged");
                
            }
            else
            {
               
                StartCoroutine(ColorMon());
            }

        }
    }

    public IEnumerator ThisDestroy()
    {
        fov.MoveOn = false;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
        
       

    }
    public IEnumerator ColorMon()
    {
        spriteRenderer.material.color = Color.red;
        Debug.Log("Color");
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material.color = Color.white;
    }
}
