using Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BossEngage : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cineCam;
    [SerializeField] PlayerInput input;
    [SerializeField] GameObject Boss;
    [SerializeField] Animator anim;

    public UnityEvent bossEngageEvent;
    private void OnTriggerEnter2D( Collider2D collision )
    {
        Manager.Game.HerePlayer();
        PlayerInput input = Manager.Game.player.GetComponent<PlayerInput>();
        input.enabled = false;
        cineCam.Priority = 100;
        Boss.SetActive(true);
        anim.Play("FireBossEngage");

    }
}
