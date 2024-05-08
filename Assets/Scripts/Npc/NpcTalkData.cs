using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "TalkData",menuName = "NpcData/TalkData")]
public class NpcTalkData : ScriptableObject
{





    [Header("Talk")]
    [SerializeField] public string [] dialogueLine;

}
