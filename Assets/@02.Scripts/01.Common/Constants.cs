using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임에서 사용되는 상수 값들을 정의한 클래스.
/// </summary>
public static class Constants
{
    //public const string ServerURL = "http://localhost:3000"; // for local test
    public const string ServerURL = "http://34.64.121.247:3000"; // google cloud server
    //public const string GameSeverURL = "ws://localhost:3000";
    public const string GameSeverURL = "ws://34.64.121.247:3000";
    
    public const string BGMVolumeKey = "BGMVolume";
    public const string SFXVolumeKey = "SFXVolume";
    
    public const int ConsumeCoin = 100;
}
