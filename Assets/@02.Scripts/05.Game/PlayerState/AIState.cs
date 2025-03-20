using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : BasePlayerState
{
    public override void OnEnter(GameLogic gameLogic)
    {
        /*var result = MinimaxAIController.GetBestMove(gameLogic.GetBoard());
        if (result.HasValue)
        {
            HandleMove(gameLogic, result.Value.row, result.Value.col);
        }
        else
        {
            gameLogic.EndGame(Enums.EGameResult.Draw);
        }*/
    }

    public override void OnExit(GameLogic gameLogic)
    {
        
    }

    public override void HandleMove(GameLogic gameLogic, int x, int y)
    {
        //ProcessMove(gameLogic, Enums.EPlayerType.PlayerB, row, col);
    }
}
