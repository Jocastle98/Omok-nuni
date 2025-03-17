using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayerState
{
    public Enums.EPlayerType playerType;

    public abstract void OnEnter(GameLogic gameLogic);
    public abstract void OnExit(GameLogic gameLogic);
    public abstract void HandleMove(GameLogic gameLogic,int x, int y);

    protected void ProcessMove(GameLogic gameLogic, Enums.EPlayerType player, int Y, int X)
    {
        if (gameLogic.SetStone(player, Y, X))
        {
            bool gameResult = gameLogic.GameResult(player, Y, X);
            if (gameResult)
            {
                gameLogic.EndGame();
            }
            else
            {
                gameLogic.NextTurn(player);
            }
        }
    }
}
