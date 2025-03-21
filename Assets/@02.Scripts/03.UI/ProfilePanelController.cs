using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserDataStructs;

public class ProfilePanelController : PopupPanelController
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
    [SerializeField] private Image mProfileImage;
    
    private List<PopupPanelController> mChildPanels = new List<PopupPanelController>();

    public override async void Show()
    {
        mChildPanels.Clear();
        
        UserInfoResult userInfo = await NetworkManager.Instance.GetUserInfo(() => { }, () => { });
        setProfileInfo(userInfo);
        base.Show();
    }

    public override void Hide(Action hideDelegate = null)
    {
        foreach (var panel in mChildPanels)
        {
            if (panel != null)
            {
                panel.Hide();
            }
        }
        mChildPanels.Clear();
        
        base.Hide(hideDelegate);
    }

    public void OnClickProfileButton()
    {
        var childPanel = GameManager.Instance.OpenSelectProfilePanelFromProfilePanel();
        SelectProfilePanelController selectPanel = childPanel as SelectProfilePanelController;
        if (selectPanel != null)
        {
            selectPanel.OnProfileSelected = UpdateProfileInfo;
        }
        mChildPanels.Add(childPanel);
    }

    public void OnClickBackButton()
    {
        Hide();
    }

    public async void UpdateProfileInfo()
    {
        UserInfoResult userInfo = await NetworkManager.Instance.GetUserInfo(() => { }, () => { });
        setProfileInfo(userInfo);
    }

    private void setProfileInfo(UserInfoResult userInfo)
    {
        mNicknameText.text = userInfo.nickname;
        mEmailText.text = userInfo.username;
        mCoinText.text = userInfo.coin.ToString();
        mWinText.text = userInfo.wincount.ToString();
        mDrawText.text = userInfo.drawcount.ToString();
        mLoseText.text = userInfo.losecount.ToString();

        if (userInfo.wincount > 0)
        {
            float winRateValue = (float)userInfo.wincount / (userInfo.wincount + userInfo.losecount) * 100f;
            mWinRateText.text = winRateValue.ToString("F2") + "%";
        }
        else
        {
            mWinRateText.text = "0%";
        }
        mRankText.text = userInfo.rank.ToString();
        mRankupPoinText.text = userInfo.rankuppoints.ToString() + " / "+Constants.RankChangeThreshold.ToString();

        if (userInfo.winlosestreak < 0)
        {
            mWinLoseStreakImageObj.sprite = mLoseStreakImage;
        }
        else
        {
            mWinLoseStreakImageObj.sprite = mWinStreakImage;
        }

        mWinLoseStreakText.text = Mathf.Abs(userInfo.winlosestreak).ToString();
        mADBlockText.text = userInfo.hasadremoval ? "O" : "X";
        mProfileImage.sprite = GameManager.Instance.GetProfileSprite(userInfo.profileimageindex);
    }
}