using UnityEngine;

public static class Manager
{
    public static GameManager Game { get { return GameManager.Instance; } }
    public static DataManager Data { get { return DataManager.Instance; } }
    public static PoolManager Pool { get { return PoolManager.Instance; } }
    public static ResourceManager Resource { get { return ResourceManager.Instance; } }
    public static SceneManager Scene { get { return SceneManager.Instance; } }
    public static SoundManager Sound { get { return SoundManager.Instance; } }
    public static UIManager UI { get { return UIManager.Instance; } }
    public static QuestUIManager Quest { get { return QuestUIManager.Instance; } }
    public static inventoryManager inven { get { return inventoryManager.Instance; } }
    public static CoolTimeManager Cool { get { return CoolTimeManager.Instance; } }
    
    public static UICanvasManager UICanvas { get { return UICanvasManager.Instance; } }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        
     
        GameManager.ReleaseInstance();
        DataManager.ReleaseInstance();
        PoolManager.ReleaseInstance();
        ResourceManager.ReleaseInstance();
        SceneManager.ReleaseInstance();
        SoundManager.ReleaseInstance();
        UIManager.ReleaseInstance();
        
        CoolTimeManager.ReleaseInstance();
        UICanvasManager.ReleaseInstance();




        GameManager.CreateInstance();
        DataManager.CreateInstance();
        PoolManager.CreateInstance();
        ResourceManager.CreateInstance();
        SceneManager.CreateInstance();
        SoundManager.CreateInstance();
        UIManager.CreateInstance();
        CoolTimeManager.CreateInstance();
        UICanvasManager.CreateInstance();


    }
}
