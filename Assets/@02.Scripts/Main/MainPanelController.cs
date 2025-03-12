using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanelController : MonoBehaviour
{
    public void OnClickStartButton()
    {
        // 
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
