using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // [SerializeField] 패널 연결
    
    private Canvas mcanvas;
    
    private Contants.EGameType mgameType;
    
    // GamePanelController, GameLogic 구현
    
    private void Start()
    {
        
    }

    public void ChangeToGameScene(Contants.EGameType gameType)
    {
        mgameType = gameType;
        SceneManager.LoadScene("Game");
    }

    public void ChangeToMainScene()
    {
        // gameLogic 초기화
        SceneManager.LoadScene("Main");
    }

    public void OpenLeaderboardPanel()
    {
        
    }
    
    public void OpenSettingsPanel()
    {
        
    }
    
    public void OpenConfirmPanel()
    {
        
    }
    
    public void OpenSigninPanel()
    {
        
    }
    
    public void OpenSignupPanel()
    {
        
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
