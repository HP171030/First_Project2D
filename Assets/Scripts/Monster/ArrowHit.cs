using DG.Tweening;
using System.Collections;
using UnityEngine;

public class ArrowHit : MonoBehaviour
{
    Archer archer;
    Transform target;

    public void SetArcher( Archer archerRef )
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
            Manager.Game.HpEvent -= 15;
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
}
