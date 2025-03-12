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
    
    private Canvas mcanvas;
    
    private Enum.EGameType mgameType;
    
    // GamePanelController, GameLogic 구현
    
    private void Start()
    {
        // 로그인 기능 구현?
    }
    
    public void ChangeToGameScene(Enum.EGameType gameType)
    {
        mgameType = gameType;
        SceneManager.LoadScene("Game");
    }

    public void ChangeToMainScene()
    {
        // gameLogic 초기화 추가
        
        SceneManager.LoadScene("Main");
    }

    public void OpenRecordPanel()
    {
        if (mcanvas != null)
        {
            
        }
    }
    
    public void OpenLeaderboardPanel()
    {
        if (mcanvas != null)
        {
            
        }
    }

    public void OpenShopPanel()
    {
        if (mcanvas != null)
        {
            
        }
    }
    
    public void OpenSettingsPanel()
    {
        if (mcanvas != null)
        {
            
        }
    }
    
    public void OpenConfirmPanel()
    {
        if (mcanvas != null)
        {
            
        }
    }
    
    public void OpenSigninPanel()
    {
        if (mcanvas != null)
        {
            
        }
    }
    
    public void OpenSignupPanel()
    {
        if (mcanvas != null)
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
        
        mcanvas = GameObject.FindObjectOfType<Canvas>();
    }

    private void OnApplicationQuit()
    {
        
    }
}
