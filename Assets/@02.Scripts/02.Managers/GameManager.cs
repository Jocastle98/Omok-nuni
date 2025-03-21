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
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject mSignupPanel;
    [SerializeField] private GameObject mSigninPanel;
    [SerializeField] private GameObject mProfilePanel;
    [SerializeField] private GameObject mSelectProfilePanel;
    [SerializeField] private GameObject mRankingPanel;
    [SerializeField] private List<Sprite> mProfileSprites;
    
    
    private Canvas mCanvas;

    private Enums.EGameType mGameType;

    // GamePanelController, GameLogic 구현
    private GamePanelController mGamePanelController;
    private GameLogic mGameLogic;

    // waitingPanel의 대기종료 여부(게임이 시작했는지)
    private bool mbIsStartGame = false;

    private void Start()
    {
        OpenSigninPanel();
    }

    // 게임 화면으로 씬 전환하는 메서드
    public void ChangeToGameScene(Enums.EGameType gameType)
    {
        mGameType = gameType;
        SceneManager.LoadScene("Game");
    }

    // 메인 화면으로 씬 전환하는 메서드
    public void ChangeToMainScene()
    {
        // gameLogic 초기화
        mGameLogic?.Dispose();
        mGameLogic = null;

        SceneManager.LoadScene("Main");
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
            confirmPanelObject.GetComponent<ConfirmPanelController>()
                .Show(message, OnConfirmButtonClick, activeCancelButton);
        }
    }

    // 로그인 패널 호출 메서드
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

    // 회원가입 패널 호출 메서드
    public void OpenSignupPanel()
    {
        if (mCanvas != null)
        {
            var signupPanelObj = Instantiate(mSignupPanel, mCanvas.transform);
            signupPanelObj.GetComponent<PanelController>().Show();
        }
    }

    public void OpenProfilePanel()
    {
        if (mCanvas != null)
        {
            var profilePanelObj = Instantiate(mProfilePanel, mCanvas.transform);
            profilePanelObj.GetComponent<PanelController>().Show();
        }
    }

    public PanelController OpenSelectProfilePanel()
    {
        if (mCanvas != null)
        {
            var selectProfilePanelObj = Instantiate(mSelectProfilePanel, mCanvas.transform);
            selectProfilePanelObj.GetComponent<PanelController>().Show();

            return selectProfilePanelObj.GetComponent<PanelController>();
        }

        Debug.Log("Canvas not open");
        return null;
    }

    public void OpenRankingPanel()
    {
        if (mCanvas != null)
        {
            var rankingPanelObj = Instantiate(mRankingPanel, mCanvas.transform);
            rankingPanelObj.GetComponent<PanelController>().Show();
        }
    }

    public Sprite GetProfileSprite(int profileIndex)
    {
        if (profileIndex >= 0 && profileIndex < mProfileSprites.Count)
        {
            return mProfileSprites[profileIndex];
        }

        Debug.Log("out of index in ProfileSprites");
        return null;
    }

    // 매칭 대기 패널 호출 메서드
    public void OpenWaitingPanel()
    {
        if (mCanvas != null)
        {
            GameObject waitingPanelObject = Instantiate(waitingPanel, mCanvas.transform);
            waitingPanelObject.GetComponent<WaitingPanelController>().Show();
            mbIsStartGame = false;
        }
    }

    // waitingPanel의 종료 여부(게임 시작)를 waitingPanel로 전달(반환)해주는 메서드
    public bool GetIsStartGame()
    {
        return mbIsStartGame;
    }

    // GameLogic에서 StartGame 여부를 설정(Set)해주는 메서드
    public void SetIsStartGame(bool isStartGame)
    {
        mbIsStartGame = isStartGame;
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
        if (scene.name == "Main")
        {
            AudioManager.Instance.PlayIntroBgm();
        }

        // 임시기능: 테스트용
        if (scene.name == "Game")
        {
            AudioManager.Instance.PlayGameBgm();

            // 씬에 배치된 오브젝트 찾기(BoardCellController, GamePanelController)
            BoardCellController boardCellController = GameObject.FindObjectOfType<BoardCellController>();
            GamePanelController gamePanelController = GameObject.FindObjectOfType<GamePanelController>();

            // BoardCellController 초기화
            boardCellController.InitBoard();

            // GamePanelController UI 초기화
            gamePanelController.SetGameUI(Enums.EGameUIState.Turn_Black);

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