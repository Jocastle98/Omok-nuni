using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
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

    /// <summary>
    /// 자신의 턴이 되었을 때 착수가능하게 버튼을 활성화
    /// </summary>
    /// <param name="gameLogic"></param>
    public override void OnEnter(GameLogic gameLogic)
    {
        //셀이 눌렸을 때 : 셀 선택
        gameLogic.boardCellController.onCellClicked = (cellIndex) =>
        {
            gameLogic.currentSelectedCell = cellIndex;
        };
        
        //착수 버튼을 눌렀을 때 : 선택된 셀에 착수
        gameLogic.gamePanelController.onBeginButtonClicked = () =>
        {
            int cellIndex = gameLogic.currentSelectedCell;
            if (cellIndex == null) return;
            
            int size = gameLogic.boardCellController.size;
            int X = cellIndex % (size + 1);
            int Y = cellIndex / (size + 1);
            
            HandleMove(gameLogic, Y, X);
        };
    }

    public override void OnExit(GameLogic gameLogic)
    {
        //델리게이트 초기화
        gameLogic.gamePanelController.onBeginButtonClicked = null;
        gameLogic.boardCellController.onCellClicked  = null;
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
