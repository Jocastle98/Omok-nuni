using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : BasePlayerState
{
    private MultiplayManager mMultiplayManager;
    private string mRoomId;
    private bool mbIsMultiplay;
    
    public PlayerState(bool Black)
    {
        if (Black)
        {
            playerType = Enums.EPlayerType.Player_Black;
        }
        else
        {
            playerType = Enums.EPlayerType.Player_White;
        }
        
        mbIsMultiplay = false;
    }

    public PlayerState(bool Black, MultiplayManager multiplayManager, string roomId) : this(Black)
    {
        mMultiplayManager = multiplayManager;
        mRoomId = roomId;
        mbIsMultiplay = true;
    }

    public override void OnEnter(GameLogic gameLogic)
    {
        //수를 둔다
        gameLogic.boardCellController.onCellClicked = (Y, X) =>
        {
            HandleMove(gameLogic, Y, X);
        };
    }

    public override void OnExit(GameLogic gameLogic)
    {
        //델리게이트 초기화
        gameLogic.boardCellController.onCellClicked = null;
    }

    public override void HandleMove(GameLogic gameLogic, int Y, int X)
    {
        ProcessMove(gameLogic, playerType, Y, X);

        if (mbIsMultiplay)
        {
            mMultiplayManager.SendPlayerMove(mRoomId, Y *  15 + X);
        }
    }
}
