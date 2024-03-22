using UnityEngine;
using System.Collections;

public class FireEnergy : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 3;
    float lifeTime = 3f;

    [SerializeField] LayerMask targetLayer;
    [SerializeField] Animator anim;
    [SerializeField] ParticleSystem particle;
    [SerializeField] AudioClip fire;
    [SerializeField] AudioClip boom;


    public Rigidbody2D rb;

    void Start()
    {
        
        
        Manager.Sound.PlaySFX(fire);
        StartCoroutine(DestroyAfterLifetime());

    }
    public IEnumerator DestroyAfterLifetime()
    {

        yield return new WaitForSeconds(lifeTime);
        if ( this != null )
            Destroy(gameObject);
    }
    void OnTriggerEnter2D( Collider2D other )
    {

        if ( targetLayer.Contain(other.gameObject.layer) )
        {
           
            if ( other != null )
            {

                Manager.Game.HpEvent -= 5;
                Manager.Game.ShakeCam();
                rb.velocity = Vector3.zero;
                anim.Play("FlameOver");
                Manager.Sound.PlaySFX(boom);
                Instantiate(particle, transform.position, Quaternion.Euler(-60f, 0f, 0f));
            }
        }
    }
    public void Dead()
    {
        Debug.Log("deadanim");
        if ( this != null )
            Destroy(gameObject);
    }

}
