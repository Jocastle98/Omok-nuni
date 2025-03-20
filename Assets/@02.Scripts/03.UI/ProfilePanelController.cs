using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanelController : PanelController
{
    [SerializeField] private TextMeshProUGUI mNicknameText;
    [SerializeField] private TextMeshProUGUI mEmailText;
    [SerializeField] private TextMeshProUGUI mCoinText;
    [SerializeField] private TextMeshProUGUI mWinText;
    [SerializeField] private TextMeshProUGUI mDrawText;
    [SerializeField] private TextMeshProUGUI mLoseText;
    [SerializeField] private TextMeshProUGUI mWinRateText;
    [SerializeField] private TextMeshProUGUI mRankupPoinText;
    [SerializeField] private TextMeshProUGUI mRankText;
    [SerializeField] private TextMeshProUGUI mWinLoseStreakText;
    [SerializeField] private TextMeshProUGUI mADBlockText;
    [SerializeField] private Image mWinLoseStreakImageObj;
    [SerializeField] private Sprite mWinStreakImage;
    [SerializeField] private Sprite mLoseStreakImage;

    public override void Show()
    {
        base.Show();
        
        
    }


public void OnClickBackButton()
    {
        Destroy(gameObject);
    }
}
