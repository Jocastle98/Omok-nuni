using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TestBoardStart : MonoBehaviour
{
    GameLogic gameLogic;
    [FormerlySerializedAs("board")] public BoardCellController boardCellController;
    // Start is called before the first frame update
    void Start()
    {
        gameLogic = new GameLogic();
        gameLogic.GameStart(boardCellController,Enums.EGameType.PassAndPlay);
    }

}
