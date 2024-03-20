using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FireBoss : Monster
{
    [SerializeField] string [] bossString;
    [SerializeField] CinemachineVirtualCamera cineCam;
    public void animEnd()
    {
        Manager.Game.HerePlayer();
        PlayerInput input = Manager.Game.player.GetComponent<PlayerInput>();
        Manager.UICanvas.dialogue.enabled = true;
        Manager.UICanvas.text.enabled = true;

        Queue<string> strings = new Queue<string>(bossString.Length);
        for ( int i = 0; i < bossString.Length; i++ )
        {

            strings.Enqueue(bossString [i]);
        }

        StartCoroutine(WaitSpace());
        Manager.UICanvas.text.text = strings.Dequeue();
        DoTweenText.DoText(Manager.UICanvas.text, 0.2f);
        IEnumerator WaitSpace()
        {
            while ( true )
            {
                yield return null;
                if ( Input.GetKeyDown(KeyCode.Space) )
                {
                    Next();
                    yield break;
                }
            }
        }
        void Next()
        {
            if ( strings.Count > 0 )
            {
                if ( Input.GetKeyDown(KeyCode.Space) )
                {
                    Manager.UICanvas.text.text = strings.Dequeue();
                    DoTweenText.DoText(Manager.UICanvas.text, 0.2f);

                    StartCoroutine(WaitSpace());
                }
            }
            else
            {

                Manager.UICanvas.dialogue.enabled = false;
                Manager.UICanvas.text.enabled = false;
                input.enabled = true;

                cineCam.Follow = Manager.Game.player.transform;
                return;
            }
        }

    }

    
}
