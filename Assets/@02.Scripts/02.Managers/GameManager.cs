using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
    [SerializeField] private GameObject mScorePanel;
    [SerializeField] private GameObject mSelectProfileForProfilePanel;
    [SerializeField] private GameObject mSelectProfileForSignupPanel;
    [SerializeField] private GameObject mRankingPanel;
    [SerializeField] private List<Sprite> mProfileSprites;
    
    private Canvas mCanvas;

    private Enums.EGameType mGameType;

    // GamePanelController, GameLogic 구현
    private GamePanelController mGamePanelController;
    private GameLogic mGameLogic;

    // waitingPanel의 대기종료 여부(게임이 시작했는지)
    private bool mbIsStartGame = false;

    public Action OnMainPanelUpdate;
    public Action<Enums.EPlayerType> OnMyGameProfileUpdate;
    public Action<UsersInfoData> OnOpponentGameProfileUpdate;
    public Action OnCoinUpdated;
    public Action OnAdsRemoved;

    private void Start()
    {
        OpenSigninPanel();
    }
    
    #region  Score

    // TODO: 스코어 급수에 맞게 조정 현재는(-30~30)
    
    public async void WinGame()
    {
        // 서버에 wincount 증가 요청
        await NetworkManager.Instance.AddWinCount(
            successCallback: async () =>
            {
                // 성공 시 최신 사용자 정보 가져오기
                var userInfo = await NetworkManager.Instance.GetUserInfo(
                    successCallback: () => { Debug.Log("유저 정보 갱신 성공"); },
                    failureCallback: () => { Debug.LogWarning("유저 정보 갱신 실패"); }
                );

                // 승패 점수는 wincount - losecount, 추가로 등급과 승급 포인트도 가져옴
                int totalScore = userInfo.wincount - userInfo.losecount;
                // 수정: OpenScorePanel에 등급과 승급 포인트 정보를 추가로 전달
                OpenScorePanel(true, 1, totalScore, userInfo.rank, userInfo.rankuppoints);
            },
            failureCallback: () =>
            {
                Debug.LogWarning("승리 카운트 업데이트 실패");
            }
        );
    }

    public async void LoseGame()
    {
        // 서버에 losecount 증가 요청
        await NetworkManager.Instance.AddLoseCount(
            successCallback: async () =>
            {
                var userInfo = await NetworkManager.Instance.GetUserInfo(
                    successCallback: () => { Debug.Log("유저 정보 갱신 성공"); },
                    failureCallback: () => { Debug.LogWarning("유저 정보 갱신 실패"); }
                );

                int totalScore = userInfo.wincount - userInfo.losecount;
                // 수정: 등급과 승급 포인트도 전달
                OpenScorePanel(false, -1, totalScore, userInfo.rank, userInfo.rankuppoints);
            },
            failureCallback: () =>
            {
                Debug.LogWarning("패배 카운트 업데이트 실패");
            }
        );
    }
    /// <summary>
    /// 승점 패널 오픈
    /// </summary>
    /// <param name="isWin">승패</param>
    /// <param name="addDelete">점수획득 1/-1</param>
    public void OpenScorePanel(bool isWin, int addDelete, int totalScore, int rank, int rankuppoints)
    {
        if (mCanvas != null)
        {
            var scorePanelObject = Instantiate(mScorePanel, mCanvas.transform);
            var scoreController = scorePanelObject.GetComponent<ScorePanelController>();
            if (scoreController != null)
            {
                //  서버에서 가져온 총점, 승패, 증감값과 함께 등급, 승급 포인트를 전달
                scoreController.InitializePanel(totalScore, isWin, addDelete, rank, rankuppoints);
            }
        }
    }
    #endregion
    
    // 게임 화면으로 씬 전환하는 메서드
    public void ChangeToGameScene(Enums.EGameType gameType)
    {
        mGameType = gameType;
        SceneManager.LoadScene("Game");
    }

    // 메인 화면으로 씬 전환하는 메서드
    public void ChangeToMainScene()
    {
        ClearAllCallbacks();
        
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
            var rankingPanelObj = Instantiate(mRankingPanel, mCanvas.transform);
            rankingPanelObj.GetComponent<PopupPanelController>().Show();
        }
    }

    // 상점 패널 호출 메서드
    public void OpenShopPanel()
    {
        if (mCanvas != null)
        {
            var shopPanelObject = Instantiate(shopPanel, mCanvas.transform);
            shopPanelObject.GetComponent<PopupPanelController>().Show();
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
            signinPanelObj.GetComponent<SigninPanelController>().Show(OnMainPanelUpdate);
        }
    }

    // 회원가입 패널 호출 메서드
    public void OpenSignupPanel()
    {
        if (mCanvas != null)
        {
            var signupPanelObj = Instantiate(mSignupPanel, mCanvas.transform);
            signupPanelObj.GetComponent<PopupPanelController>().Show();
        }
    }

    public void OpenProfilePanel()
    {
        if (mCanvas != null)
        {
            var profilePanelObj = Instantiate(mProfilePanel, mCanvas.transform);
            profilePanelObj.GetComponent<PopupPanelController>().Show();
        }
    }

    // 프로필 패널에서 프로필 수정 시 호출
    public PopupPanelController OpenSelectProfilePanelFromProfilePanel()
    {
        if (mCanvas != null)
        {
            var selectProfilePanelObj = Instantiate(mSelectProfileForProfilePanel, mCanvas.transform);
            selectProfilePanelObj.GetComponent<PopupPanelController>().Show();

            return selectProfilePanelObj.GetComponent<PopupPanelController>();
        }

        Debug.Log("Canvas not open");
        return null;
    }

    // 회원가입 패널에서 프로필 수정 시 호출
    public PopupPanelController OpenSelectProfilePanelFromSignupPanel()
    {
        if (mCanvas != null)
        {
            var selectProfilePanelObj = Instantiate(mSelectProfileForSignupPanel, mCanvas.transform);
            selectProfilePanelObj.GetComponent<PopupPanelController>().Show();

            return selectProfilePanelObj.GetComponent<PopupPanelController>();
        }

        Debug.Log("Canvas not open");
        return null;
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
            mbIsStartGame = false;
            GameObject waitingPanelObject = Instantiate(waitingPanel, mCanvas.transform);
            waitingPanelObject.GetComponent<WaitingPanelController>().Show();
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
    
    // 콜백 초기화 메서드
    private void ClearAllCallbacks()
    {
        OnMainPanelUpdate = null;
        OnMyGameProfileUpdate = null;
        OnOpponentGameProfileUpdate = null;
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        mCanvas = GameObject.FindObjectOfType<Canvas>();
        
        // 인트로 BGM 재생
        if (scene.name == "Main")
        {
            AudioManager.Instance.PlayIntroBgm();
            
            MainPanelController mainPanelController = GameObject.FindObjectOfType<MainPanelController>();

            if (mainPanelController != null)
            {
                OnMainPanelUpdate -= mainPanelController.SetProfileInfo;
                OnMainPanelUpdate += mainPanelController.SetProfileInfo;
            }
            
            NetworkManager.Instance.GetUserInfo(() =>
            {
                OnMainPanelUpdate?.Invoke();
            }, () => { });
        }
        
        if (scene.name == "Game")
        {
            AudioManager.Instance.PlayGameBgm();

            // 씬에 배치된 오브젝트 찾기(BoardCellController, GamePanelController)
            BoardCellController boardCellController = GameObject.FindObjectOfType<BoardCellController>();
            GamePanelController gamePanelController = GameObject.FindObjectOfType<GamePanelController>();

            // BoardCellController 초기화
            boardCellController.InitBoard();
            
            if (gamePanelController != null)
            {
                // GamePanelController UI 초기화
                gamePanelController.SetGameUI(Enums.EGameUIState.Turn_Black);
                
                OnMyGameProfileUpdate -= gamePanelController.SetMyProfile;
                OnMyGameProfileUpdate += gamePanelController.SetMyProfile;
                
                OnOpponentGameProfileUpdate -= gamePanelController.SetOpponentProfile;
                OnOpponentGameProfileUpdate += gamePanelController.SetOpponentProfile;
            }

            // Game Logic 객체 생성
            if (mGameLogic != null)
            {
                mGameLogic.Dispose();
            }

            mGameLogic = new GameLogic();
            mGameLogic.GameStart(boardCellController, gamePanelController, mGameType, 
                OnMyGameProfileUpdate, OnOpponentGameProfileUpdate);
        }
    }

    private void OnApplicationQuit()
    {
        ClearAllCallbacks();
        
        mGameLogic?.Dispose();
        mGameLogic = null;
    }
}