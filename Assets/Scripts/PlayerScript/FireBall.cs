using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

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
                #region 몬스터데미지Ui
                float monsterHpUi = monster.thisMonsterMaxHp;
                string monsterNameUi = monster.monsterData.name;
                Manager.UICanvas.enemyDamagedEvent.Invoke();
                Manager.UICanvas.enemyHpUi.fillAmount = monster.thisMonsterHP / monsterHpUi;
                Manager.UICanvas.enemyHpText.text = $"{monster.thisMonsterHP} /  {monsterHpUi}";
                Debug.Log($"{monster.thisMonsterHP} /  {monsterHpUi}");
                Manager.UICanvas.enemyNameUi.text = monsterNameUi;
                PooledObject dmgui = Manager.Pool.GetPool(Manager.Game.damageUI, monster.transform.position, Quaternion.identity);
                TMP_Text dmgtext = dmgui.gameObject.GetComponent<TMP_Text>();
                dmgtext.text = damage.ToString();
                Sequence sequence = DOTween.Sequence();
                sequence.Append(dmgui.transform.DOMoveY(dmgui.transform.position.y + 2f, 0.5f).SetEase(Ease.OutQuad));
                sequence.Append(dmgui.transform.DOMoveY(dmgui.transform.position.y, 0.5f).SetEase(Ease.InQuint));
                #endregion
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
