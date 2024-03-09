using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1 : MonoBehaviour
{
    [SerializeField]AudioClip startBGM;
    private void Start()
    {
        Manager.Sound.PlayBGM(startBGM);
    }
}
