using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PlayerState : BasePlayerState
{
    public PlayerState(bool Black)
    {
        if (Black)
        {
            playerType = Enums.EPlayerType.PlayerA;
        }
        else
        {
            playerType = Enums.EPlayerType.PlayerB;
        }
    }

    public override void OnEnter(GameLogic gameLogic)
    {
        //수를 둔다
        gameLogic.board.onCellClicked = (Y, X) =>
        {
            HandleMove(gameLogic, Y, X);
        };
    }

    public override void OnExit(GameLogic gameLogic)
    {
        //델리게이트 초기화
        gameLogic.board.onCellClicked = null;
    }

    public override void HandleMove(GameLogic gameLogic,int Y, int X)
    {
        ProcessMove(gameLogic, playerType, Y, X);
    }
}
