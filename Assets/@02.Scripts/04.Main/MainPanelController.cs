using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 메인 화면에서 UI 버튼 입력을 처리하는 컨트롤러.
/// GameManager를 통해 각종 패널을 열거나 씬을 전환하는 역할을 수행.
/// </summary>
public class MainPanelController : MonoBehaviour
{
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
    
    // 로그아웃 클릭 시 호출되는 메서드 구현
}
