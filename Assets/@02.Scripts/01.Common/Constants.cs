using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임에서 사용되는 상수 값들을 정의한 클래스.
/// </summary>
public static class Constants
{
    public const string ServerURL = "http://localhost:3000"; // for local test
    //public const string ServerURL = "https://golden-hologram-453916-a2.du.r.appspot.com"; // google cloud server
    public const string GameSeverURL = "ws://localhost:3000";
    
    public const string BGMVolumeKey = "BGMVolume";
    public const string SFXVolumeKey = "SFXVolume";

    //TODO: 급수시스템에서 사용하는 변수와 연결
    public const int RankChangeThreshold = 30;
}
