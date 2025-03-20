using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameLogic
{

    public Board board;

    private BasePlayerState player1;
    private BasePlayerState player2;
    private BasePlayerState currentPlayer;

    public void GameStart(Board board, Enums.EGameType playMode)
    {
        this.board = board;

        switch (playMode)
        {
            case Enums.EGameType.SinglePlay:
                player1 = new PlayerState(true);
                //AI

                SetState(player1);
                break;
            case Enums.EGameType.DualPlay:
                player1 = new PlayerState(true);
                player2 = new PlayerState(false);

                SetState(player1);
                break;
            case Enums.EGameType.MultiPlay:
                break;

        }
    }

    public void NextTurn(Enums.EPlayerType player)
    {
        switch (player)
        {
            case Enums.EPlayerType.PlayerA:
                SetState(player2);
                break;
            case Enums.EPlayerType.PlayerB:
                SetState(player1);
                break;
        }
    }

    public void EndGame()
    {
        SetState(null);
        player1 = null;
        player2 = null;
        
        //점수 랭킹 업데이트
        //씬 혹은 게임화면 위치 변경
    }

    public void SetState(BasePlayerState newState)
    {
        currentPlayer?.OnExit(this);
        currentPlayer = newState;
        currentPlayer?.OnEnter(this);
    }

    public bool SetStone(Enums.EPlayerType playerType, int Y, int X)
    {
        if (SetableStone(playerType, Y, X))
        {
            board.cells[Y, X].SetMark(playerType);
            board.cells[Y, X].playerType = playerType;
            
            BoardCell[][] lists = MakeLists(board.size,Y,X,4);
            
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < lists[i]?.Length; k++)
                {
                    //금수 최신화
                    if(lists[i][k] == null) continue;
                    int x = lists[i][k].blockIndex % (board.size + 1);
                    int y = lists[i][k].blockIndex / (board.size + 1);
                    CheckCellInRule(y,x);
                }
            }
        }
        else
        {
            Debug.Log("둘 수 없음");
            return false;
        }

        return true;
    }

    public bool SetableStone(Enums.EPlayerType player, int Y, int X)
    {
        if (board.cells[Y, X].playerType != Enums.EPlayerType.None) return false;
        
        if (player == Enums.EPlayerType.PlayerB) return true;
        
        //셀이 금수bool에 따라 리턴
        if(board.cells[Y,X].IsForbidden) return false;
        
        return true;
    }

    public void CheckCellInRule(int Y, int X)
    {
        int firstScanRange = 5;

        // 방향별로 BoardCell 배열을 생성하고 채우는 코드
        BoardCell[][] lists = MakeLists(board.size, Y, X, firstScanRange);

        List<BoardCell>[] rule33Results = new List<BoardCell>[4];
        List<BoardCell>[] rule44Results = new List<BoardCell>[4];
        BoardCell[] result44Cell = new BoardCell[4];
        BoardCell[] result6Bools = new BoardCell[4];

        for (int i = 0; i < 4; i++)
        {
            (rule33Results[i], rule44Results[i], result44Cell[i], result6Bools[i]) = RenjuRule(lists[i]);
        }


        for (int i = 0; i < 4; i++)
        {
            if (result6Bools[i] != null)
            {
                result6Bools[i].IsForbidden = true;
                return;
            }

            if (result44Cell[i] == board.cells[Y, X])
            {
                if (!FakeForbidden(result44Cell[i],lists[i],lists[i]))
                {
                    result44Cell[i].IsForbidden = true;
                    return;
                }
                else
                {
                    result44Cell[i].IsForbidden = false;
                }
            }

            for (int k = i + 1; k < 4; k++)
            {
                //금수 발생
                if (rule33Results[i].Intersect(rule33Results[k]).Any())
                {
                    var forbiddenList = rule33Results[i].Intersect(rule33Results[k]).ToList();
                    foreach (BoardCell cell in forbiddenList)
                    {
                        if (!FakeForbidden(cell,lists[i],lists[k]))
                        {
                            cell.IsForbidden = true;
                            return;
                        }
                        else
                        {
                            cell.IsForbidden = false;
                        }
                    }
                }

                //금수 발생
                if (rule44Results[i].Intersect(rule44Results[k]).Any())
                {
                    var forbiddenList = rule44Results[i].Intersect(rule44Results[k]).ToList();
                    foreach (BoardCell cell in forbiddenList)
                    {
                        if (!FakeForbidden(cell,lists[i],lists[k]))
                        {
                            cell.IsForbidden = true;
                            return;
                        }
                        else
                        {
                            cell.IsForbidden = false;
                        }
                    }
                }
            }
        }

        board.cells[Y, X].IsForbidden = false;
    }
    
    public bool ForbiddenSelf(BoardCell cell)
    {
        int X = cell.blockIndex % (board.size + 1);
        int Y = cell.blockIndex / (board.size + 1);
        return ForbiddenSelf(Y, X);
    }
    
    //자신의 좌표가 금수라면 false를 반환하는 목적으로 만든 함수
    public bool ForbiddenSelf(int Y, int X)
    {
        int firstScanRange = 5;
        
        // 방향별로 BoardCell 배열을 생성하고 채우는 코드
        BoardCell[][] lists = MakeLists(board.size,Y,X,firstScanRange);

        List<BoardCell>[] rule33Results = new List<BoardCell>[4];
        List<BoardCell>[] rule44Results = new List<BoardCell>[4];
        BoardCell[] result44Bools = new BoardCell[4];
        BoardCell[] result6Bools = new BoardCell[4];

        for (int i = 0; i < 4; i++)
        {
            (rule33Results[i], rule44Results[i], result44Bools[i],result6Bools[i]) = RenjuRule(lists[i]);
        }
        
        for (int i = 0; i < 4; i++)
        {
            if (result6Bools[i] == board.cells[Y, X])
            {
                return false;
            }

            if (result44Bools[i] == board.cells[Y, X])
            {
                return false;
            }
            
            //할당받은 좌표와
            //좌표를 통해 서치된 금수 배열들중 하나가 일치해야
            //첫번째 금수의 금수가됨, 따라서 첫번째 금수는 거짓금수가 될 수 있음
            for (int k = i + 1; k < 4; k++)
            {
                //금수 발생
                if (rule33Results[i].Intersect(rule33Results[k]).Any())
                {
                    var forbiddenList = rule33Results[i].Intersect(rule33Results[k]).ToList();
                    foreach (var cell in forbiddenList)
                    {
                        if (cell == board.cells[Y, X])
                        {
                            return false;
                        }
                    }
                }

                //금수 발생
                if (rule44Results[i].Intersect(rule44Results[k]).Any())
                {
                    var forbiddenList = rule44Results[i].Intersect(rule44Results[k]).ToList();
                    foreach (var cell in forbiddenList)
                    {
                        if (cell == board.cells[Y, X])
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    //거짓금수
    //금수가 될 위치 놓았을 때 금수가 되는 배열에 새로운 금수가 있다면 거짓금수
    //false == 금수 , true == 거짓금수
    public bool FakeForbidden(BoardCell cell, BoardCell[] firstList, BoardCell[] secondList)
    {
        //첫번째 금수가 되는 칸
        cell.playerType = Enums.EPlayerType.PlayerA;

        int scanRange = 2;
        
       
        for (int i = 0; i < firstList.Length; i++)
        {
            if (firstList[i] == cell)
            {
                for (int r = i - scanRange; r < i + scanRange + 1; r++)
                {
                    if (firstList[r]?.playerType == Enums.EPlayerType.None)
                    {
                        //두번째 금수가 될 위치가 이미 금수라면 거짓금수가 아님
                        if (firstList[r].IsForbidden == true)
                        {
                            cell.playerType = Enums.EPlayerType.None;
                            return false;
                        }
                        else if (!ForbiddenSelf(firstList[r]))
                        {
                            cell.playerType = Enums.EPlayerType.None;
                            return true;
                        }
                    }
                }
            }

            if (secondList[i] == cell)
            {
                for (int r = i - scanRange; r < i + scanRange + 1; r++)
                {
                    if (secondList[r]?.playerType == Enums.EPlayerType.None)
                    {
                        //두번째 금수가 될 위치가 이미 금수라면 거짓금수가 아님
                        if (secondList[r].IsForbidden == true)
                        {
                            cell.playerType = Enums.EPlayerType.None;
                            return false;
                        }
                        else if (!ForbiddenSelf(secondList[r]))
                        {
                            cell.playerType = Enums.EPlayerType.None;
                            return true;
                        }
                    }
                }
            }
        }

        cell.playerType = Enums.EPlayerType.None;


        return false;
    }

    //렌주룰 (33 44 룰)
    public (List<BoardCell>, List<BoardCell>, BoardCell result44, BoardCell rule6) RenjuRule(BoardCell[] list)
    {
        //33이 될 수 있는 최대길이는 4이다
        int rule33MaxLength = 4;
        int rule44MaxLenght = 5;

        List<BoardCell> rule33 = new List<BoardCell>();
        List<BoardCell> rule44 = new List<BoardCell>();
        List<BoardCell> rule33Result = new List<BoardCell>();
        List<BoardCell> rule44Result = new List<BoardCell>();

        BoardCell rule6Result = null;

        #region 33Rule
        
        for (int i = 2; i < 7; i++)
        {
            //예외 코드
            if (list[i - 1]?.playerType != Enums.EPlayerType.None || list[i - 1] == null)
            {
                continue;
            }
            
            if (list[i + rule33MaxLength]?.playerType != Enums.EPlayerType.None || (list[i + 4] == null))
            {
                continue;
            }

            if (list[i - 2]?.playerType == Enums.EPlayerType.PlayerA ||
                list[i + rule33MaxLength + 1]?.playerType == Enums.EPlayerType.PlayerA)
            {
                continue;
            }
            
            if (list[i - 2]?.playerType == Enums.EPlayerType.PlayerB &&
                list[i + rule33MaxLength + 1]?.playerType == Enums.EPlayerType.PlayerB)
            {
                continue;
            }

            int rule33Stone = 0;
            //한 줄에 3x3이 아닌 3x4 경우를 만들 수 있으므로 흑돌이 이미 3개일 경우 break
            bool rulebreak = false;

            //놓는곳으로부터 오른쪽으로 4칸 확인
            for (int t = 0; t < rule33MaxLength; t++)
            {
                if(list[i + t] == null) break;
                
                if (list[i + t].playerType != Enums.EPlayerType.PlayerB && rule33Stone < 3)
                {
                    rule33.Add(list[i + t]);
                    if (list[i + t] != null)
                    {
                        if (list[i + t].playerType != Enums.EPlayerType.None)
                        {
                            rule33Stone++;
                        }
                    }
                }
                else
                {
                    rule33.Clear();
                    rule33Stone = 0;
                    break;
                }

                //4칸 안에 흑돌이 두개라면 조건이 만족
                if (t == rule33MaxLength - 1 && rule33Stone == 2)
                {
                    foreach (BoardCell cell in rule33)
                    {
                        if (cell.playerType == Enums.EPlayerType.None && !rule33Result.Contains(cell))
                        {
                            rule33Result.Add(cell);
                        }
                    }
                }
                else if (2 < rule33Stone)
                {
                    rulebreak = true;
                    rule33Result.Clear();
                }
            }

            if (rulebreak) break;
            rule33Stone = 0;
            rule33.Clear();
        }

        #endregion

        #region 44Rule

        for (int i = 1; i <6; i++)
        {
            //예외 코드
            if (list[i - 1]?.playerType == Enums.EPlayerType.PlayerA ||
                list[i + rule44MaxLenght]?.playerType == Enums.EPlayerType.PlayerA) continue;

            if (list[i] == null) continue;
            
            int rule44Stone = 0;
            
            for (int f = 0; f < rule44MaxLenght; f++)
            {
                if(list[i + f] == null) break;
                
                if (list[i + f]?.playerType != Enums.EPlayerType.PlayerB && rule44Stone < 4)
                {
                    rule44.Add(list[i + f]);
                    if (list[i + f]?.playerType != Enums.EPlayerType.None)
                    {
                        rule44Stone++;
                    }
                }
                else
                {
                    rule44.Clear();
                    rule44Stone = 0;
                    break;
                }

                if (f == rule44MaxLenght - 1 && rule44Stone == 3)
                {

                    foreach (BoardCell cell in rule44)
                    {

                        //한 줄에도 44가 가능하기 때문에 아래 코드들로 확인
                        if (rule44Result.Contains(cell))
                        {
                            return (rule33Result, rule44Result, cell, rule6Result);
                        }

                        if (cell?.playerType == Enums.EPlayerType.None)
                        {
                            rule44Result.Add(cell);

                        }
                    }

                    i = i + 1;
                }
            }

            rule44Stone = 0;
            rule44.Clear();
        }

        #endregion

        #region 6Rule

        int playerStone = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int s = 0; s < 6; s++)
            {
                if (i + s >= list.Length || list[i + s] == null) continue;
                
                if (list[i + s]?.playerType != Enums.EPlayerType.PlayerB &&
                    list[i + s]?.playerType != Enums.EPlayerType.None)
                {
                    playerStone++;
                }
                else if (list[i + s]?.playerType == Enums.EPlayerType.None)
                {
                    rule6Result = list[i + s];
                }
                
                if (playerStone == 5 && rule6Result != null)
                {
                    return (rule33Result, rule44Result, null, rule6Result);
                }
            }

            rule6Result = null;
            playerStone = 0;
        }

        #endregion

        return (rule33Result, rule44Result, null, null);
    }

    public bool GameResult(Enums.EPlayerType player, int Y, int X)
    {
        BoardCell[][] lists = MakeLists(board.size, Y, X, 4);

        int counting = 0;
        for (int i = 0; i < lists.Length; i++)
        {
            for (int j = 0; j < lists[i].Length; j++)
            {
                if (lists[i][j]?.playerType == player)
                {
                    counting++;
                }
                else
                {
                    counting = 0;
                }

                if (counting == 5)
                {
                    Debug.Log($"{player} + win");
                    return true;
                }
            }
        }

        return false;
    }

    public BoardCell[][] MakeLists(int boardSize, BoardCell cell, int checkLength)
    {
        int X = cell.blockIndex % (boardSize + 1);
        int Y = cell.blockIndex / (boardSize + 1);
        return MakeLists(boardSize, Y, X, checkLength);
    }

    public BoardCell[][] MakeLists(int boardSize,int Y, int X,int checkLenght)
    {
        int endOfLeft = checkLenght * -1;
        int endOfRight = checkLenght + 1;
        int indexSize = Mathf.Abs(endOfRight) + Mathf.Abs(endOfLeft) + 1;
        
        BoardCell[][] lists = new BoardCell[4][];
        for (int j = 0; j < 4; j++)
            lists[j] = new BoardCell[indexSize];

        int cellMin = 0;
        int cellMax = boardSize;

        for (int i = 0; i < indexSize; i++)
        {
            int nextX1 = X + i + endOfLeft;
            int nextY1 = Y - i + endOfLeft * -1;
            int nextY2 = Y + i + endOfLeft;

            // 리스트 0번: 왼쪽 위 -> 오른쪽 아래 대각선 (범위 체크)
            if (nextX1 >= cellMin && nextX1 <= cellMax && nextY1 >= cellMin && nextY1 <= cellMax)
            {
                lists[0][i] = board.cells[nextY1, nextX1];
            }

            // 리스트 1번: 왼쪽 -> 오른쪽 (X 좌표만 체크)
            if (nextX1 >= cellMin && nextX1 <= cellMax)
            {
                lists[1][i] = board.cells[Y, nextX1];
            }

            // 리스트 2번: 왼쪽 아래 -> 오른쪽 위 대각선 (범위 체크)
            if (nextX1 >= cellMin && nextX1 <= cellMax && nextY2 >= cellMin && nextY2 <= cellMax)
            {
                lists[2][i] = board.cells[nextY2, nextX1];
            }

            // 리스트 3번: 아래 -> 위 (Y 좌표만 체크)
            if (nextY2 >= cellMin && nextY2 <= cellMax)
            {
                lists[3][i] = board.cells[nextY2, X];
            }
        }

        return lists;
    }
}
