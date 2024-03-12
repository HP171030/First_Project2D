using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuestNPC : MonoBehaviour
{
    [SerializeField] Transform mimicRegen2;
    [SerializeField] float mimicSpawnInterval;
    [SerializeField] PooledObject monsterMimic;
    private IEnumerator SpawnMimicsRoutine()
    {
        int limit = 0;
        while ( true )
        {
            yield return new WaitForSeconds(mimicSpawnInterval);

            if ( monsterMimic != null && mimicRegen2 != null && limit < 10 )
            {
                int ranSize = Random.Range(1, 4);
                limit += ranSize;
                for ( int i = 0; i < ranSize; i++ )
                {
                    Vector2 newPosition = mimicRegen2.position + Random.insideUnitSphere * 5f;
                    Manager.Pool.GetPool(monsterMimic, newPosition, Quaternion.identity);

                }
            }
            else if ( limit >= 11 )
            {
                StopCoroutine("SpawnMimicsRoutine");
                Debug.Log("CheckStopCo");
            }
        }
    }
}
