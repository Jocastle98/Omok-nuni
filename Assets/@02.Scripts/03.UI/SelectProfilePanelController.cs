using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UserDataStructs;

public class SelectProfilePanelController : PopupPanelController
{
    [SerializeField] private List<Image> mProfileImages;
    public Action OnProfileSelected;

    public override void Show()
    {
        for (int i = 0; i < mProfileImages.Count; i++)
        {
            mProfileImages[i].sprite = GameManager.Instance.GetProfileSprite(i);
        }
        base.Show();
    }

    public void OnClickCancelButton()
    {
        Hide();
    }

    public async void OnClickSelectProfileButton(int profileIndex)
    {
        await NetworkManager.Instance.ChangeProfileImage(new ProfileImageData(profileIndex), () => { }, () => { });
        OnProfileSelected?.Invoke();
        Hide();
    }
}
