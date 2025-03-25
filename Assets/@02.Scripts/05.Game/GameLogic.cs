using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UserDataStructs;

public class GameLogic : IDisposable
{
    public BoardCellController boardCellController;
    public GamePanelController gamePanelController;
    
    public int currentSelectedCell = Int32.MaxValue;
    
    private BasePlayerState mPlayer_Black;
    private BasePlayerState mPlayer_White;
    private BasePlayerState mCurrentPlayer;
    
    private MultiplayManager mMultiplayManager;
    private string mRoomId;
    
    //승점 패널
    public Enums.EPlayerType localPlayerType = Enums.EPlayerType.Player_Black;
    public bool isGameOver = false;
    private Action<Enums.EPlayerType> OnMyGameProfileUpdate;
    private Action<UsersInfoData> OnOpponentGameProfileUpdate;
    
    /// <summary>
    /// 게임 시작 메서드
    /// </summary>
    /// <param name="boardCellController"></param>
    /// <param name="playMode"></param>
    public void GameStart(BoardCellController boardCellController, GamePanelController gamePanelController, Enums.EGameType playMode, 
        Action<Enums.EPlayerType> onMyGameProfileUpdate, Action<UsersInfoData> onOpponentGameProfileUpdate)
    {
        this.boardCellController = boardCellController;
        this.gamePanelController = gamePanelController;
        OnMyGameProfileUpdate = onMyGameProfileUpdate;
        OnOpponentGameProfileUpdate = onOpponentGameProfileUpdate;
            
        switch (playMode)
        {
            case Enums.EGameType.PassAndPlay:
                mPlayer_Black = new PlayerState(true);
                mPlayer_White = new PlayerState(false);
                
                OnMyGameProfileUpdate?.Invoke(Enums.EPlayerType.Player_Black);
                SetState(mPlayer_Black);
                break;
            case Enums.EGameType.SinglePlay:
                mPlayer_Black = new PlayerState(true);
                mPlayer_White = new AIState(false);

                OnMyGameProfileUpdate?.Invoke(Enums.EPlayerType.Player_Black);
                SetState(mPlayer_Black);
                break;
            case Enums.EGameType.MultiPlay:
                mMultiplayManager =  new MultiplayManager((state, roomId) =>
                {
                    mMultiplayManager.OnOpponentProfileUpdate += OnOpponentGameProfileUpdate;
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
                            mPlayer_White = new PlayerState(false, mMultiplayManager, mRoomId);
                            
                            // 방들어온 플레이어는 백
                            localPlayerType = mPlayer_White.playerType; 
                            MyGameProfileUpdate(Enums.EPlayerType.Player_White);
                            SendOpponentGameProfile(mRoomId, Enums.EPlayerType.Player_White);
                            SetState(mPlayer_Black);
                            break;
                        case Enums.EMultiplayManagerState.StartGame:
                            Debug.Log("## Start Game");
                            GameManager.Instance.SetIsStartGame(true);
                            
                            mPlayer_Black = new PlayerState(true, mMultiplayManager, mRoomId);
                            mPlayer_White = new MultiplayerState(false, mMultiplayManager);
                            
                            // 첫 수 두는 플레이어 흑
                            localPlayerType = mPlayer_Black.playerType;
                            MyGameProfileUpdate(Enums.EPlayerType.Player_Black);
                            SendOpponentGameProfile(mRoomId, Enums.EPlayerType.Player_Black);
                            SetState(mPlayer_Black);
                            break;
                        case Enums.EMultiplayManagerState.ExitRoom:
                            // todo: 퇴장 처리
                            break;
                        case Enums.EMultiplayManagerState.EndGame:
                            // todo: 게임 종료 처리
                            break;
                    }
                });
                break;
        }
    }

    /// <summary>
    /// 턴을 변경하면 메서드
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

        if (mCurrentPlayer is AIState aiPlayer)
        {
            aiPlayer.OnEnter(this);
        }
    }

    /// <summary>
    /// 게임 종료 처리를 해주는 메서드
    /// </summary>
    public void EndGame(Enums.EPlayerType winnerType)
    {
        if (isGameOver) return; 
        isGameOver = true;
        SetState(null);
        mPlayer_Black = null;
        mPlayer_White = null;
        
        gamePanelController.StopClock();
        gamePanelController.InitClock();
        
        if (winnerType == Enums.EPlayerType.None)
        {
            // 무승부
            Debug.Log("무승부!");
        }
        else if (winnerType == localPlayerType)
        {
            // 내가 이긴 경우
            Debug.Log("내가 승리했습니다!");
            GameManager.Instance.WinGame();
        }
        else
        {
            // 상대가 이긴 경우 => 나는 패배
            Debug.Log("상대가 승리");
            GameManager.Instance.LoseGame();
        }

        // 점수 확인 패널 호출: 멀티플레이이거나 AI플레이일 경우 -> 승자 점수 확인, 패자 점수 확인
        if (mMultiplayManager != null /* && AI */)
        {
            GameManager.Instance.OpenScoreConfirmationPanel();
        }
        //점수 랭킹 업데이트
        //씬 혹은 게임화면 위치 변경
        
        //TODO: 게임 진행 로직 완성되면 ScorePanel을 띄워 승점 관리
        
        
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
    /// 매칭 대기 시간을 알려주는 팝업창을 호출하는 메서드
    /// </summary>
    private void WaitingMatch()
    {
        UnityThread.executeInUpdate(() =>
        {
            GameManager.Instance.OpenWaitingPanel();
        });
    }
    
    private void MyGameProfileUpdate(Enums.EPlayerType playerType)
    {
        UnityThread.executeInUpdate(() =>
        {
            OnMyGameProfileUpdate?.Invoke(playerType);
        });
    }

    private void SendOpponentGameProfile(string roomId, Enums.EPlayerType playerType)
    {
        UnityThread.executeInUpdate(() =>
        {
            mMultiplayManager.SendOpponentProfile(roomId, SetMyUserInfo(roomId, playerType));
        });
    }

    private UsersInfoData SetMyUserInfo(string roomId, Enums.EPlayerType playerType)
    {
        // 네트워크에서 실제 사용자 정보를 받아옵니다
        UserInfoResult userInfo = NetworkManager.Instance.GetUserInfoSync(() => { }, () => { });

        // 실제 데이터를 기반으로 사용자 정보를 설정
        UsersInfoData usersInfoData = new UsersInfoData
        {
            roomId = roomId,
            nickname = userInfo.nickname,
            profileimageindex = userInfo.profileimageindex,
            rank = userInfo.rank,
            playerType = playerType
        };

        return usersInfoData;
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
            
            BoardCell[][] lists = MakeLists(boardCellController.size,Y,X,4);
            
            for (int i = 0; i < 4; i++)
            {
                for (int k = 0; k < lists[i]?.Length; k++)
                {
                    //금수 최신화
                    if(lists[i][k] == null) continue;
                    int x = lists[i][k].cellIndex % (boardCellController.size + 1);
                    int y = lists[i][k].cellIndex / (boardCellController.size + 1);
                    CheckCellInRule(y,x);
                }
            }
        }
        else
        {
            GameManager.Instance.OpenConfirmPanel("그 곳에 둘 수 없습니다.", null, false);
            return false;
        }
        
        //승점 패널
        bool isWin = GameResult(playerType, Y, X);
        if (isWin)
        {
            // 승리 플레이어 엔드게임
            EndGame(playerType);
        }
        else
        {
            // 5목 아니면 다음 턴
            NextTurn(playerType);
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
    
        //셀이 금수bool에 따라 리턴
        if(boardCellController.cells[Y,X].IsForbidden) return false;
        
        return true;
    }
    
    /// <summary>
    /// X,Y좌표를 기준으로 firstScanRange * 2 의 길이 만큼 4방향으로 금수가 있는지 확인하는 메서드 
    /// </summary>
    /// <param name="Y"></param>
    /// <param name="X"></param>
    public void CheckCellInRule(int Y, int X)
    {
        int firstScanRange = 5;

        // 방향별로 BoardCell 배열을 생성하고 채우는 코드
        BoardCell[][] lists = MakeLists(boardCellController.size, Y, X, firstScanRange);

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
                result6Bools[i].OnForbbiden(true, mPlayer_Black);
                return;
            }

            if (result44Cell[i] == boardCellController.cells[Y, X])
            {
                if (!FakeForbidden(result44Cell[i],lists[i],lists[i]))
                {
                    result44Cell[i].OnForbbiden(true, mPlayer_Black);
                    return;
                }
                else
                {
                    result44Cell[i].OnForbbiden(false, mPlayer_Black);
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
                            cell.OnForbbiden(true, mPlayer_Black);
                            return;
                        }
                        else
                        {
                            cell.OnForbbiden(false, mPlayer_Black);
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
                            cell.OnForbbiden(true, mPlayer_Black);
                            return;
                        }
                        else
                        {
                            cell.OnForbbiden(false, mPlayer_Black);
                        }
                    }
                }
            }
        }

        boardCellController.cells[Y, X].OnForbbiden(false, mPlayer_Black);
    }
    
    public bool ForbiddenSelf(BoardCell cell)
    {
        int X = cell.cellIndex % (boardCellController.size + 1);
        int Y = cell.cellIndex / (boardCellController.size + 1);
        return ForbiddenSelf(Y, X);
    }
    
    /// <summary>
    /// 자신의 위치가 금수인지 확인하는 메서드
    /// 금수라면 false, 금수가 아니라면 true
    /// </summary>
    /// <param name="Y"></param>
    /// <param name="X"></param>
    /// <returns></returns>
    public bool ForbiddenSelf(int Y, int X)
    {
        int firstScanRange = 5;
        
        // 방향별로 BoardCell 배열을 생성하고 채우는 코드
        BoardCell[][] lists = MakeLists(boardCellController.size,Y,X,firstScanRange);

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
            if (result6Bools[i] == boardCellController.cells[Y, X])
            {
                return false;
            }

            if (result44Bools[i] == boardCellController.cells[Y, X])
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
                        if (cell == boardCellController.cells[Y, X])
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
                        if (cell == boardCellController.cells[Y, X])
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    
    /// <summary>
    ///거짓금수를 확인하는 메서드
    ///금수가 될 위치 놓았을 때 자신의 위치에서, 자신이 금수간 된 배열의 2칸 이내에 새로운 금수가 있다면 거짓금수
    ///false == 금수 , true == 거짓금수
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="firstList"></param>
    /// <param name="secondList"></param>
    /// <returns></returns>
    public bool FakeForbidden(BoardCell cell, BoardCell[] firstList, BoardCell[] secondList)
    {
        //첫번째 금수가 되는 칸
        cell.playerType = Enums.EPlayerType.Player_Black;

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

    /// <summary>
    /// 렌주룰
    /// 매개변수로 받은 배열의 모든 칸에 금수 체크를 하고 리스트를 반환하는 메서드
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
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

            if (list[i - 2]?.playerType == Enums.EPlayerType.Player_Black ||
                list[i + rule33MaxLength + 1]?.playerType == Enums.EPlayerType.Player_Black)
            {
                continue;
            }
            
            if (list[i - 2]?.playerType == Enums.EPlayerType.Player_White &&
                list[i + rule33MaxLength + 1]?.playerType == Enums.EPlayerType.Player_White)
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
                
                if (list[i + t].playerType != Enums.EPlayerType.Player_White && rule33Stone < 3)
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
            if (list[i - 1]?.playerType == Enums.EPlayerType.Player_Black ||
                list[i + rule44MaxLenght]?.playerType == Enums.EPlayerType.Player_Black) continue;

            if (list[i] == null) continue;
            
            int rule44Stone = 0;
            
            for (int f = 0; f < rule44MaxLenght; f++)
            {
                if(list[i + f] == null) break;
                
                if (list[i + f]?.playerType != Enums.EPlayerType.Player_White && rule44Stone < 4)
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
                
                if (list[i + s]?.playerType != Enums.EPlayerType.Player_White &&
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

    /// <summary>
    /// 승리 조건을 확인하는 메서드
    /// </summary>
    /// <param name="player"></param>
    /// <param name="Y"></param>
    /// <param name="X"></param>
    /// <returns></returns>
    public bool GameResult(Enums.EPlayerType player, int Y, int X)
    {

        BoardCell[][] lists = MakeLists(boardCellController.size, Y, X, 4);

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
    /// 받은 좌표를 중심으로 전방향으로 리스트를 만들고 반환하는 메서드
    /// </summary>
    /// <param name="boardSize"></param>
    /// <param name="Y"></param>
    /// <param name="X"></param>
    /// <param name="checkLenght"></param>
    /// <returns></returns>
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

    public Enums.EPlayerType[,] GetBoard()
    {
        int size = boardCellController.size;
        Enums.EPlayerType[,] board = new Enums.EPlayerType[size, size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                board[y, x] = boardCellController.cells[y, x].playerType;
            }
        }

        return board;
    }

    // 멀티 모드에서 룸 초기화하는 메서드
    public void Dispose()
    {
        mMultiplayManager?.LeaveRoom(mRoomId);
        mMultiplayManager?.Dispose();
    }
}
