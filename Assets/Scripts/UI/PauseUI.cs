
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PauseUI : PopUpUI
{

   [SerializeField] OptionUI optionUI;
    [SerializeField] Transform player;

    protected override void Awake()
    {
        base.Awake();
        GetUI<Button>("Options").onClick.AddListener(Option);
        GetUI<Button>("Save").onClick.AddListener(Save);
        GetUI<Button>("Title").onClick.AddListener(Title);
        GetUI<Button>("Close").onClick.AddListener(Close);
    }
    public void Option()
    {
        Manager.UI.ShowPopUpUI(optionUI);

    }
    public void Title()
    {
        Time.timeScale = 1f;
        Manager.UI.ClearPopUpUI();
        Manager.Scene.LoadScene("TitleScene");
    }
    public void Save()
    {
        if ( Manager.Data == null )
        {
            Debug.LogError("Manager.Data is null");
            return;
        }

        if ( Manager.Data.GameData == null )
        {
            Debug.LogError("Manager.Data.GameData is null");
            return;
        }

        // 이하 코드는 Manager.Data와 Manager.Data.GameData가 null이 아닌 경우에만 실행됩니다.

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if ( playerObject == null )
        {
            Debug.LogError("Player object not found");
            return;
        }

        player = playerObject.transform;
        Manager.Data.GameData.playerPos = player.position;
        Manager.Data.GameData.Hp = Manager.Game.HpEvent;
        
        Manager.Data.GameData.maxHp = Manager.Game.MaxHpEvent;
        Manager.Data.GameData.Mp = Manager.Game.MpEvent;
        Manager.Data.GameData.maxMp = Manager.Game.MaxMpEvent;
        Manager.Data.GameData.gold = Manager.Game.GoldEvent;
        Manager.Data.SaveData();
        Debug.Log("Save");
        Manager.UI.ClearPopUpUI();
    }


}
