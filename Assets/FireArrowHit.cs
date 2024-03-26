using DG.Tweening;
using System.Collections;
using UnityEngine;

public class FireArrowHit : MonoBehaviour
{
    FireArcher archer;
    Transform target;
    Animator animator;

    private void Start()
    {
        StartCoroutine(DeadDelay());
    }
    public void SetArcher( FireArcher archerRef )
    {
        archer = archerRef;
        
        
    }
    private void OnTriggerEnter2D( Collider2D collision )
    {
        if ( collision.CompareTag("Player") )
        {
            target = collision.transform;
            Manager.Game.ShakeCam();
            SpriteRenderer spriteRenderer = collision.GetComponent<SpriteRenderer>();
            Manager.Game.HpEvent -= 5;
            spriteRenderer.material.color = Color.red;
            spriteRenderer.material.DOColor(Color.white, 1f);
            Debug.Log("stop");
            archer.StopArrowShot();
            StartCoroutine(ChaseAndDestroy());
            
        }
        IEnumerator ChaseAndDestroy()
        {
            float t = 0;
            while ( t < 3f )
            {
                t += Time.deltaTime;
                transform.DOMove(target.position, 0f);

                yield return null;
            }


            Destroy(gameObject);
        }


    }

    public IEnumerator DeadDelay()
    {
        if(this != null)
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
