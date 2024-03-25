using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireExplosion : MonoBehaviour
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] AudioClip fire;
    [SerializeField] AudioClip boom;

    [SerializeField] Collider2D Range;


    private void OnEnable()
    {
        Range.enabled = false;
        
    }
    void OnTriggerEnter2D( Collider2D player )
    {

            if ( player.CompareTag("Player") )
            {
                Manager.Game.HpEvent -= 15;
                Manager.Sound.PlaySFX(boom);
                SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
                spriteRenderer.material.color = Color.red;
            Manager.Game.ShakeCam();
            spriteRenderer.material.DOColor(Color.white, 1f);

            }
        
       
    }

    public void onDamaged()
    {
        Range.enabled = true;
    }
    public void offDamaged()
    {
        Range.enabled = false;
    }
}

