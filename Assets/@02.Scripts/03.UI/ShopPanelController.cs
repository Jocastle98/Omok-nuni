using UnityEngine;

public class ShopPanelController : PopupPanelController
{
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
