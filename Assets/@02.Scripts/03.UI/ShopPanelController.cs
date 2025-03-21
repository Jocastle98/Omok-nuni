using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ShopPanelController : PopupPanelController
{
    public TMP_Text coinText;

    private void OnEnable()
    {
        NetworkManager.Instance.GetUserInfo(() =>
        {
            Debug.Log("코인 로드 완료");
        }, () =>
        {
            Debug.Log("코인 로드 실패");
            
        }).ContinueWith(userInfo =>
        {
            int coin = userInfo.coin;
            coinText.text = coin.ToString();

        });
    }

    public void OnClickCloseButton()
    {
        Hide();
    }

    public void OnClickRewardAds()
    {
        AdmobAdsManager.Instance.ShowRewardedAd();
    }

    public void OnClickShopItem(int type)
    {
        Enums.EItemType selectedItem = (Enums.EItemType)type;

        if (selectedItem == Enums.EItemType.NoAds || selectedItem == Enums.EItemType.NoAds_Coin_2000)
        {
            if (IAPManager.Instance.HadPurchased(selectedItem))
            {
                Debug.Log("이미 구매한 상품입니다");
                return;
            }
            
        }
        IAPManager.Instance.BuyProduct(selectedItem);
    }
}
