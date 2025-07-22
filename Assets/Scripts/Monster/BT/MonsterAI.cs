using System.Collections;
using UnityEngine;
using static NodeRunner;

public class MonsterAI : MonoBehaviour
{
    Monster _monster;
    Node _rootNode;

    NodeRunner _nodeRunner;
    bool _isRunning;

    private void Awake()
    {
        _monster = GetComponent<Monster>();
       
    }
    public IEnumerator Start()
    {
        if ( _monster == null )
        {
            Debug.LogError("Monster Component is null");
            yield return null;
        }


        string assetPath = $"MonsterPattern/{_monster.name} ({typeof(MonsterData).Name})";
        var resultAsset = assetPath.Replace("(Clone)", "");
        var asset = Resources.Load<BehaviourTreeAsset>(resultAsset);
        _rootNode = asset.rootNode;

        if ( _rootNode == null )
        {
            Debug.LogWarning($"{_monster.monsterData.name} data is null ");
            yield return null;

        }
        yield return new WaitUntil(() => NodeRunnerFactory.Instance.Complete = true);
        _nodeRunner = NodeRunnerFactory.Instance.Create(_rootNode, _monster);


    }
    private void Update()
    {
        if ( _isRunning || _nodeRunner == null )
            return;

        StartCoroutine(RunBT());
    }

    private IEnumerator RunBT()
    {
        Debug.Log("Run BT");
        _isRunning = true;

        yield return _nodeRunner.Execute();

        _isRunning = false;
    }

}

