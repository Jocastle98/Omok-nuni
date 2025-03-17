using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MinimaxAIController
{
    /*private const int BOARD_SIZE = 15;
    private static int SEARCH_RADIUS;
    private static float mistakeChance;

    public static void SetLevel(Enums.ELevel level)
    {
        switch (level)
        {
            case Enums.ELevel.Easy:
                SEARCH_RADIUS = 1;
                mistakeChance = 0.3f;
                break;
            case Enums.ELevel.Medium:
                SEARCH_RADIUS = 2;
                mistakeChance = 0.2f;
                break;
            case Enums.ELevel.Hard:
                SEARCH_RADIUS = 4;
                mistakeChance = 0.1f;
                break;
        }
    }

    public static (int row, int col)? GetBestMove(Constants.PlayerType[,] board)
    {
        Enums.ELevel level = GameMmanger.GetLevelBasedOnRank();
        SetLevel(level);
        
        float bestScore = float.MinValue;
        (int row, int col) bestMove = (-1, -1);
        List<(int row, int col)> possibleMoves = GetPossibleMoves(board);

        if (possibleMoves.Count == 0)
            return null;

        
        if (Random.value < mistakeChance)
        {
            return possibleMoves[Random.Range(0, possibleMoves.Count)];
        }

        foreach (var (row, col) in possibleMoves)
        {
            board[row, col] = Constants.PlayerType.PlayerB;
            float score = DoMinimax(board, 0, float.MinValue, float.MaxValue, false);
            board[row, col] = Constants.PlayerType.None;

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = (row, col);
            }
        }

        return bestMove != (-1, -1) ? bestMove : null;
    }

    private static float DoMinimax(Constants.PlayerType[,] board, int depth, float alpha, float beta, bool isMaximizing)
    {
        if (CheckGameWin(Constants.PlayerType.PlayerB, board))
            return 1000 - depth;
        if (CheckGameWin(Constants.PlayerType.PlayerA, board))
            return -1000 + depth;
        if (IsAllBlocksPlaced(board) || depth >= 4)
            return EvaluateBoard(board);

        float bestScore = isMaximizing ? float.MinValue : float.MaxValue;
        List<(int row, int col)> possibleMoves = GetPossibleMoves(board);

        foreach (var (row, col) in possibleMoves)
        {
            board[row, col] = isMaximizing ? Constants.PlayerType.PlayerB : Constants.PlayerType.PlayerA;
            float score = DoMinimax(board, depth + 1, alpha, beta, !isMaximizing);
            board[row, col] = Constants.PlayerType.None;

            if (isMaximizing)
            {
                bestScore = Math.Max(bestScore, score);
                alpha = Math.Max(alpha, bestScore);
            }
            else
            {
                bestScore = Math.Min(bestScore, score);
                beta = Math.Min(beta, bestScore);
            }

            if (beta <= alpha)
                break;
        }

        return bestScore;
    }

    private static List<(int row, int col)> GetPossibleMoves(Constants.PlayerType[,] board)
    {
        List<(int row, int col)> moves = new List<(int row, int col)>();

        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (board[row, col] != Constants.PlayerType.None)
                {
                    for (int dr = -SEARCH_RADIUS; dr <= SEARCH_RADIUS; dr++)
                    {
                        for (int dc = -SEARCH_RADIUS; dc <= SEARCH_RADIUS; dc++)
                        {
                            int newRow = row + dr;
                            int newCol = col + dc;

                            if (newRow >= 0 && newRow < BOARD_SIZE && newCol >= 0 && newCol < BOARD_SIZE &&
                                board[newRow, newCol] == Constants.PlayerType.None && !moves.Contains((newRow, newCol)))
                            {
                                moves.Add((newRow, newCol));
                            }
                        }
                    }
                }
            }
        }

        return moves;
    }

    private static float EvaluateBoard(Constants.PlayerType[,] board)
    {
        float score = 0;
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (board[row, col] == Constants.PlayerType.PlayerB)
                    score += GetPatternScore(board, row, col, Constants.PlayerType.PlayerB);
                else if (board[row, col] == Constants.PlayerType.PlayerA)
                    score -= GetPatternScore(board, row, col, Constants.PlayerType.PlayerA);
            }
        }

        return score;
    }

    private static float GetPatternScore(Constants.PlayerType[,] board, int row, int col, Constants.PlayerType player)
    {
        float score = 0;
        int[] dx = { 1, 0, 1, -1 };
        int[] dy = { 0, 1, 1, 1 };

        for (int d = 0; d < 4; d++)
        {
            int count = 0;
            int openEnds = 0;

            for (int i = -4; i <= 4; i++)
            {
                int newRow = row + i * dx[d];
                int newCol = col + i * dy[d];

                if (newRow >= 0 && newRow < BOARD_SIZE && newCol >= 0 && newCol < BOARD_SIZE)
                {
                    if (board[newRow, newCol] == player)
                        count++;
                    else if (board[newRow, newCol] == Constants.PlayerType.None)
                        openEnds++;
                    else
                        break;
                }
            }

            if (count == 5) score += 1000;
            else if (count == 4 && openEnds > 0) score += 100;
            else if (count == 3 && openEnds > 1) score += 10;
        }

        return score;
    }

    private static bool IsAllBlocksPlaced(Constants.PlayerType[,] board)
    {
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (board[row, col] == Constants.PlayerType.None)
                    return false;
            }
        }

        return true;
    }

    private static bool CheckGameWin(Constants.PlayerType playerType, Constants.PlayerType[,] board)
    {
        int[] dx = { 1, 0, 1, -1 };
        int[] dy = { 0, 1, 1, 1 };

        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if(board[row, col] != playerType) continue;

                for (int d = 0; d < 4; d++)
                {
                    int count = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        int newRow = row + i * dx[d];
                        int newCol = col + i * dy[d];

                        if (newRow >= 0 && newRow < BOARD_SIZE && newCol >= 0 && newCol < BOARD_SIZE &&
                            board[newRow, newCol] == playerType)
                        {
                            count++;
                        }
                    }

                    if (count == 5) return true;
                }
            }
        }

        return false;
    }*/
}
