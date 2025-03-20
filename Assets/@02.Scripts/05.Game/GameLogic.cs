using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GameLogic : IDisposable
{
    public BoardCellController boardCellController;
    public GamePanelController gamePanelController;

    private BasePlayerState mPlayer_Black;
    private BasePlayerState mPlayer_White;
    private BasePlayerState mCurrentPlayer;
    
    private MultiplayManager mMultiplayManager;
    private string mRoomId;
    
    /// <summary>
    /// 게임 시작 메서드
    /// </summary>
    /// <param name="boardCellController"></param>
    /// <param name="playMode"></param>
    public void GameStart(BoardCellController boardCellController, GamePanelController gamePanelController,Enums.EGameType playMode)
    {
        this.boardCellController = boardCellController;
        this.gamePanelController = gamePanelController;
        
        switch (playMode)
        {
            case Enums.EGameType.PassAndPlay:
                mPlayer_Black = new PlayerState(true);
                mPlayer_White = new PlayerState(false);
                
                SetState(mPlayer_Black);
                break;
            case Enums.EGameType.SinglePlay:
                mPlayer_Black = new PlayerState(true);
                // mPlayer_White = new AIState(false)

                SetState(mPlayer_Black);
                break;
            case Enums.EGameType.MultiPlay:
                mMultiplayManager = new MultiplayManager((state, roomId) =>
                {
                    mRoomId = roomId;
                    switch (state)
                    {
                        case Enums.EMultiplayManagerState.CreateRoom:
                            Debug.Log("## Create Room");
                            
                            // todo: 대기화면 표시(제한시간 동안 급수에 맞는 상대 매칭 실패 시 싱글 플레이로 모드 전환)
                            WaitingMatch();
                            break;
                        case Enums.EMultiplayManagerState.JoinRoom:
                            Debug.Log("## Join Room");
                            mPlayer_Black = new MultiplayerState(true, mMultiplayManager);
                            mPlayer_White = new PlayerState(false, mMultiplayManager, roomId);
                            
                            SetState(mPlayer_Black);
                            break;
                        case Enums.EMultiplayManagerState.StartGame:
                            Debug.Log("## Start Game");
                            CompleteMatch();
                            
                            mPlayer_Black = new PlayerState(true, mMultiplayManager, roomId);
                            mPlayer_White = new MultiplayerState(false, mMultiplayManager);
                            
                            SetState(mPlayer_Black);
                            break;
                        case Enums.EMultiplayManagerState.ExitRoom:
                            Debug.Log("## Exit Room");
                            // todo: 퇴장 처리
                            break;
                        case Enums.EMultiplayManagerState.EndGame:
                            Debug.Log("## End Game");
                            // todo: 게임 종료 처리
                            break;
                    }
                });
                break;
        }
    }
    
    /// <summary>
    /// 턴을 변경하는 메서드
    /// </summary>
    /// <param name="player"></param>
    public void NextTurn(Enums.EPlayerType player)
    {
        switch (player)
        {
            case Enums.EPlayerType.Player_Black:
                SetState(mPlayer_White);
                break;
            case Enums.EPlayerType.Player_White:
                SetState(mPlayer_Black);
                break;
        }
    }

    /// <summary>
    /// 게임 종료 처리를 해주는 메서드
    /// </summary>
    public void EndGame()
    {
        SetState(null);
        mPlayer_Black = null;
        mPlayer_White = null;
        
        gamePanelController.StopClock();
        gamePanelController.InitClock();
        
        // 점수 확인 패널 호출: 멀티플레이이거나 AI플레이일 경우 -> 승자 점수 확인, 패자 점수 확인
        if (mMultiplayManager != null /* && AI */)
        {
            GameManager.Instance.OpenScoreConfirmationPanel();
        }
        //점수 랭킹 업데이트
        //씬 혹은 게임화면 위치 변경
    }

    /// <summary>
    /// 현재 턴의 플레이어 상태(자신, AI, 멀티플레이어)를 변경하는 메서드
    /// </summary>
    /// <param name="newState"></param>
    public void SetState(BasePlayerState newState)
    {
        mCurrentPlayer?.OnExit(this);
        mCurrentPlayer = newState;
        mCurrentPlayer?.OnEnter(this);
        
        TurnUIUpdate();
        gamePanelController.StartClock();
    }

    private void WaitingMatch()
    {
        UnityThread.executeInUpdate(() =>
        {
            GameManager.Instance.OpenWaitingPanel();
        });
    }

    private void CompleteMatch()
    {
        UnityThread.executeInUpdate(() =>
        {
            GameManager.Instance.CloseWaitingPanel();
        });
    }
    
    /// <summary>
    /// 게임 진행 중일 때 Turn UI 표시
    /// </summary>
    private void TurnUIUpdate()
    {
        // 상태 변경 후 GamePanel의 Turn 표시 UI 업데이트
        if (mCurrentPlayer is PlayerState playerState)
        {
            if (playerState == mPlayer_Black)
            {
                gamePanelController.SetGameUI(Enums.EGameUIState.Turn_Black);
            }
            else
            {
                gamePanelController.SetGameUI(Enums.EGameUIState.Turn_White);
            }
        }
        /*else if (mCurrentPlayer id AIState)
        {
            gamePanelController.SetGameUI(Enums.EGameUIState.Turn_White);
        }*/
        else if (mCurrentPlayer is MultiplayerState multiplayerState)
        {
            if (multiplayerState == mPlayer_Black)
            {
                gamePanelController.SetGameUI(Enums.EGameUIState.Turn_Black);
            }
            else
            {
                gamePanelController.SetGameUI(Enums.EGameUIState.Turn_White);
            }
        }
    }
    
    /// <summary>
    /// (Y, X) 좌표에 해당 플레이어의 돌을 놓는 메서드
    /// </summary>
    /// <param name="playerType"></param>
    /// <param name="Y"></param>
    /// <param name="X"></param>
    /// <returns></returns>
    public bool SetStone(Enums.EPlayerType playerType, int Y, int X)
    {
        if (SetableStone(playerType, Y, X))
        {
            boardCellController.cells[Y, X].SetMark(playerType);
            boardCellController.cells[Y, X].playerType = playerType;
        }
        else
        {
            GameManager.Instance.OpenConfirmPanel("그 곳에 둘 수 없습니다.", null, false);
            return false;
        }

        return true;
    }

    /// <summary>
    /// 돌을 놓을 수 있는지 확인 여부를 반환하는 메서드
    /// 흑돌은 렌주룰 적용
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Y"></param>
    /// <param name="X"></param>
    /// <returns></returns>
    public bool SetableStone(Enums.EPlayerType player, int Y, int X)
    {
        if (boardCellController.cells[Y, X].playerType != Enums.EPlayerType.None) return false;
        
        if (player == Enums.EPlayerType.Player_White) return true;
        
        Enums.EPlayerType oppositePlayerType = Enums.EPlayerType.None;
        switch (player)
        {
            case Enums.EPlayerType.Player_Black:
                oppositePlayerType = Enums.EPlayerType.Player_White;
                break;
            case Enums.EPlayerType.Player_White:
                oppositePlayerType = Enums.EPlayerType.Player_Black;
                break;
        }
        
        #region 룰 리스트

        // 방향별로 BoardCell 배열을 생성하고 채우는 코드
        BoardCell[][] lists = MakeLists(boardCellController.size,Y,X);

        List<BoardCell>[] rule33Results = new List<BoardCell>[4];
        List<BoardCell>[] rule44Results = new List<BoardCell>[4];
        bool[] result44Bools = new bool[4];
        bool[] result6Bools = new bool[4];

        for (int i = 0; i < 4; i++)
        {
            (rule33Results[i], rule44Results[i], result44Bools[i],result6Bools[i]) = RenjuRule(lists[i], oppositePlayerType);
        }

        for (int i = 0; i < 4; i++)
        {
            if (!result44Bools[i] || !result6Bools[i]) return false;
            for (int k = i + 1; k < 4; k++)
            {
                if (rule33Results[i].Intersect(rule33Results[k]).Any())
                {
                    return false;
                }

                if (rule44Results[i].Intersect(rule44Results[k]).Any())
                {
                    return false;
                }
            }
        }
        #endregion
        
        return true;
    }

    //렌주룰 (33 44 룰)
    public (List<BoardCell>, List<BoardCell>, bool result44,bool rule6) RenjuRule(BoardCell[] list, Enums.EPlayerType opposite)
    {
        //33이 될 수 있는 최대길이는 4이다
        int rule33MaxLength = 4;
        int rule44MaxLenght = 5;

        List<BoardCell> rule33 = new List<BoardCell>();
        List<BoardCell> rule44 = new List<BoardCell>();
        List<BoardCell> rule33Result = new List<BoardCell>();
        List<BoardCell> rule44Result = new List<BoardCell>();

        bool rule6Result = true;

        //렌주룰에의해 한 줄에서 44가 안되는 경우도 있음
        bool result44 = true;

        #region 33Rule
        //백돌확인용 1, 놓는곳 위치의 왼쪽 흑돌 부터 확인
        for (int i = 1; i < 5; i++)
        {
            //왼쪽 끝 흰돌 확인
            if (list[i - 1]?.playerType == opposite || list[i - 1] == null)
            {
                continue;
            }

            //오른쪽 끝 흰돌 확인
            if (list[i + 4]?.playerType == opposite || (list[i + 4] == null))
            {
                continue;
            }

            int rule33Stone = 0;
            //한 줄에 3x3이 아닌 3x4 경우를 만들 수 있으므로 흑돌이 이미 3개일 경우 break
            bool rulebreak = false;
            
            //놓는곳으로부터 오른쪽으로 4칸 확인
            for (int t = 0; t < rule33MaxLength; t++)
            {
                if (list[i + t].playerType != opposite && rule33Stone < 3)
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
            
            if(rulebreak) break;
            rule33Stone = 0;
            rule33.Clear();
        }
        #endregion

        #region 44Rule
        for (int i = 0; i < 5; i++)
        {
            int rule44Stone = 0;
            for (int f = 0; f < rule44MaxLenght; f++)
            {
                if (list[i + f] == null) break;

                if (list[i + f].playerType != opposite && rule44Stone < 4)
                {
                    rule44.Add(list[i + f]);
                    if (list[i + f].playerType != Enums.EPlayerType.None)
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
                        if (cell.playerType == Enums.EPlayerType.None)
                        {
                            rule44Result.Add(cell);
                        }

                        //한 줄에도 44가 가능하기 때문에 아래 코드들로 확인
                        if (!result44 && rule44Result.Contains(cell))
                        {
                            return (rule33Result, rule44Result, result44, rule6Result);
                        }

                    }
                    result44 = false;
                    i = i + 2;
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
            for (int s = 0; s < 7; s++)
            {
                if (i + s >= list.Length || list[i + s] == null) continue;
                if (list[i + s]?.playerType != opposite && list[i + s]?.playerType != Enums.EPlayerType.None)
                {
                    playerStone++;
                    if (playerStone == 5)
                    {
                        rule6Result = false;
                        return (rule44Result, rule44Result, result44, rule6Result);
                    }
                }
            }
                playerStone = 0;
        }

        #endregion

        return (rule33Result, rule44Result, true, rule6Result);
    }

    /// <summary>
    /// 승리 조건을 확인하는 메서드
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Y"></param>
    /// <param name="X"></param>
    /// <returns></returns>
    public bool GameResult(Enums.EPlayerType player, int Y, int X)
    {
        BoardCell[][] lists = MakeLists(boardCellController.size, Y, X);

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

    /// <summary>
    /// (Y, X) 좌표를 기준으로 주위를 탐색하는 메서드
    /// </summary>
    /// <param name="boardSize"></param>
    /// <param name="Y"></param>
    /// <param name="X"></param>
    /// <returns></returns>
    public BoardCell[][] MakeLists(int boardSize,int Y, int X)
    {
        int endOfLeft = -4;
        int endOfRight = 5;
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
                lists[0][i] = boardCellController.cells[nextY1, nextX1];
            }

            // 리스트 1번: 왼쪽 -> 오른쪽 (X 좌표만 체크)
            if (nextX1 >= cellMin && nextX1 <= cellMax)
            {
                lists[1][i] = boardCellController.cells[Y, nextX1];
            }

            // 리스트 2번: 왼쪽 아래 -> 오른쪽 위 대각선 (범위 체크)
            if (nextX1 >= cellMin && nextX1 <= cellMax && nextY2 >= cellMin && nextY2 <= cellMax)
            {
                lists[2][i] = boardCellController.cells[nextY2, nextX1];
            }

            // 리스트 3번: 아래 -> 위 (Y 좌표만 체크)
            if (nextY2 >= cellMin && nextY2 <= cellMax)
            {
                lists[3][i] = boardCellController.cells[nextY2, X];
            }
        }

        return lists;
    }

    // 멀티 모드에서 룸 초기화하는 메서드
    public void Dispose()
    {
        mMultiplayManager?.LeaveRoom(mRoomId);
        mMultiplayManager?.Dispose();
    }
}
