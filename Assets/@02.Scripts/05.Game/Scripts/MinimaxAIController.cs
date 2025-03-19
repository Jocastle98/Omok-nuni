using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public static class MinimaxAIController
{
    private const int BOARD_SIZE = 15;
    private static int SEARCH_RADIUS;
    private static int minimaxDepth;
    private static float mistakeChance;
    private static int MOVE_CONSIDERATION_LIMIT; // 고려할 수 있는 위치 제한 
    private static float PATTERN_AWARENESS; // 패턴 인식 능력

    // 보드 상태와 평가 점수를 저장하는 딕셔너리
    private static Dictionary<string, float> transpositionTable = new Dictionary<string, float>();
        
    // 시간 제한 설정
    private static float timeLimit = 1.0f;
    
    public static void SetLevel(Enums.EDifficultyLevel level)
    {
        // 난이도에 따른 매개변수 설정
        switch (level)
        {
            case Enums.EDifficultyLevel.Easy:
                SEARCH_RADIUS = 1;              //착수된 모든 돌에서 몇칸 이내를 볼건지
                minimaxDepth = 2;               //minimax의 깊이 조정(몇수 앞을 볼건지)
                mistakeChance = 0.15f;          //AI가 실수할 확률
                MOVE_CONSIDERATION_LIMIT = 5;   //고려할 수 있는 위치 제한
                PATTERN_AWARENESS = 0.5f;       //패턴 인식 능력(승리/위협 패턴 무시 확률)
                break;
            case Enums.EDifficultyLevel.Medium:
                SEARCH_RADIUS = 2;
                minimaxDepth = 3;
                mistakeChance = 0.05f;
                MOVE_CONSIDERATION_LIMIT = 10;
                PATTERN_AWARENESS = 0.8f;
                break;
            case Enums.EDifficultyLevel.Hard:
                SEARCH_RADIUS = 2;
                minimaxDepth = 4;
                mistakeChance = 0.01f;
                MOVE_CONSIDERATION_LIMIT = 15;
                PATTERN_AWARENESS = 1.0f;
                break;
        }
        
        // 새 게임 시작 시 캐시 초기화
        transpositionTable.Clear();
    }

    public static (int row, int col)? GetBestMove(Enums.EPlayerType[,] board)
    {
        //난이도 설정
        /*Enums.EDifficultyLevel level = GameMmanger.GetLevelBasedOnRank();
        SetLevel(level);*/
        
        // 반복 심화 탐색을 위한 변수들
        (int row, int col) bestMove = (-1, -1);
        float startTime = Time.realtimeSinceStartup;
        
        // 실수 확률 적용
        if (Random.value < mistakeChance)
        {
            List<(int row, int col)> possibleMoves = GetPossibleMoves(board);
            if (possibleMoves.Count == 0)
                return null;
            return possibleMoves[UnityEngine.Random.Range(0, possibleMoves.Count)];
        }
        
        for (int depth = 1; depth <= minimaxDepth; depth++) //depth 1부터 점진적으로 증가
        {
            (int newRow, int newCol) = FindBestMoveAtDepth(board, depth); //해당 깊이에서 가장 최적의 수를 찾음
            
            // 시간이 남았으면 결과 업데이트
            if (Time.realtimeSinceStartup - startTime < timeLimit)
            {
                bestMove = (newRow, newCol);
            }
            else //시간이 초과되었을시
            {
                Debug.Log($"AI 탐색 완료: 깊이 {depth-1}까지 탐색했습니다.");
                break;
            }
        }

        return bestMove != (-1, -1) ? bestMove : null; // 초기값 -1, -1이 아니라면 해당 수를 반환, 맞다면 null값 반환
    }

    private static (int row, int col) FindBestMoveAtDepth(Enums.EPlayerType[,] board, int depth)
    {
        float bestScore = float.MinValue;
        (int row, int col) bestMove = (-1, -1);
        
        // 가능한 수들 중 유망한 수들만 고려
        List<(int row, int col)> possibleMoves = GetPrioritizedMoves(board);
        
        if (possibleMoves.Count == 0)
            return (-1, -1);
            
        foreach (var (row, col) in possibleMoves)
        {
            board[row, col] = Enums.EPlayerType.PlayerB;
            float score = DoMinimax(board, 0, depth, float.MinValue, float.MaxValue, false);
            board[row, col] = Enums.EPlayerType.None;

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = (row, col);
            }
        }
        
        return bestMove;
    }
    
    private static float DoMinimax(Enums.EPlayerType[,] board, int currentDepth, int maxDepth, float alpha, float beta, bool isMaximizing)
    {
        if (CheckGameWin(Enums.EPlayerType.PlayerB, board)) //PlayerB(AI)가 이기면 1000-depth 점수 반환
            return 1000 - currentDepth;
        if (CheckGameWin(Enums.EPlayerType.PlayerA, board)) //PlayerA(사용자)가 이기면 -1000+depth 점수 반환
            return -1000 + currentDepth;
        if (IsAllBlocksPlaced(board) || currentDepth >= minimaxDepth) //모든 칸에 돌이 채워져 있거나, depth가 난이도에 따라 설정한 minimaxDepth보다 크거나 같으면 점수 반환
            return EvaluateBoard(board);

        // 같은 보드 상태를 여러 번 평가하지 않도록 캐싱
        string boardHash = GetBoardHash(board, isMaximizing);
        if (transpositionTable.ContainsKey(boardHash))
        {
            return transpositionTable[boardHash];
        }
        
        float bestScore = isMaximizing ? float.MinValue : float.MaxValue; //최솟값, 최댓값을 선택하는지에 따라 bestScore 초기값을 설정
        List<(int row, int col)> possibleMoves = GetPossibleMoves(board); //착수가 가능한 위치를 포함하는 리스트 생성

        foreach (var (row, col) in possibleMoves)   //착수가 가능한 위치들 중에서
        {
            board[row, col] = isMaximizing ? Enums.EPlayerType.PlayerB : Enums.EPlayerType.PlayerA; //최댓값을 구하는 거면 playerB(AI), 최솟값을 구하는 거면 PlayerA(player)
            float score = DoMinimax(board, currentDepth + 1, maxDepth, alpha, beta, !isMaximizing);     //재귀함수로 해당 타입을 두고 승패체크, 다시 확인
            board[row, col] = Enums.EPlayerType.None;    //해당 돌 타입을 None으로 다시 설정

            if (isMaximizing)
            {
                bestScore = Mathf.Max(bestScore, score);
                alpha = Mathf.Max(alpha, bestScore);
            }
            else
            {
                bestScore = Mathf.Min(bestScore, score);
                beta = Mathf.Min(beta, bestScore);
            }

            if (beta <= alpha)
                break;
        }

        // 트랜스포지션 테이블에 결과 저장
        transpositionTable[boardHash] = bestScore;
        return bestScore;
    }

    private static string GetBoardHash(Enums.EPlayerType[,] board, bool isMaximizing)
    {
        StringBuilder hash = new StringBuilder();
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                hash.Append((int)board[row, col]);
            }
        }
        hash.Append(isMaximizing ? "1" : "0");
        return hash.ToString();
    }
    
    // 가능한 모든 수를 찾아 점수로 정렬하여 반환
    private static List<(int row, int col)> GetPrioritizedMoves(Enums.EPlayerType[,] board)
    {
        List<(int row, int col, float score)> scoredMoves = new List<(int row, int col, float score)>();
        
        // 기존 코드와 동일하게 가능한 위치 찾기
        for (int row = 0; row < BOARD_SIZE; row++)
        {
            for (int col = 0; col < BOARD_SIZE; col++)
            {
                if (board[row, col] != Enums.EPlayerType.None) 
                {
                    for (int dr = -SEARCH_RADIUS; dr <= SEARCH_RADIUS; dr++)
                    {
                        for (int dc = -SEARCH_RADIUS; dc <= SEARCH_RADIUS; dc++)
                        {
                            int newRow = row + dr;
                            int newCol = col + dc;

                            if (newRow >= 0 && newRow < BOARD_SIZE && newCol >= 0 && newCol < BOARD_SIZE && 
                                board[newRow, newCol] == Enums.EPlayerType.None)
                            {
                                // 이미 목록에 있는지 확인
                                bool alreadyAdded = scoredMoves.Any(m => m.row == newRow && m.col == newCol);
                                if (!alreadyAdded)
                                {
                                    // 간단한 휴리스틱 점수 계산
                                    float score = SimpleEvaluate(board, newRow, newCol);
                                    scoredMoves.Add((newRow, newCol, score));
                                }
                            }
                        }
                    }
                }
            }
        }
        
        // 점수에 따라 정렬하고 상위 N개만 선택
        int movesToConsider = Mathf.Min(MOVE_CONSIDERATION_LIMIT, scoredMoves.Count);
        return scoredMoves.OrderByDescending(m => m.score)
            .Take(movesToConsider)
            .Select(m => (m.row, m.col))
            .ToList();
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

    // 위치 평가를 위한 간단한 휴리스틱
    private static float SimpleEvaluate(Enums.EPlayerType[,] board, int row, int col)
    {
        float score = 0;
        
        // 공격 점수 - AI 돌 놓을 경우
        board[row, col] = Enums.EPlayerType.PlayerB;
        score += GetPatternScore(board, row, col, Enums.EPlayerType.PlayerB) * 1.1f; // 공격 점수는 1.1배 가중치
        
        // 방어 점수 - 상대 돌 놓을 경우
        board[row, col] = Enums.EPlayerType.PlayerA;
        score += GetPatternScore(board, row, col, Enums.EPlayerType.PlayerA);
        
        // 원상복구
        board[row, col] = Enums.EPlayerType.None;
        
        // 보드 중앙에 가까울수록 약간의 추가 점수
        int centerDist = Mathf.Abs(row - BOARD_SIZE / 2) + Mathf.Abs(col - BOARD_SIZE / 2);
        score += (BOARD_SIZE - centerDist) * 0.1f;
        
        return score;
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
        int[] dx = { 1, 0, 1, -1 };
        int[] dy = { 0, 1, 1, 1 };

        for (int d = 0; d < 4; d++)
        {
            // 패턴 문자열 구성
            StringBuilder pattern = new StringBuilder();
            List<Enums.EPlayerType> linePattern = new List<Enums.EPlayerType>();
            
            for (int i = -4; i <= 4; i++)
            {
                int newRow = row + i * dx[d];
                int newCol = col + i * dy[d];
                
                if (newRow >= 0 && newRow < BOARD_SIZE && newCol >= 0 && newCol < BOARD_SIZE)
                {
                    linePattern.Add(board[newRow, newCol]);
                    if (board[newRow, newCol] == player)
                        pattern.Append('X');
                    else if (board[newRow, newCol] == Enums.EPlayerType.None)
                        pattern.Append('.');
                    else
                        pattern.Append('O');
                }
            }
            
            string patternStr = pattern.ToString();
            
            // 난이도에 따른 패턴 인식 제한
            if (Random.value > PATTERN_AWARENESS)
            {
                // 낮은 난이도에서는 일부 패턴을 무시함
                continue;
            }
            
            // 패턴 점수 계산
            if (patternStr.Contains("XXXXX")) score += 1000;  // 5개 연속
            else if (patternStr.Contains(".XXXX.")) score += 200; // 양쪽 열린 4
            else if (patternStr.Contains("XXXX.") || patternStr.Contains(".XXXX")) score += 100; // 한쪽 열린 4
            else if (patternStr.Contains(".XXX.")) score += 50; // 양쪽 열린 3
            else if (patternStr.Contains("XXX..") || patternStr.Contains("..XXX")) score += 10; // 한쪽 열린 3
            else if (patternStr.Contains("XX...") || patternStr.Contains("...XX")) score += 5; // 두 개 연속
            else if (patternStr.Contains(".XX..") || patternStr.Contains("..XX.")) score += 8; // 양쪽 열린 2
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
