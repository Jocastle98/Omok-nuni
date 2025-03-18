using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GamePanelController : MonoBehaviour
{
    [SerializeField] private GameObject timer;
    
    [SerializeField] private GameObject turnUI;
    [SerializeField] private GameObject recordUI;
    
    [SerializeField] private CanvasGroup blackTurnPanel;
    [SerializeField] private CanvasGroup whiteTurnPanel;

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
            case Enums.EGameUIState.Init:
                timer.SetActive(true);
                turnUI.SetActive(true);
                recordUI.SetActive(false);
                
                timer.GetComponent<Timer>().InitTimer();
                blackTurnPanel.alpha = mDisableAlpha;
                whiteTurnPanel.alpha = mDisableAlpha;
                break;
            case Enums.EGameUIState.Turn_Black:
                timer.SetActive(true);
                turnUI.SetActive(true);
                recordUI.SetActive(false);
                
                timer.GetComponent<Timer>().InitTimer();
                blackTurnPanel.alpha = mEnableAlpha;
                whiteTurnPanel.alpha = mDisableAlpha;
                break;
            case Enums.EGameUIState.Turn_White:
                timer.SetActive(true);
                turnUI.SetActive(true);
                recordUI.SetActive(false);

                timer.GetComponent<Timer>().InitTimer();
                blackTurnPanel.alpha = mDisableAlpha;
                whiteTurnPanel.alpha = mEnableAlpha;
                break;
            case Enums.EGameUIState.Record:
                timer.SetActive(false);
                turnUI.SetActive(false);
                recordUI.SetActive(true);
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
    
    #endregion
}
