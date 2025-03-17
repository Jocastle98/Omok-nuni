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
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private GameObject settingsPanel;
    
    private Canvas mCanvas;
    
    private Enums.EGameType mGameType;
    
    // GamePanelController, GameLogic 구현
    
    private GameLogic gameLogic;
    public Board board;
    
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
    
    public void OpenConfirmPanel(string message, Action OnConfirmButtonClick)
    {
        if (mCanvas != null)
        {
            GameObject confirmPanelObject = Instantiate(confirmPanel, mCanvas.transform);
            confirmPanelObject.GetComponent<ConfirmPanelController>().Show(message, OnConfirmButtonClick);
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
            

            // BoardCellController 초기화
            
            
            // GamePanelController UI 초기화
            
            
            // Game Logic 객체 생성
        }
        
        mCanvas = GameObject.FindObjectOfType<Canvas>();
    }

    private void OnApplicationQuit()
    {
        
    }
}
