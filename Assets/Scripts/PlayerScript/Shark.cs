using UnityEngine;
using System.Collections;
using DG.Tweening;
using TMPro;

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
