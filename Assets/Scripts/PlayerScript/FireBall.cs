using UnityEngine;
using System.Collections;

public class FireBall : MonoBehaviour
{
    public float speed = 10f; 
    public int damage = 3;
    float lifeTime = 3f;
    [SerializeField] LayerMask targetLayer;
    [SerializeField] Animator anim;
    [SerializeField] ParticleSystem particle;
    [SerializeField] AudioClip fire;
    [SerializeField] AudioClip boom;


    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Manager.Sound.PlaySFX(fire);
        rb.velocity = transform.right * speed;
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
            Monster monster = other.GetComponent<Monster>();
            if ( monster != null )
            {
                Manager.Sound.PlaySFX(boom);
                monster.TakeDamage(damage);
                rb.velocity = Vector2.zero;
                anim.Play("FireballDead");
                Instantiate(particle, transform.position, Quaternion.Euler(-60f, 0f, 0f));
            }
        }
    }
    public void Dead()
    {
        Debug.Log("deadanim");
        if ( this != null)
        Destroy(gameObject);
    }

}
