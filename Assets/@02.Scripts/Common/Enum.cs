using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임에서 공용으로 사용되는 열거형(Enum) 정의 클래스.
/// </summary>
public class Enum
{
    public enum EMultiplayManagerState
    {
        CreateRoom,     // 방 생성
        JoinRoom,       // 생성된 방에 참여
        StartGame,      // 생성된 방에 다른 유저가 참여해서 게임 시작
        ExitRoom,       // 자신이 방을 빠져 나왔을 때
        EndGame         // 상대방이 접속을 끊거나 방을 나갔을 때
    };
    
    public enum EPlayerType 
    { 
        None, 
        PlayerA, 
        PlayerB 
    }

    public enum EGameType
    {
        SinglePlay,
        MultiPlay
    }
}
