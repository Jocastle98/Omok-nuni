using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UserDataStructs;

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

    public async void OnClickSigninButton()
    {
        string username = mUsernameInputField.text;
        string password = mPasswordInputField.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            GameManager.Instance.OpenConfirmPanel("빈칸을 모두 채워주세요.", () => { }, false);
            return;
        }

        SigninData signinData = new SigninData(username, password);

        await NetworkManager.Instance.SigninWithSigninData(signinData, (string nickname) =>
        {
            Debug.Log("어서오세요 "+nickname+"님");
            Destroy(gameObject);
        }, (int result) =>
        {
            if (result == 0)        //INVALID_USERNAME
            {
                mUsernameInputField.text = string.Empty;
            }
            else if (result == 1)   //INVALID_PASSWORD
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