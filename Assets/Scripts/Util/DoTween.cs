using UnityEngine;
using TMPro;
using DG.Tweening;

public static class DoTweenText
{
    public static void DoText( this TMP_Text a_text, float a_duration )
    {
       
        a_text.alpha = 0f;


        // ���̵� �� �ִϸ��̼�
        a_text.DOFade(1f, a_duration).SetEase(Ease.InQuad);
    }
}
