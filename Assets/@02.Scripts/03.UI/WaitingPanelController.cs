using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPanelController : PopupPanelController
{
    // 남은 시간을 표시할 스크롤바와 텍스트
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private float mProgressDuration = 30.0f;    // 매칭 제한시간
    
    private Coroutine mProgressCoroutine;
    
    public void Show()
    {
        base.Show();
        
        StartProgressBar();
    }

    public void Hide()
    {
        StopProgressBar();
        
        base.Hide();
    }
    
    public void OnClickCancelButton()
    {
        StopProgressBar();
        
        Hide();
    }
    
    private void StartProgressBar()
    {
        if (mProgressCoroutine != null)
        {
            StopCoroutine(mProgressCoroutine);
        }

        mProgressCoroutine = StartCoroutine(FillProgressBar());
    }

    private void StopProgressBar()
    {
        if (mProgressCoroutine != null)
        {
            StopCoroutine(mProgressCoroutine);
            mProgressCoroutine = null;
        }
        
        progressBar.value = 0.0f;
        progressText.text = $"{mProgressDuration}초";
    }

    private IEnumerator FillProgressBar()
    {
        float time = 0.0f;
        float duration = mProgressDuration;

        while (time < duration)
        {
            time += Time.deltaTime;
            progressBar.value = time / duration;
            
            // 남은시간 표시
            float remainingTime = duration - time;
            progressText.text = string.Format("{0:0}초", remainingTime);

            if (GameManager.Instance.GetIsStartGame())
            {
                this.Hide();
            }
            
            yield return null;
        }
        
        // 제한시간이 지나면
        OnMatchingTimeout();
    }
    
    private void OnMatchingTimeout()
    {
        Hide();
        
        GameManager.Instance.OpenConfirmPanel("다른 유저와의 매칭이 실패하였습니다.", () =>
        {
            // todo: AI와 매칭 기능 구현
            GameManager.Instance.ChangeToGameScene(Enums.EGameType.SinglePlay);
        }, false);
    }
}
