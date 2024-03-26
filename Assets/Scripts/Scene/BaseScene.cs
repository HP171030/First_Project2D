    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public abstract class BaseScene : MonoBehaviour
    {
        public abstract IEnumerator LoadingRoutine();

        public virtual IEnumerator OnStartScene() { yield return null; }

        public virtual void SceneSave()
        {

        }

        public virtual void SceneLoad()
        {
            Manager.Game.MaxHpEvent = Manager.Data.GameData.maxHp;
            Manager.Game.MaxMpEvent = Manager.Data.GameData.maxMp;
            Manager.Game.HpEvent = Manager.Data.GameData.Hp;
            Manager.Game.MpEvent = Manager.Data.GameData.Mp;
            Manager.Game.GoldEvent = Manager.Data.GameData.gold;
        foreach ( KillQuest quest in Manager.Data.GameData.questList )
        {
            
            Manager.Quest.AddKillQuest(quest);
            Debug.Log(quest);
        }

    }
    }
