using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : BasePlayerState
{
    public AIState(bool Black)
    {
        if (Black)
        {
            playerType = Enums.EPlayerType.Player_Black;
        }
        else
        {
            playerType = Enums.EPlayerType.Player_White;
        }
    }
    
    public override void OnEnter(GameLogic gameLogic)
    {
        var result = MinimaxAIController.GetBestMove(gameLogic.GetBoard());
        if (result.HasValue)
        {
            HandleMove(gameLogic, result.Value.row, result.Value.col);
        }
        else
        {
            gameLogic.EndGame();
        }
    }

    public override void OnExit(GameLogic gameLogic)
    {
        
    }

    public override void HandleMove(GameLogic gameLogic, int y, int x)
    {
        ProcessMove(gameLogic, playerType, y, x);
    }
}
