using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contants
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
