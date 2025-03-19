using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임의 전체적인 흐름을 관리하는 싱글톤 게임 매니저 클래스.
/// 씬 전환, UI 패널 관리, 게임 상태 관리 등의 역할을 수행.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    // [SerializeField] 각종 패널들 연결
    [SerializeField] private GameObject gameTypeSelectPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject watingPanel;

    
    private Canvas mCanvas;
    
    private Enums.EGameType mGameType;
    
    // GamePanelController, GameLogic 구현
    private GamePanelController mGamePanelController;
    private GameLogic mGameLogic;
    
    private void Start()
    {
        // 로그인 기능 구현?
        
    }
    
    // 게임 화면으로 씬 전환하는 메서드
    public void ChangeToGameScene(Enums.EGameType gameType)
    {
        mGameType = gameType;
        //SceneManager.LoadScene("Game");
        
        // 임시기능: 테스트용
        SceneManager.LoadScene("ysw_Game");
    }

    // 메인 화면으로 씬 전환하는 메서드
    public void ChangeToMainScene()
    {
        // gameLogic 초기화
        mGameLogic?.Dispose();
        mGameLogic = null;
        
        //SceneManager.LoadScene("Main");
        
        // 임시기능: 테스트용
        SceneManager.LoadScene("ysw_Main");
    }

    // 대국 시작 시 모드선택 패널 호출 메서드
    public void OpenGameTypeSelectPanel()
    {
        if (mCanvas != null)
        {
            GameObject gameTypeSelectPanelObject = Instantiate(gameTypeSelectPanel, mCanvas.transform);
            gameTypeSelectPanelObject.GetComponent<GameTypeSelectPanelController>().Show();
        }
    }
    
    // 내 기보(확인하기) 패널 호출 메서드
    public void OpenRecordPanel()
    {
        if (mCanvas != null)
        {
            
        }
    }
    
    // 랭킹(리더보드) 패널 호출 메서드
    public void OpenLeaderboardPanel()
    {
        if (mCanvas != null)
        {
            
        }
    }

    // 상점 패널 호출 메서드
    public void OpenShopPanel()
    {
        if (mCanvas != null)
        {
            GameObject shopPanelObject = Instantiate(shopPanel, mCanvas.transform);
            // shopPanelController 컴포넌트 연결 및 창 띄우는 메서드
        }
    }
    
    // 세팅 패널 호출 메서드
    public void OpenSettingsPanel()
    {
        if (mCanvas != null)
        {
            var settingPanelObject = Instantiate(settingsPanel, mCanvas.transform);
            settingPanelObject.GetComponent<PopupPanelController>().Show();
        }
    }
    
    // 확인(and 취소) 패널 호출 메서드
    public void OpenConfirmPanel(string message, Action OnConfirmButtonClick, bool activeCancelButton = true)
    {
        if (mCanvas != null)
        {
            GameObject confirmPanelObject = Instantiate(confirmPanel, mCanvas.transform);
            confirmPanelObject.GetComponent<ConfirmPanelController>().Show(message, OnConfirmButtonClick, activeCancelButton);
        }
    }
    
    // 로그인 패널 호출 메서드
    public void OpenSigninPanel()
    {
        if (mCanvas != null)
        {
            
        }
    }
    
    // 회원가입 패널 호출 메서드
    public void OpenSignupPanel()
    {
        if (mCanvas != null)
        {
            
        }
    }
    
    // 매칭 대기 패널 호출 메서드
    public void OpenWaitingPanel()
    {
        if (mCanvas != null)
        {
            GameObject waitingPanelObject = Instantiate(watingPanel, mCanvas.transform);
            waitingPanelObject.GetComponent<WaitingPanelController>().Show();
        }
    }

    public void CloseWaitingPanel()
    {
        if (mCanvas != null)
        {
            GameObject waitingPanelObject = Instantiate(watingPanel, mCanvas.transform);
            waitingPanelObject.GetComponent<WaitingPanelController>().OnClickCancelButton();
        }
    }
    
    // 승점 확인 패널 호출 메서드
    public void OpenScoreConfirmationPanel()
    {
        if (mCanvas != null)
        {
            
        }
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 인트로 BGM 재생
        if (scene.name == "ysw_Main")
        //if (scene.name == "Main")   
        {
            AudioManager.Instance.PlayIntroBgm();
        }
        
        // 임시기능: 테스트용
        if (scene.name == "ysw_Game")
        //if (scene.name == "Game")
        {
            AudioManager.Instance.PlayGameBgm();
            
            // 씬에 배치된 오브젝트 찾기(BoardCellController, GamePanelController)
            BoardCellController boardCellController = GameObject.FindObjectOfType<BoardCellController>();
            GamePanelController gamePanelController = GameObject.FindObjectOfType<GamePanelController>();

            // BoardCellController 초기화
            boardCellController.InitBoard();
            
            // GamePanelController UI 초기화
            gamePanelController.SetGameUI(Enums.EGameUIState.Init);
            
            // Game Logic 객체 생성
            if (mGameLogic != null)
            {
                mGameLogic.Dispose();
            }
            mGameLogic = new GameLogic();
            mGameLogic.GameStart(boardCellController, gamePanelController, mGameType);
        }
        
        mCanvas = GameObject.FindObjectOfType<Canvas>();
    }

    private void OnApplicationQuit()
    {
        mGameLogic?.Dispose();
        mGameLogic = null;
    }
}
