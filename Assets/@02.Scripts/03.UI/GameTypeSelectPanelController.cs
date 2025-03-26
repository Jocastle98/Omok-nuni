using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameTypeSelectPanelController : PopupPanelController
{
    [SerializeField] private GameObject passAndPlayButton;
    [SerializeField] private GameObject multiplayButton;
    [SerializeField] private GameObject passAndPlayButtonFade;
    
    public void OnClickPassAndPlayButton()
    {
        Hide(() =>
        {
            GameManager.Instance.ChangeToGameScene(Enums.EGameType.PassAndPlay);
        });
    }
    
    public void OnClickMultiplayButton()
    {
        Hide(() =>
        {
            NetworkManager.Instance.ConsumeCoin(Constants.ConsumeCoin, 
                successCallback: (remainingCoins) => 
                {
                    GameManager.Instance.OpenConfirmPanel($"남은 코인은 {remainingCoins} 입니다.", () =>
                    {
                        GameManager.Instance.ChangeToGameScene(Enums.EGameType.MultiPlay);
                    }, false);
                },
                failureCallback: () =>
                {
                    GameManager.Instance.OpenConfirmPanel("코인이 부족합니다.", () => { }, false);
                });
        });
    }
    
    public void OnClickMultiplayFadeButton()
    {
        Hide(() =>
        {
            GameManager.Instance.ChangeToGameScene(Enums.EGameType.PassAndPlayFade);
        });
    }

    public void OnClickBackButton()
    {
        Hide();
    }
}
