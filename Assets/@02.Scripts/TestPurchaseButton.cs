using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPurchaseButton : MonoBehaviour
{
    private IAPManager inAppPurchase;
    private void Awake()
    {
        inAppPurchase = GetComponent<IAPManager>();
    }

    public void OnClickShopItem(int type)
    {
        IAPManager.EItemType selectedItem = (IAPManager.EItemType)type;

        if (selectedItem == IAPManager.EItemType.NoAds || selectedItem == IAPManager.EItemType.NoAds_Coin_2000)
        {
            if (inAppPurchase.HadPurchased(selectedItem))
            {
                Debug.Log("이미 구매한 상품입니다");
                return;
            }
            
        }
        inAppPurchase.BuyProduct(selectedItem);
    }
}
