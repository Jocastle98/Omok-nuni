using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TestBoardStart : MonoBehaviour
{
    GameLogic gameLogic;
    public BoardCellController boardCellController;
    public GamePanelController gamePanelController;
    
    // Start is called before the first frame update
    void Start()
    {
        gameLogic = new GameLogic();
        gameLogic.GameStart(boardCellController,gamePanelController, Enums.EGameType.PassAndPlay);
    }

}
