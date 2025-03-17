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
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject confirmPanel;
    
    private Canvas mCanvas;
    
    private Enums.EGameType mGameType;
    
    // GamePanelController, GameLogic 구현
    
    private GameLogic mGameLogic;
    
    private void Start()
    {
        // 로그인 기능 구현?
    }
    
    public void ChangeToGameScene(Enums.EGameType gameType)
    {
        mGameType = gameType;
        SceneManager.LoadScene("Game");
    }

    public void ChangeToMainScene()
    {
        // gameLogic 초기화 추가
        
        SceneManager.LoadScene("Main");
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
            
        }
    }
    
    public void OpenConfirmPanel(string message, Action OnConfirmButtonClick, bool activeCancelButton = true)
    {
        if (mCanvas != null)
        {
            GameObject confirmPanelObject = Instantiate(confirmPanel, mCanvas.transform);
            confirmPanelObject.GetComponent<ConfirmPanelController>().Show(message, OnConfirmButtonClick, activeCancelButton);
        }
    }
    
    public void OpenSigninPanel()
    {
        if (mCanvas != null)
        {
            
        }
    }
    
    public void OpenSignupPanel()
    {
        if (mCanvas != null)
        {
            
        }
    }
    
    public void OpenGameOverPanel()
    {
        
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            // 씬에 배치된 오브젝트 찾기(BoardCellController, GamePanelController)
            Board board = GameObject.FindObjectOfType<Board>();
            

            // BoardCellController 초기화
            board.InitBoard();
            
            // GamePanelController UI 초기화
            
            
            // Game Logic 객체 생성
            if (mGameLogic != null)
            {
                mGameLogic.Dispose();
            }
            mGameLogic = new GameLogic();
            mGameLogic.GameStart(board, mGameType);
        }
        
        mCanvas = GameObject.FindObjectOfType<Canvas>();
    }

    private void OnApplicationQuit()
    {
        mGameLogic?.Dispose();
        mGameLogic = null;
    }
}
