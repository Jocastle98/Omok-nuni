using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SocketIOClient;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomData
{
    [JsonProperty("roomId")]
    public string roomId { get; set; }
}

public class MoveData
{
    [JsonProperty("position")]
    public int position { get; set; }
}

public class MultiplayManager : IDisposable
{
    private SocketIOUnity mSocket;
    
    private event Action<Enums.EMultiplayManagerState, string> mOnMultiplayStateChange;
    public Action<MoveData> OnOpponentMove;

    public MultiplayManager(Action<Enums.EMultiplayManagerState, string> onMultiplayStateChange)
    {
        mOnMultiplayStateChange = onMultiplayStateChange;
        
        var uri = new Uri(Constants.GameSeverURL);
        mSocket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
        
        mSocket.On("createRoom", CreateRoom);
        mSocket.On("joinRoom", JoinRoom);
        mSocket.On("startGame", StartGame);
        mSocket.On("exitRoom", ExitRoom);
        mSocket.On("endGame", EndGame);
        mSocket.On("doOpponent", DoOpponent);
        
        mSocket.Connect();
    }
    
    // 자신이 방(세션)을 생성
    private void CreateRoom(SocketIOResponse response)
    {
        var data = response.GetValue<RoomData>();
        mOnMultiplayStateChange?.Invoke(Enums.EMultiplayManagerState.CreateRoom, data.roomId);
    }
    
    // 상대방이 생성한 방(세션)에 참가
    private void JoinRoom(SocketIOResponse response)
    {
        var data = response.GetValue<RoomData>();
        mOnMultiplayStateChange?.Invoke(Enums.EMultiplayManagerState.JoinRoom, data.roomId);
    }
    
    // 생성된 방에 상대방이 참가 했을 때 게임 시작
    private void StartGame(SocketIOResponse response)
    {
        var data = response.GetValue<RoomData>();
        mOnMultiplayStateChange?.Invoke(Enums.EMultiplayManagerState.StartGame, data.roomId);
    }

    // 자신이 방에서 나갔을 때
    private void ExitRoom(SocketIOResponse response)
    {
        mOnMultiplayStateChange?.Invoke(Enums.EMultiplayManagerState.ExitRoom, null);
    }
    
    // 상대방이 방에서 나갔을 때
    private void EndGame(SocketIOResponse response)
    {
        mOnMultiplayStateChange?.Invoke(Enums.EMultiplayManagerState.EndGame, null);
    }
    
    // 서버로부터 상대방의 마커 정보를 받기 위한 메서드
    private void DoOpponent(SocketIOResponse response)
    {
        var data = response.GetValue<MoveData>();
        OnOpponentMove?.Invoke(data);
    }

    // 플레이어의 마커 위치를 서버로 전달하기 위한 메서드
    public void SendPlayerMove(string roomId, int position)
    {
        mSocket.Emit("doPlayer", new { roomId , position });
    }

    public void Dispose()
    {
        
    }
}
