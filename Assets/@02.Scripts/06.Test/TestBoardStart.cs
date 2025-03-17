using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoardStart : MonoBehaviour
{
    GameLogic gameLogic;
    public Board board;
    // Start is called before the first frame update
    void Start()
    {
        gameLogic = new GameLogic();
        gameLogic.GameStart(board,Enums.EGameType.DualPlay);
    }

}
