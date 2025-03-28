using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelController : PopupPanelController
{

    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    private void OnEnable()
    {
        AudioManager.Instance.InitSliders(_bgmSlider, _sfxSlider);
    }

    public void OnClickSignoutButton()
    {
        Hide(() =>
        {
            UniTask.Void(async () =>
            {
                await NetworkManager.Instance.Signout(() =>
                {
                    GameManager.Instance.OpenSigninPanel();
                }, () => { });
            });
        });
    }
    
    public void OnClickClosedButton()
    {
        Hide();
    }
    
    public override void Hide(Action OnPanelControllerHide = null)
    {
        FindObjectOfType<MainButtonAnimation>().ShowAllStone();

        base.Hide(OnPanelControllerHide);
    }
}