using System.Collections;
using UnityEngine;

public abstract class BaseScene : MonoBehaviour
{
    public abstract IEnumerator LoadingRoutine();

    public virtual IEnumerator OnStartScene() { yield return null; }
}
