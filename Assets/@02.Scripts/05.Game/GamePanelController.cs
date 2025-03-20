using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UserDataStructs;

public class GamePanelController : MonoBehaviour
{
    [SerializeField] private GameObject timer;
    
    [SerializeField] private GameObject turnUI;
    [SerializeField] private GameObject resignButton;
    [SerializeField] private GameObject recordUI;
    [SerializeField] private GameObject backButton;
    
    [SerializeField] private CanvasGroup blackTurnPanel;
    [SerializeField] private CanvasGroup whiteTurnPanel;

    [SerializeField] private Image playerBlackProfileImage;
    [SerializeField] private TMP_Text playerBlackProfileText;
    [SerializeField] private Image playerWhiteProfileImage;
    [SerializeField] private TMP_Text playerWhiteProfileText;
    
    private const float mDisableAlpha = 0.3f;
    private const float mEnableAlpha = 1.0f;
    
    /// <summary>
    /// Play하고 있을 때는 턴을 표시하는 turnUI를 보여주고
    /// Record(기보)하고 있을 때는 recordUI를 보여줌
    /// </summary>
    /// <param name="gameUIState"></param>
    public void SetGameUI(Enums.EGameUIState gameUIState)
    {
        switch (gameUIState)
        {
            case Enums.EGameUIState.Turn_Black:
                timer.SetActive(true);
                turnUI.SetActive(true);
                resignButton.SetActive(true);
                recordUI.SetActive(false);
                backButton.SetActive(false);
                
                timer.GetComponent<Timer>().InitTimer();
                blackTurnPanel.alpha = mEnableAlpha;
                whiteTurnPanel.alpha = mDisableAlpha;
                break;
            case Enums.EGameUIState.Turn_White:
                timer.SetActive(true);
                turnUI.SetActive(true);
                resignButton.SetActive(true);
                recordUI.SetActive(false);
                backButton.SetActive(false);

                timer.GetComponent<Timer>().InitTimer();
                blackTurnPanel.alpha = mDisableAlpha;
                whiteTurnPanel.alpha = mEnableAlpha;
                break;
            case Enums.EGameUIState.Record:
                timer.SetActive(false);
                turnUI.SetActive(false);
                resignButton.SetActive(false);
                recordUI.SetActive(true);
                backButton.SetActive(true);
                break;
        }
    }
    
    

    public void InitClock()
    {
        timer.GetComponent<Timer>().InitTimer();
    }
    
    public void StartClock()
    {
        timer.GetComponent<Timer>().StartTimer();
    }

    public void StopClock()
    {
        timer.GetComponent<Timer>().PauseTimer();
    }
    
    /// <summary>
    /// 기권버튼 클릭 시 패배 처리 호출하는 메서드
    /// 확인 팝업 후 확인 시 패배처리
    /// </summary>
    public void OnClickResignButton()
    {
        GameManager.Instance.OpenConfirmPanel("기권하시겠습니까?", () =>
        {
            // 패배처리
            
            
            // 임시기능: 메인 화면으로 전환(패배처리 구현 시 삭제)
            GameManager.Instance.ChangeToMainScene();
        });
    }

    /// <summary>
    /// 착수 버튼 클릭 시 착수 처리하는 메서드
    /// </summary>
    public void OnClickBeginButton()
    {
        
    }

    #region Record UI(기보 기능 UI)
    
    // 맨 첫 턴으로 이동
    public void OnClickFirstMoveButton()
    {
        
    }

    // 이전 턴으로 이동
    public void OnClickPreviousMoveButton()
    {
        
    }

    // 다음 턴으로 이동
    public void OnClickNextMoveButton()
    {
        
    }

    // 맨 마지막 턴으로 이동
    public void OnClickLastMoveButton()
    {
        
    }

    // 뒤로 가기 버튼
    public void OnClickBackButton()
    {
        GameManager.Instance.OpenConfirmPanel("내 기보를 종료하시겠습니까?", () =>
        {
            // 내 기보 목록 팝업 창으로 돌아갈지, 그냥 메인화면 자체로 돌아갈지?(일단은 메인화면)
            GameManager.Instance.ChangeToMainScene();
        });
    }
    
    #endregion
}
