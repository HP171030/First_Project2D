using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastTele : MonoBehaviour
{
    [SerializeField] Animator anim;
    private void OnTriggerEnter2D( Collider2D collision )
    {
        if ( collision.CompareTag("Player"))
        anim.SetTrigger("Teleport");
       
    }

    public void Teleport()
    {
        Manager.UICanvas.gameObject.SetActive(false);
        Manager.Scene.LoadScene("NextScene");
    }
}
