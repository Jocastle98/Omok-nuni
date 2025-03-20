using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UserDataStructs;

public class SignupPanelController : PanelController
{
    [SerializeField] private TMP_InputField mUsernameInputField;
    [SerializeField] private TMP_InputField mNicknameInputField;
    [SerializeField] private TMP_InputField mPasswordInputField;
    [SerializeField] private TMP_InputField mConfirmPasswordInputField;

    public async void OnClickConfirmButton()
    {
        var username = mUsernameInputField.text;
        var nickname = mNicknameInputField.text;
        var password = mPasswordInputField.text;
        var confirmPassword = mConfirmPasswordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(confirmPassword))
        {
            GameManager.Instance.OpenConfirmPanel("입력할 항목이 남아있습니다.", () => { }, false);
            return;
        }

        if (password.Equals(confirmPassword))
        {
            SignupData signupData = new SignupData();
            signupData.username = username;
            signupData.nickname = nickname;
            signupData.password = password;

            // 서버로 SignupData 전달하면서 회원가입 진행
            await NetworkManager.Instance.Signup(signupData, () => { Destroy(gameObject); }, () =>
            {
                mUsernameInputField.text = string.Empty;
                mNicknameInputField.text = string.Empty;
                mPasswordInputField.text = string.Empty;
                mConfirmPasswordInputField.text = string.Empty;
            });
        }
        else
        {
            GameManager.Instance.OpenConfirmPanel("비밀번호가 서로 다릅니다.", () =>
            {
                mPasswordInputField.text = string.Empty;
                mConfirmPasswordInputField.text = string.Empty;
            }, false);
        }
    }


    public void OnClickCancelButton()
    {
        Destroy(gameObject);
    }
}