using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class BrightnessControl : MonoBehaviour
{
    [SerializeField] Slider brightnessSlider;
    [SerializeField] PostProcessVolume ppVolume; 
    private AutoExposure autoExposureLayer; 

    private void OnEnable()
    {
        Camera mainCamera = Camera.main;
        if ( mainCamera != null )
        {
            ppVolume = mainCamera.GetComponent<PostProcessVolume>();
            if ( ppVolume != null && ppVolume.profile.TryGetSettings(out autoExposureLayer) )
            {
                brightnessSlider.value = ppVolume.weight;
                Manager.Game.BrightnessVol = brightnessSlider.value;    
               
            }
      
    }
        }

    public void AdjustBrightness()
    {
        if ( autoExposureLayer != null && ppVolume != null )
        {
            Manager.Game.BrightnessVol = brightnessSlider.value;
            
        }
    }
}
