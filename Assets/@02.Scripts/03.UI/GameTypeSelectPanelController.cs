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
            GameManager.Instance.ChangeToGameScene(Enums.EGameType.MultiPlay);
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
