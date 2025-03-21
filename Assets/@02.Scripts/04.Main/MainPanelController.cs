using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserDataStructs;

/// <summary>
/// 메인 화면에서 UI 버튼 입력을 처리하는 컨트롤러.
/// GameManager를 통해 각종 패널을 열거나 씬을 전환하는 역할을 수행.
/// </summary>
public class MainPanelController : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private TMP_Text userInfoText;
    [SerializeField] private TMP_Text coinText;

    private void Start()
    {
        GameManager.Instance.OnMainPanelUpdate += SetProfileInfo;
    }
    
    public async void SetProfileInfo()
    {
        UserInfoResult userInfo = await NetworkManager.Instance.GetUserInfo(() => { }, () => { });

        profileImage.sprite = GameManager.Instance.GetProfileSprite(userInfo.profileimageindex);
        userInfoText.text = $"{userInfo.rank}급 {userInfo.nickname}";
        coinText.text = $"코인: {userInfo.coin}";
    }
    
    public void OnClickStartButton()
    {
        GameManager.Instance.OpenGameTypeSelectPanel();
    }
    
    public void OnClickRecordButton()
    {
        GameManager.Instance.OpenRecordPanel();
    }

    public void OnClickLeaderboardButton()
    {
        GameManager.Instance.OpenLeaderboardPanel();
    }
    public void OnClickShopButton()
    {
        GameManager.Instance.OpenShopPanel();
    }

    public void OnClickSettingsButton()
    {
        GameManager.Instance.OpenSettingsPanel();
    }

    public void OnClickProfileButton()
    {
        GameManager.Instance.OpenProfilePanel();
    }
    
    // 로그아웃 클릭 시 호출되는 메서드 구현
}
