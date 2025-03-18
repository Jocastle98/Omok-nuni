using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MinimaxAIController
{
    private const int BOARD_SIZE = 15;
    private static int SEARCH_RADIUS;
    private static int minimaxDepth;
    private static float mistakeChance;

    public static void SetLevel(Enums.EDifficultyLevel level)
    {
        switch (level)
        {
            case Enums.EDifficultyLevel.Easy:
                SEARCH_RADIUS = 1;
                minimaxDepth = 2;
                mistakeChance = 0.1f;
                break;
            case Enums.EDifficultyLevel.Medium:
                SEARCH_RADIUS = 1;
                minimaxDepth = 3;
                mistakeChance = 0.05f;
                break;
            case Enums.EDifficultyLevel.Hard:
                SEARCH_RADIUS = 1;
                minimaxDepth = 3;
                mistakeChance = 0.01f;
                break;
        }
    }

    public static (int row, int col)? GetBestMove(Enums.EPlayerType[,] board)
    {
        //난이도 설정
        /*Enums.EDifficultyLevel level = GameMmanger.GetLevelBasedOnRank();
        SetLevel(level);*/
        
        float bestScore = float.MinValue; //초기에 bestScore를 최저 점수로 설정
        (int row, int col) bestMove = (-1, -1); //bestMove는 (int, int)타입으로 선택되지 않을 값 -1, -1로 설정
        List<(int row, int col)> possibleMoves = GetPossibleMoves(board); //착수가 가능한 좌표의 리스트들을 모음

        if (possibleMoves.Count == 0) //만약 둘 수 있는 곳이 없다면 null을 리턴
            return null;

        
        if (Random.value < mistakeChance) //랜덤값이 실수확률에 해당한다면 착수 가능한 리스트에서 랜덤으로 한수를 착수
        {
            return possibleMoves[Random.Range(0, possibleMoves.Count)];
        }

        foreach (var (row, col) in possibleMoves)//착수 가능한 리스트를 순회하면서 한곳씩 수를 둠(현재 AI는 PlayerB=백돌 고정인 상황), DoMinimax를 진행하여 가장 높은 값과 해당 위치를 저장
        {
            board[row, col] = Enums.EPlayerType.PlayerB;
            float score = DoMinimax(board, 0, float.MinValue, float.MaxValue, false);
            board[row, col] = Enums.EPlayerType.None;

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = (row, col);
            }
        }

        return bestMove != (-1, -1) ? bestMove : null; // 초기값 -1, -1이 아니라면 해당 수를 반환, 맞다면 null값 반환
    }

    private static float DoMinimax(Enums.EPlayerType[,] board, int depth, float alpha, float beta, bool isMaximizing)
    {
        if (CheckGameWin(Enums.EPlayerType.PlayerB, board)) //PlayerB(AI)가 이기면 1000-depth 점수 반환
            return 1000 - depth;
        if (CheckGameWin(Enums.EPlayerType.PlayerA, board)) //PlayerA(사용자)가 이기면 -1000+depth 점수 반환
            return -1000 + depth;
        if (IsAllBlocksPlaced(board) || depth >= minimaxDepth) //모든 칸에 돌이 채워져 있거나, depth가 난이도에 따라 설정한 minimaxDepth보다 크거나 같으면 점수 반환
            return EvaluateBoard(board);

        float bestScore = isMaximizing ? float.MinValue : float.MaxValue; //최솟값, 최댓값을 선택하는지에 따라 bestScore 초기값을 설정
        List<(int row, int col)> possibleMoves = GetPossibleMoves(board); //착수가 가능한 위치를 포함하는 리스트 생성

        foreach (var (row, col) in possibleMoves) //착수가 가능한 위치들 중에서
        {
            board[row, col] = isMaximizing ? Enums.EPlayerType.PlayerB : Enums.EPlayerType.PlayerA; //최댓값을 구하는 거면 playerB(AI), 최솟값을 구하는 거면 PlayerA(player)
            float score = DoMinimax(board, depth + 1, alpha, beta, !isMaximizing); //재귀함수로 해당 타입을 두고 승패체크, 다시 확인
            board[row, col] = Enums.EPlayerType.None; //트리 끝까지 구하고 나서 해당 돌 타입을 None으로 설정

            if (isMaximizing)
            {
                bestScore = Mathf.Max(bestScore, score); //최댓값을 구하는거면 bestScore는 max값을 취함
                alpha = Mathf.Max(alpha, bestScore); //alpha값 선택
            }
            else
            {
                bestScore = Mathf.Min(bestScore, score); //최솟값을 구하는거면 bestScore에 min값을 취함
                beta = Mathf.Min(beta, bestScore); //beta값 선택
            }

            if (beta <= alpha) //알파값이 베타값보다 크거나 같으면 불필요한 가지 탐색을 줄이기 위해 반복문 탈출 (알파베타 가지치기)
                break;
        }

        return bestScore; //bestScore 반환
    }

    private static List<(int row, int col)> GetPossibleMoves(Enums.EPlayerType[,] board)
    {
        List<(int row, int col)> moves = new List<(int row, int col)>(); //리스트 생성

        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (board[row, col] != Enums.EPlayerType.None) //각 row, col을 돌면서 타입이 none이 아니라면
                {
                    for (int dr = -SEARCH_RADIUS; dr <= SEARCH_RADIUS; dr++) //해당 A든 B든 설정된 검색 radius -부터, +까지 row값
                    {
                        for (int dc = -SEARCH_RADIUS; dc <= SEARCH_RADIUS; dc++) //해당 A든 B든 설정된 검색 radius -부터, +까지 col값
                        {
                            int newRow = row + dr;
                            int newCol = col + dc;

                            if (newRow >= 0 && newRow < BOARD_SIZE && newCol >= 0 && newCol < BOARD_SIZE && 
                                board[newRow, newCol] == Enums.EPlayerType.None && !moves.Contains((newRow, newCol)))//0보다 작을 수는 없음, 보드 사이즈 15보다 클 수는 없음, 해당 위치는 none이여야함, 착수한 곳이 아니여야함 
                            {
                                moves.Add((newRow, newCol));//착수한곳에서 raius만큼 범위의 빈칸을 추가
                            }
                        }
                    }
                }
            }
        }

        return moves; //해당 범위의 착수가능한 리스트를 리턴
    }

    private static float EvaluateBoard(Enums.EPlayerType[,] board)
    {
        float score = 0;
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++) //row, col을 순회하면서
            {
                if (board[row, col] == Enums.EPlayerType.PlayerB) //백돌이면 스코어에 패턴 스코어를 더함
                    score += GetPatternScore(board, row, col, Enums.EPlayerType.PlayerB);
                else if (board[row, col] == Enums.EPlayerType.PlayerA)//흑돌이면 스코어에 패턴 스코어를 뺌
                    score -= GetPatternScore(board, row, col, Enums.EPlayerType.PlayerA);
            }
        }

        return score; //계산한 스코어를 반환
    }

    private static float GetPatternScore(Enums.EPlayerType[,] board, int row, int col, Enums.EPlayerType player)
    {
        float score = 0;
        int[] dx = { 1, 0, 1, -1 }; //4방향 검사를 위한 int 배열
        int[] dy = { 0, 1, 1, 1 }; //4방향 검사를 위한 int 배열 (오른쪽, 아래쪽, 오른아래, 왼아래)
                                   //=> 모든 반대 방향은 이미 다른 방향으로 탐색되므로 중복체크 필요x

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
                    if (board[newRow, newCol] == player) //현재 위치에서 이동하면서 같은 색의 돌을 셈
                        count++;
                    else if (board[newRow, newCol] == Enums.EPlayerType.None) //양쪽이 열려있는 것도 고려
                        openEnds++;
                    else
                        break;
                }
            }

            if (count == 5) score += 1000;
            else if (count == 4 && openEnds > 0) score += 100;
            else if (count == 3 && openEnds > 1) score += 10;
            
            // 4개 연속된 돌이 있는 경우
            // X X X X .   (openEnds = 2, count = 4) 
            
            // 한쪽이 막힌 4개 돌
            // O X X X X .   (openEnds = 1, count = 4)
            
            // 3개 연속된 돌
            // X X X .   (openEnds = 2, count = 3)
            
            // openEnds = 2이면 10점 부여
        }

        
        return score;
    }

    private static bool IsAllBlocksPlaced(Enums.EPlayerType[,] board)
    {
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (board[row, col] == Enums.EPlayerType.None) //모든 row, col을 순회하면서 빈칸인 경우가 있으면 false반환
                    return false;
            }
        }

        return true; //None인 곳이 없다면 true 반환
    }

    private static bool CheckGameWin(Enums.EPlayerType playerType, Enums.EPlayerType[,] board)
    {
        int[] dx = { 1, 0, 1, -1 };
        int[] dy = { 0, 1, 1, 1 };

        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if(board[row, col] != playerType) continue; //보드의 모든 칸을 확인하면서 해당 플레이어의 돌이 있는지 체크

                for (int d = 0; d < 4; d++)//4방향 검사
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

                    if (count == 5) return true; //5개 연속된 돌이 있으면 승리
                }
            }
        }

        return false;
    }
}
