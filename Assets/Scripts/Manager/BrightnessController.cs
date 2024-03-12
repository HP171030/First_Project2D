using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BrightnessController : MonoBehaviour
{
    [SerializeField] PostProcessVolume postProcessVolume;

    private void OnEnable()
    {
        Manager.Game.brightUpdate += BrightControll;
    }
    private void BrightControll(float vol)
    {
        postProcessVolume.weight = Manager.Game.BrightnessVol;
        Debug.Log(postProcessVolume.weight);
    }

    private void OnDisable()
    {
        Manager.Game.brightUpdate -= BrightControll;    
    }

}
