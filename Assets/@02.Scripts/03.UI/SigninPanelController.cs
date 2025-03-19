using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public struct SigninData
{
    public string username;
    public string password;
}

struct SigninResult
{
    public int result;
    public string nickname;
}

public struct ScoreResult
{
    public string id;
    public string username;
    public string nickname;
    public int score;
}

[Serializable]
public struct ScoreInfo
{
    public string username;
    public string nickname;
    public int score;

    public ScoreInfo(string username, string nickname, int score)
    {
        this.username = username;
        this.nickname = nickname;
        this.score = score;
    }
}

[Serializable]
public struct Scores
{
    public ScoreInfo[] scores;
    public string playerName;
    public string playerNickname;
    public int playerScore;
}

public class SigninPanelController : PanelController
{
    [SerializeField] private TMP_InputField mUsernameInputField;
    [SerializeField] private TMP_InputField mPasswordInputField;

    public void OnClickSigninButton()
    {
        string username = mUsernameInputField.text;
        string password = mPasswordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            //TODO: 누락된 값 입력 요청 팝업 표시
            return;
        }

        SigninData signinData = new SigninData();
        signinData.username = username;
        signinData.password = password;

        NetworkManager.Instance.SigninWithSigninData(signinData, () =>
        {
            Destroy(gameObject);
        }, (int result) =>
        {
            if (result == 0)
            {
                mUsernameInputField.text = string.Empty;
            }
            else if(result == 1)
            {
                mPasswordInputField.text = string.Empty;
            }
        });
    }
    
    public void OnClickSignupButton()
    {
        GameManager.Instance.OpenSignupPanel();
    }
}