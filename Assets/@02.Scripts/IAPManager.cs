using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public enum EItemType
    {
        NoAds,          //광고 제거
        NoAds_Coin_2000, //광고제거 + 코인 2000개
        Coin_1000,       //코인 1000개
        Coin_2000,       //코인 2000개
        Coin_4500,      //코인 4500개
        Coin_10000,      //코인 10000개
    }
    
    //상품의 ID 부여 (앱 상품 페이지에서 직접 설정)
    public const string PRODUCT_ID_COIN_1000 = "coin_1000";
    public const string PRODUCT_ID_COIN_2000 = "coin_2000";
    public const string PRODUCT_ID_COIN_4500 = "coin_4500";
    public const string PRODUCT_ID_COIN_10000 = "coin_10000";
    public const string PRODUCT_ID_NOADS = "noads";
    public const string PRODUCT_ID_NOADS_COIN_2000 = "noads_coin_2000";
    

    private IStoreController mStoreController; //구매 과정을 제어하는 함수를 제공
    private IExtensionProvider mStoreExtensionProvider; //여러 플랫폼을 위한 확장 처리를 제공

    private void Start()
    {
        if (mStoreController == null)
        {
            InitPurchasing();
        }
    }
    
    //초기화 작업
    private void InitPurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance()); //유니티가 기본적으로 제공하는 스토어 설정
        builder.AddProduct(PRODUCT_ID_COIN_1000, ProductType.Consumable);
        builder.AddProduct(PRODUCT_ID_COIN_2000, ProductType.Consumable);
        builder.AddProduct(PRODUCT_ID_COIN_4500, ProductType.Consumable);
        builder.AddProduct(PRODUCT_ID_COIN_10000, ProductType.Consumable);
        builder.AddProduct(PRODUCT_ID_NOADS, ProductType.NonConsumable);
        builder.AddProduct(PRODUCT_ID_NOADS_COIN_2000, ProductType.NonConsumable);
        UnityPurchasing.Initialize(this, builder);
    }
    
    private bool IsInitialized()
    {
        return mStoreController != null && mStoreExtensionProvider != null;
    }

    public void BuyProduct(EItemType itemType)
    {
        switch (itemType)
        {
            case EItemType.Coin_1000:
                BuyProductID(PRODUCT_ID_COIN_1000);
                break;
            case EItemType.Coin_2000:
                BuyProductID(PRODUCT_ID_COIN_2000);
                break;
            case EItemType.Coin_4500:
                BuyProductID(PRODUCT_ID_COIN_4500);
                break;
            case EItemType.Coin_10000:
                BuyProductID(PRODUCT_ID_COIN_10000);
                break;
            case EItemType.NoAds:
                BuyProductID(PRODUCT_ID_NOADS);
                break;
            case EItemType.NoAds_Coin_2000:
                BuyProductID(PRODUCT_ID_NOADS_COIN_2000);
                break;
        }
    }
    
    private void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = mStoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                mStoreController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public bool HadPurchased(string productId)
    {
        if (!IsInitialized()) return false;
        var product = mStoreController.products.WithID(productId);
        if (product != null) {return product.hasReceipt;}
        return false;
    }
    
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        mStoreController = controller;
        mStoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializedFailed InitializationFailureReason:" + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Debug.Log("OnInitializedFailed InitializationFailureReason:" + error + "message:" + message);
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed : FAIL. Product: '{0}', PurchaseFailureResone: {1}",
            product.definition.storeSpecificId, failureReason));
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        if (string.Equals(purchaseEvent.purchasedProduct.definition.id, PRODUCT_ID_COIN_1000, StringComparison.Ordinal))
        {
            Debug.Log("ProcessPurchase: PASS. Product: '" + purchaseEvent.purchasedProduct.definition.id + "'");
            
            //TODO: 코인 1000개 지급
            
        }
        else if (string.Equals(purchaseEvent.purchasedProduct.definition.id, PRODUCT_ID_COIN_2000, StringComparison.Ordinal))
        {
            //TODO: 코인 2000개 지급
            Debug.Log("코인 2000개 지급");
        }
        else if (string.Equals(purchaseEvent.purchasedProduct.definition.id, PRODUCT_ID_COIN_4500, StringComparison.Ordinal))
        {
            //TODO: 코인 4500개 지급
            Debug.Log("코인 4500개 지급");
        }
        else if (string.Equals(purchaseEvent.purchasedProduct.definition.id, PRODUCT_ID_COIN_10000, StringComparison.Ordinal))
        {
            //TODO: 코인 10000개 지급
            Debug.Log("코인 10000개 지급");
        }
        else if (string.Equals(purchaseEvent.purchasedProduct.definition.id, PRODUCT_ID_NOADS, StringComparison.Ordinal))
        {
            //TODO: 광고제거 지급
            Debug.Log("광고제거 지급");
        }
        else if (string.Equals(purchaseEvent.purchasedProduct.definition.id, PRODUCT_ID_NOADS_COIN_2000, StringComparison.Ordinal))
        {
            //TODO: 광고제거 + 코인 2000개 지급
            Debug.Log("광고제거 + 코인 2000개 지급");
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", purchaseEvent.purchasedProduct.definition.id));
        }

        return PurchaseProcessingResult.Complete;
    }

}
