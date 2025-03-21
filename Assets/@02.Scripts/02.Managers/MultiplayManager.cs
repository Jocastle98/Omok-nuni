using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SocketIOClient;
using UnityEngine;

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

public class OpponentInfoResult
{
    [JsonProperty("opponentNickname")]
    public string opponentNickname { get; set; }
    
    [JsonProperty("opponentProfileImageIndex")]
    public int opponentProfileImageIndex { get; set; }
    
    [JsonProperty("opponentRank")]
    public int opponentRank { get; set; }
}

public class MultiplayManager : IDisposable
{
    private SocketIOUnity mSocket;
    
    private event Action<Enums.EMultiplayManagerState, string> mOnMultiplayStateChange;
    public Action<OpponentInfoResult> OnOpponentInfoReceived;
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
        mSocket.On("receiveOpponentInfo", ReceiveOpponentInfo);
        mSocket.On("doOpponent", DoOpponent);
        mSocket.On("rematchStart", RematchStart);
        
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
    
    // 상대방 정보 수신
    private void ReceiveOpponentInfo(SocketIOResponse response)
    {
        var data = response.GetValue<OpponentInfoResult>();
        OnOpponentInfoReceived?.Invoke(data);
        Debug.Log(OnOpponentInfoReceived);
    }
    
    // 플레이어의 정보를 서버로 전달하기 위한 메서드
    public void SendOpponentInfo(string roomId, string nickname, int profileImageIndex, int rank)
    {
        mSocket.Emit("opponentInfo", new { roomId, nickname, profileImageIndex, rank });
    } 
    
    // 서버로부터 상대방의 마커 정보를 받기 위한 메서드
    private void DoOpponent(SocketIOResponse response)
    {
        try
        {
            var data = response.GetValue<MoveData>();
            OnOpponentMove?.Invoke(data);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in DoOpponent: {ex.Message}");
        }
    }
    
    // 플레이어의 마커 위치를 서버로 전달하기 위한 메서드
    public void SendPlayerMove(string roomId, int position)
    {
        mSocket.Emit("doPlayer", new { roomId , position });
    }

    public void LeaveRoom(string roomId)
    {
        mSocket.Emit("leaveRoom", new { roomId });
    }
    
    // 재대국 신청 메서드
    public void RequestRematch(string roomId)
    {
        mSocket.Emit("requestRematch", new { roomId });
    }
    
    // 새로운 방으로 이동 (서버에서 받은 새로운 방 정보 처리)
    private void RematchStart(SocketIOResponse response)
    {
        var data = response.GetValue<RoomData>();
        mOnMultiplayStateChange?.Invoke(Enums.EMultiplayManagerState.StartGame, data.roomId);
    }
    
    public void Dispose()
    {
        if (mSocket != null)
        {
            mSocket.Disconnect();
            mSocket.Dispose();
            mSocket = null;
        }
    }
}
