using UnityEngine;
using TMPro;
using DG.Tweening;

public static class DoTweenText
{
    public static void DoText( this TMP_Text a_text, float a_duration )
    {
       
        a_text.alpha = 0f;


        // 페이드 인 애니메이션
        a_text.DOFade(1f, a_duration).SetEase(Ease.InQuad);
    }
}
