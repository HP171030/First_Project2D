
using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerControll : MonoBehaviour
{
    [Header("Character Setting")]
    [SerializeField] int power;
    [SerializeField] float moveSpeed;
    [SerializeField] float dashSpeed;
    private Vector3 mousePosition;

    [Header("In Game Element")]
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] TrailRenderer dashTrail;
    [SerializeField] Animator skillEffect;
    [SerializeField] Transform skillEffectLocation;
    [SerializeField] Image DashskillCoolTime;
    [SerializeField] Image FireBallCoolTime;
    TrailRenderer dashTrailInstance;
    [SerializeField] protected float range;
    [SerializeField] Collider2D [] colliders = new Collider2D [5];
    [SerializeField] Transform effectDir;
    Vector2 lastMoveDirection;
    public GameObject fireBallPrefab;
    public GameObject sharkPrefab;
    [SerializeField] Image slashUi;
    [SerializeField] Image skUi1;
    [SerializeField] Image skUi2;
    [SerializeField] GameObject playerMarker;   

    
    float Xdir;
    float Ydir;
    bool up;
    bool left;
    bool right;
    bool down;
    public UnityEvent DieEvent;

    [Header("PlayerUI")]
    [SerializeField] PauseUI pauseUI;
    [SerializeField] DeadUi deadui;


    [Header("Character Sound")]

    [SerializeField] AudioClip soundAtk;
    [SerializeField] AudioClip soundDash;
    [SerializeField] AudioClip soundHit;
    [SerializeField] AudioClip soundHit2;
    [SerializeField] AudioClip soundHit3;
    [SerializeField] AudioClip soundDie;


    [Header("Script Element")]
    [SerializeField] LayerMask monster;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] bool dashOn;
    [SerializeField] bool atkOn = false;

    bool die;

    [SerializeField] Animator animator;
    [SerializeField] LayerMask targetLayer;
    protected Coroutine coolDown;
    protected bool CoolChecker = false;

    PlayerStatus status;

    Monster hitedMonster;


    public enum SkillState { Slash, FireBall, Shark }

    public SkillState curSkillState;

    private void Start()
    {
        ChangeSkill(SkillState.Slash);
        slashUi = Manager.UICanvas.slash;
        skUi1 = Manager.UICanvas.skill1;
        skUi2 = Manager.UICanvas.skill2;
        DashskillCoolTime = Manager.UICanvas.dash;
        slashUi.color = Color.green;
        skUi1.color = Color.clear;
        skUi2.color = Color.clear;
        Manager.Game.DieEvent.AddListener(Die);
        DontDestroyOnLoad(gameObject);


    }
    public void ChangeSkill(SkillState skillstate)
    {
        curSkillState = skillstate;
    }
    private void Update()
    {

        if ( !Manager.Game.time )
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            playerMarker.transform.rotation = Quaternion.Euler(0f, 0f, (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg)-90);
        }

        if ( !die )
        {
            Move();
            PlayerDir();
        }


    }

    public void Move()
    {
        if ( atkOn )
        {
            moveSpeed = 2f;
        }

        if ( Xdir > 0 || Ydir > 0 || Xdir < 0 || Ydir < 0 )
        {

            lastMoveDirection = new Vector2(Xdir, Ydir);
        }

        animator.SetBool("Up", up ? true : false);
        animator.SetBool("Down", down ? true : false);
        animator.SetBool("Left", left ? true : false);
        animator.SetBool("Right", right ? true : false);

        sprite.flipX = left || Xdir < 0 ? true : false;



        transform.Translate(Xdir * moveSpeed * Time.deltaTime, Ydir * moveSpeed * Time.deltaTime, 0, Space.Self);
    }
    public void DashOff()
    {
        animator.SetBool("Dash", false);
        rb.velocity = Vector3.zero;
        if ( dashTrailInstance != null && dashTrailInstance.gameObject != null )
        {
            Destroy(dashTrailInstance.gameObject);
        }


    }
    public IEnumerator DashCooltime( float coolTime )
    {
        float initial = coolTime;
        while ( coolTime >= 0f )
        {

            coolTime -= Time.deltaTime;
            DashskillCoolTime.fillAmount = coolTime / initial;
            yield return null;
        }
        dashOn = false;
    }
    public IEnumerator FireBallCooltime( float coolTime )
    {
        float initial = coolTime;
        while ( coolTime >= 0f )
        {

            coolTime -= Time.deltaTime;
            DashskillCoolTime.fillAmount = coolTime / initial;
            yield return null;
        }
        dashOn = false;
    }
    public void Die()
    {
        animator.SetBool("Die", true);
        Manager.Sound.PlaySFX(soundDie);
        Manager.UICanvas.gameObject.SetActive(false);
        die = true;

    }
    public void DieOff()
    {
        die = false;
        animator.SetBool("Die", false);
        Manager.UI.ShowPopUpUI(Manager.Game.deadui);
 
    }
    public IEnumerator MonsterDamaged( float delay, Rigidbody2D rb, Vector2 dir )
    {

        rb.AddForce(dir * 14f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(delay);
        if ( rb != null )
            rb.velocity = Vector2.zero;

    }
    public void OnAttack( InputValue value )
    {
        if ( value.isPressed && !atkOn && !die&&!IsPointerOverUIObject())
        {
            
            Attack();
            DashOff();



        }
        else if ( atkOn )
        {

        }

    }
    
    public void OnDash( InputValue value )
    {
        if ( value.isPressed && !dashOn )
        {
            Dash();

        }
        else if ( dashOn )
        {
            Debug.Log("WaitCool");
        }
    }


    public void OnInven( InputValue value )
    {
        Manager.inven.OpenInven();

    }
    public void OnQuest( InputValue value )
    {
        Manager.Quest.OpenQuest();
    }

    protected IEnumerator CoolDown( float coolTime )
    {
        CoolChecker = true;
        yield return new WaitForSeconds(coolTime);
        CoolChecker = false;

    }
    protected IEnumerator AttackAnim()
    {
        switch(curSkillState)

        {
            case SkillState.Slash:
                animator.SetBool("Attack", true);
                skillEffect.Play("Slash");
                yield return new WaitForSeconds(0.15f);
                animator.SetBool("Attack", false);
                atkOn = false;
                moveSpeed = 8f;
                break;
            case SkillState.FireBall:
                animator.SetBool("Attack", true);
                yield return new WaitForSeconds(0.3f);
                animator.SetBool("Attack", false);
                atkOn = false;
                moveSpeed = 8f;
                break;
            case SkillState.Shark:
                animator.SetBool("Attack", true);
                yield return new WaitForSeconds(0.3f);
                animator.SetBool("Attack", false);
                atkOn = false;
                moveSpeed = 8f;
                break;
        }
       
        
    }
    public void OnSkillSlash(InputValue value )
    {
        ChangeSkill(SkillState.Slash);
        slashUi.color = Color.green;
        skUi1.color = Color.clear;
        skUi2.color = Color.clear;

    }
    public void OnSkill1(InputValue value )
    {
        ChangeSkill(SkillState.FireBall);
        slashUi.color = Color.clear;
        skUi1.color = Color.green;
        skUi2.color = Color.clear;
    }
    public void OnSkill2(InputValue value )
    {
        ChangeSkill(SkillState.Shark);
        slashUi.color = Color.clear;
        skUi1.color = Color.clear;
        skUi2.color = Color.green;
    }
    private void OnTriggerEnter2D( Collider2D collision )
    {

        if ( collision.CompareTag("FieldItem") )
        {

            Fielditem fielditem = collision.GetComponent<Fielditem>();
            if ( Manager.inven.AddItem(fielditem.GetItem()) )
                fielditem.DestroyItem();
        }
    }
    #region 플레이어 조작
    public void PlayerDir()
    {

        if ( Xdir > 0 )
        {
            DirIsRightK();
        }
        else if ( Xdir < 0 )
        {
            DirIsLeftK();
        }

        if ( Ydir > 0 )
        {
            DirIsUpK();
        }
        else if ( Ydir < 0 )
        {
            DirIsDownK();
        }


    }
    public void DirIsRightK()
    {
        right = true;
        down = false;
        left = false;
        up = false;
    }
    public void DirIsLeftK()
    {
        left = true;
        right = false;
        up = false;
        down = false;

    }
    public void DirIsUpK()
    {
        up = true;
        down = false;
        left = false;
        right = false;

    }

    public void DirIsDownK()
    {
        up = false;
        down = true;
        right = false;
        left = false;

    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
    public void SetEffectDirection()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;


        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;



        // 각도에 따라 캐릭터 방향 설정
        if ( angle < 0 )
            angle += 360;

        if ( angle >= 337.5f || angle < 22.5f )
        {
            DirIsRight();
            DirIsRightK();

        }
        else if ( angle >= 22.5f && angle < 67.5f )
        {
            DirIsUpRight();
            DirIsUpK();
        }



        else if ( angle >= 67.5f && angle < 112.5f )
        {
            DirIsUp();
            DirIsUpK();
        }

        else if ( angle >= 112.5f && angle < 157.5f )
        {
            DirIsUpLeft();
            DirIsUpK();
        }

        else if ( angle >= 157.5f && angle < 202.5f )
        {
            DirIsLeft();
            DirIsLeftK();
        }

        else if ( angle >= 202.5f && angle < 247.5f )
        {
            DirIsDownK(); DirIsDownLeft();
        }

        else if ( angle >= 247.5f && angle < 292.5f )
        {
            DirIsDown();
            DirIsDownK();
        }

        else if ( angle >= 292.5f && angle < 337.5f )
        {
            DirIsDownK();
            DirIsDownRight();
        }


        skillEffectLocation.position = effectDir.position;
        skillEffectLocation.rotation = Quaternion.Euler(0, 0, angle);
       
    }

    void DirIsRight()
    {
        effectDir.position = new Vector2(transform.position.x + 1, transform.position.y);
    }

    void DirIsUpRight()
    {
        effectDir.position = new Vector2(transform.position.x + 1, transform.position.y + 1);
    }

    void DirIsUp()
    {
        effectDir.position = new Vector2(transform.position.x, transform.position.y + 1);
    }

    void DirIsUpLeft()
    {
        effectDir.position = new Vector2(transform.position.x - 1, transform.position.y + 1);
    }

    void DirIsLeft()
    {
        effectDir.position = new Vector2(transform.position.x - 1, transform.position.y);
    }

    void DirIsDownLeft()
    {
        effectDir.position = new Vector2(transform.position.x - 1, transform.position.y - 1);
    }

    void DirIsDown()
    {
        effectDir.position = new Vector2(transform.position.x, transform.position.y - 1);
    }

    void DirIsDownRight()
    {
        effectDir.position = new Vector2(transform.position.x + 1, transform.position.y - 1);
    }
    #endregion
    public void Dash()
    {

        Vector2 moveDir = new Vector2(Xdir, Ydir);
        if ( moveDir.magnitude > 0 )
        {
            Manager.Sound.PlaySFX(soundDash);
            dashTrailInstance = Instantiate(dashTrail, transform.position, transform.rotation, transform);
            dashTrailInstance.enabled = true;
            dashOn = true;

            animator.SetBool("Dash", true);
            StartCoroutine(DashOn());
            rb.velocity = moveDir.normalized * dashSpeed;
            StartCoroutine(DashCooltime(1.5f));

        }
    }
    public IEnumerator DashOn()
    {
        yield return new WaitForSeconds(0.25f);
        DashOff();
    }
    public void OnMove( InputValue value )
    {
        Vector2 moveDir = value.Get<Vector2>();
        Xdir = moveDir.x;
        Ydir = moveDir.y;

        if ( moveDir.magnitude > 0 && !die )
        {
            animator.SetBool("isMove", true);
            animator.SetFloat("Xdir", Xdir);
            animator.SetFloat("Ydir", Ydir);
        }
        else
        {

            animator.SetBool("isMove", false);
            Xdir = 0;
            Ydir = 0;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(skillEffectLocation.position, range);
    }
    public void Attack()
    {
        SetEffectDirection();
        
        switch ( curSkillState )
        {
            case SkillState.Slash:
                if ( !CoolChecker )
                {
                    coolDown = StartCoroutine(CoolDown(0.3f));
                    Manager.Sound.PlaySFX(soundAtk);
                    moveSpeed = 2f;                                 //공격시 이속저하
                    StartCoroutine(AttackAnim());


                    int size = Physics2D.OverlapCircleNonAlloc(skillEffectLocation.position, range, colliders, monster);

                    if ( size > 0 )
                    {

                        for ( int i = 0; i < size; i++ )
                        {

                            if ( targetLayer.Contain(colliders [i].gameObject.layer) )
                            {

                                hitedMonster = colliders [i].gameObject.GetComponent<Monster>();
                                if ( hitedMonster != null )
                                {
                                    Vector2 AttackedDir = ( colliders [i].transform.position - skillEffectLocation.position ).normalized;
                                    Rigidbody2D rb = colliders [i].GetComponent<Rigidbody2D>();

                                    hitedMonster.TakeDamage(power);
                                    MonsterHit();



                                    #region 피격음 1,2 랜덤생성
                                    int soundIndex = Random.Range(1, 4);
                                    switch ( soundIndex )
                                    {
                                        case 1:
                                            Manager.Sound.PlaySFX(soundHit);
                                            break;
                                        case 2:
                                            Manager.Sound.PlaySFX(soundHit2);
                                            break;
                                        case 3:
                                            Manager.Sound.PlaySFX(soundHit3);
                                            break;
                                    }
                                    #endregion
                                    if ( hitedMonster.curState != Monster.MonsterState.Dead )
                                    {
                                        StartCoroutine(MonsterDamaged(0.2f, rb, AttackedDir));

                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    
                }
                break;

            case SkillState.FireBall:
               if( Manager.Cool.IsSkillCool("FireBall"))
                {
                    if(Manager.Game.MpEvent > 10 )
                    {
                        Manager.Game.MpEvent -= 10;
                    moveSpeed = 2f;                             //스킬 사용시 이속저하
                    StartCoroutine(AttackAnim());
                    Manager.Cool.UseSkill("FireBall");
                   GameObject fire = Instantiate(fireBallPrefab,skillEffectLocation.position,Quaternion.identity);
                    fire.transform.rotation = skillEffectLocation.rotation;
                    }
                }
                else
                {
                    Debug.Log("FireBall CoolTIme");
                }
               
                break;

            case SkillState.Shark:
                Vector2 mousePos = new Vector2(mousePosition.x, mousePosition.y);
                if ( Manager.Cool.IsSkillCool("Shark") )
                {
                    if(Manager.Game.MpEvent > 30 )
                    {

                    moveSpeed = 2f;                             //스킬 사용시 이속저하
                    StartCoroutine(AttackAnim());
                    Manager.Cool.UseSkill("Shark");
                    GameObject shark = Instantiate(sharkPrefab,mousePos, Quaternion.identity);
                        Manager.Game.MpEvent -= 30;
                    }
                   
                }
                else
                {
                    Debug.Log("Shark CoolTIme");
                }
               
                break;




            

        }
        
    }
    public void MonsterHit()
    {
        float monsterHpUi = hitedMonster.thisMonsterMaxHp;
        string monsterNameUi = hitedMonster.monsterData.name;
        Manager.UICanvas.enemyDamagedEvent.Invoke();
        Manager.UICanvas.enemyHpUi.fillAmount = hitedMonster.thisMonsterHP / monsterHpUi;
        Manager.UICanvas.enemyHpText.text = $"{hitedMonster.thisMonsterHP} /  {monsterHpUi}";
        Debug.Log($"{hitedMonster.thisMonsterHP} /  {monsterHpUi}");
        Manager.UICanvas.enemyNameUi.text = monsterNameUi;
        PooledObject dmgui = Manager.Pool.GetPool(Manager.Game.damageUI, hitedMonster.transform.position, Quaternion.identity);
        TMP_Text dmgtext = dmgui.gameObject.GetComponent<TMP_Text>();
        dmgtext.text = power.ToString();
        Sequence sequence = DOTween.Sequence();
        sequence.Append(dmgui.transform.DOMoveY(dmgui.transform.position.y + 2f, 0.5f).SetEase(Ease.OutQuad)); 
        sequence.Append(dmgui.transform.DOMoveY(dmgui.transform.position.y, 0.5f).SetEase(Ease.InQuint)); 

    }
}





