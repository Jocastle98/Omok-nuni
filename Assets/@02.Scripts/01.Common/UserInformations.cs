using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 사용자 정보를 저장하고 관리하는 클래스.
/// PlayerPrefs를 활용하여 데이터를 저장하고 불러온다.
/// </summary>
public class UserInformations
{
    // 배경음악 재생 여부
    public static bool IsPlayBGM
    {
        get { return PlayerPrefs.GetInt("IsPlayBGM", 1) == 1; }
        set { PlayerPrefs.SetInt("IsPlayBGM", value ? 1 : 0); PlayerPrefs.Save(); }
    }
    
    // 효과음 재생 여부
    public static bool IsPlaySFX
    {
        get { return PlayerPrefs.GetInt("IsPlaySFX", 1) == 1; }
        set { PlayerPrefs.SetInt("IsPlaySFX", value ? 1 : 0); PlayerPrefs.Save(); }
    }
    
    //광고제거 아이템 구매 여부
    public static bool IsNoAds
    {
        get { return PlayerPrefs.GetInt("IsNoAds", 0) == 1; }
        set { PlayerPrefs.SetInt("IsNoAds", value ? 1 : 0); PlayerPrefs.Save();}
    }
}
