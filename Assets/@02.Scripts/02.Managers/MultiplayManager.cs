using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using SocketIOClient;
using UnityEngine;
using UserDataStructs;

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

public class UsersInfoData
{
    [JsonProperty("userId")]
    public string userId { get; set; }
    
    [JsonProperty("roomId")]
    public string roomId { get; set; }
    
    [JsonProperty("nickname")]
    public string nickname { get; set; }
    
    [JsonProperty("profileimageindex")]
    public int profileimageindex { get; set; }
    
    [JsonProperty("rank")]
    public int rank { get; set; }
    
    [JsonProperty("playerType")]
    public Enums.EPlayerType playerType { get; set; }
}

public class MultiplayManager : IDisposable
{
    private SocketIOUnity mSocket;
    
    private event Action<Enums.EMultiplayManagerState, string> mOnMultiplayStateChange;
    public Action<MoveData> OnOpponentMove;
    public Action<UsersInfoData> OnOpponentProfileUpdate;

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
        mSocket.On("rematchStart", RematchStart);
        mSocket.On("opponentProfile", OnOpponentProfileReceived);
        
        mSocket.Connect();
    }
    
    // 상대방의 프로필 정보 수신
    private void OnOpponentProfileReceived(SocketIOResponse response)
    {
        try
        {
            var data = response.GetValue<UsersInfoData>();
            OnOpponentProfileUpdate?.Invoke(data); // 프로필 정보를 UI로 전달
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in OnOpponentProfileReceived: {ex.Message}");
        }
    }

    public void SendOpponentProfile(string roomId, UsersInfoData profileData)
    {
        var data = new 
        {
            roomId,
            userId = profileData.userId,
            nickname = profileData.nickname,
            profileimageindex = profileData.profileimageindex,
            rank = profileData.rank,
            playerType = profileData.playerType
        };
        
        mSocket.Emit("opponentProfile", data);
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