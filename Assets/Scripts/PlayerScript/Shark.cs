using UnityEngine;
using System.Collections;

public class Shark : MonoBehaviour
{
    float lifeTime = 3f;
    public int damage = 5;
   
    [SerializeField] LayerMask targetLayer;
    [SerializeField] Animator anim;
    [SerializeField] Collider2D col;
    [SerializeField] AudioClip sharkReady;
    [SerializeField] AudioClip sharkOn;

    bool onShark;
   [SerializeField] Rigidbody2D rb;

    void Start()
    {
       col.enabled = false;
        rb = GetComponent<Rigidbody2D>();   
        StartCoroutine(DestroyAfterLifetime());
        Manager.Sound.PlaySFX(sharkReady);

    }
    
    public IEnumerator DestroyAfterLifetime()
    {

        yield return new WaitForSeconds(lifeTime);
        if ( this != null )
            Destroy(gameObject);
    }
    void OnTriggerEnter2D( Collider2D other )
    {

        if (targetLayer.Contain(other.gameObject.layer) )
        {
            Debug.Log("SHarking");
            Monster monster = other.GetComponent<Monster>();
            if ( monster != null )
            {
               
                monster.TakeDamage(damage);
                int monsterHpUi = monster.monsterData.hp;
                string monsterNameUi = monster.monsterData.name;
                Manager.UICanvas.enemyDamagedEvent.Invoke();
                Manager.UICanvas.enemyHpUi.fillAmount = monster.thisMonsterHP / monsterHpUi;
                Manager.UICanvas.enemyNameUi.text = monsterNameUi;


            }
        }
    }

    public void SharkOn()
    {
       
        col.enabled = true;
        Manager.Sound.PlaySFX(sharkOn);
        anim.SetTrigger("sharkOn");
        
    }
    public void SharkOff()
    {
       
        col.enabled = false;
    }
    public void Dead()
    {
        Debug.Log("deadanim");
        if ( this != null )
            Destroy(gameObject);
    }

}
