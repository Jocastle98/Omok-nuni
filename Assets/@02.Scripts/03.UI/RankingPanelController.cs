using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UserDataStructs;

public class RankingPanelController : PopupPanelController
{
    [SerializeField] private GameObject mContentsBoard;
    [SerializeField] private GameObject mUserProfileBlockPrefab;
    [SerializeField] private UserProfileBlock mPlayerProfileBlock;
    
    public override async void Show()
    {
        UsersRankInfo usersRankInfo = await NetworkManager.Instance.GetUsersRank(() => { }, () => { });
        
        int playerRank = -1;
        for (int i = 0; i < usersRankInfo.userrankprofiles.Length; i++)
        {
            var userRankBlock = Instantiate(mUserProfileBlockPrefab, mContentsBoard.transform);
            userRankBlock.GetComponent<UserProfileBlock>().SetProfileBlock(in usersRankInfo.userrankprofiles[i], i + 1);

            if (usersRankInfo.userrankprofiles[i].username == usersRankInfo.playerrankprofile.username)
            {
                playerRank = i + 1;
            }
        }
        
        mPlayerProfileBlock.SetProfileBlock(usersRankInfo.playerrankprofile, playerRank);
        mPlayerProfileBlock.SetColor(Color.red);
        
        base.Show();
    }
    
    public void OnClickBackButton()
    {
        Hide();
    }
}
