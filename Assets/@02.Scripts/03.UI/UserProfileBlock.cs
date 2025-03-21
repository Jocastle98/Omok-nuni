using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserDataStructs;

public class UserProfileBlock : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mUserRankText;
    [SerializeField] private TextMeshProUGUI mNicknameRankText;
    [SerializeField] private TextMeshProUGUI mWinLoseText;
    [SerializeField] private Image mProfileImage;

    public void SetProfileBlock(in UserRankProfileResult userInfo, int userRank)
    {
        mUserRankText.text = userRank.ToString();
        mNicknameRankText.text = userInfo.rank.ToString() + "급 " + userInfo.nickname;
        mWinLoseText.text = userInfo.wincount.ToString() + "승 " + userInfo.losecount.ToString() + "패";
        mProfileImage.sprite = GameManager.Instance.GetProfileSprite(userInfo.profileimageindex);
    }
}