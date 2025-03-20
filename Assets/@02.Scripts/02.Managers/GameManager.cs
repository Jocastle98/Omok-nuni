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
    [SerializeField] private GameObject mSignupPanel;
    [SerializeField] private GameObject mSigninPanel;


    private Canvas mCanvas;

    private Enums.EGameType mGameType;

    // GamePanelController, GameLogic 구현
    private GamePanelController mGamePanelController;
    private GameLogic mGameLogic;

    private void Start()
    {
        OpenSigninPanel();
    }

    public void ChangeToGameScene(Enums.EGameType gameType)
    {
        mGameType = gameType;
        //SceneManager.LoadScene("Game");

        // 임시기능: 테스트용
        SceneManager.LoadScene("ysw_Game");
    }

    public void ChangeToMainScene()
    {
        // gameLogic 초기화
        mGameLogic?.Dispose();
        mGameLogic = null;

        //SceneManager.LoadScene("Main");

        // 임시기능: 테스트용
        SceneManager.LoadScene("ysw_Main");
    }

    public void OpenGameTypeSelectPanel()
    {
        if (mCanvas != null)
        {
            GameObject gameTypeSelectPanelObject = Instantiate(gameTypeSelectPanel, mCanvas.transform);
            gameTypeSelectPanelObject.GetComponent<GameTypeSelectPanelController>().Show();
        }
    }

    public void OpenRecordPanel()
    {
        if (mCanvas != null)
        {
        }
    }

    public void OpenLeaderboardPanel()
    {
        if (mCanvas != null)
        {
        }
    }

    public void OpenShopPanel()
    {
        if (mCanvas != null)
        {
            GameObject shopPanelObject = Instantiate(shopPanel, mCanvas.transform);
            // shopPanelController 컴포넌트 연결 및 창 띄우는 메서드
        }
    }

    public void OpenSettingsPanel()
    {
        if (mCanvas != null)
        {
            var settingPanelObject = Instantiate(settingsPanel, mCanvas.transform);
            settingPanelObject.GetComponent<PopupPanelController>().Show();
        }
    }

    public void OpenConfirmPanel(string message, Action OnConfirmButtonClick, bool activeCancelButton = true)
    {
        if (mCanvas != null)
        {
            GameObject confirmPanelObject = Instantiate(confirmPanel, mCanvas.transform);
            confirmPanelObject.GetComponent<ConfirmPanelController>()
                .Show(message, OnConfirmButtonClick, activeCancelButton);
        }
    }

    public void OpenSigninPanel()
    {
        // TODO: 개별작업 씬 통합 시 삭제
        if (mSigninPanel == null)
        {
            return;
        }
        
        if (mCanvas != null)
        {
            var signinPanelObj = Instantiate(mSigninPanel, mCanvas.transform);
            signinPanelObj.GetComponent<PanelController>().Show();
        }
    }

    public void OpenSignupPanel()
    {
        if (mCanvas != null)
        {
            var signupPanelObj = Instantiate(mSignupPanel, mCanvas.transform);
            signupPanelObj.GetComponent<PanelController>().Show();
        }
    }

    public void OpenGameOverPanel()
    {
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