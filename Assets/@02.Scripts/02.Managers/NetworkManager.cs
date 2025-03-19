using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using UserDataEnums;

public class NetworkManager : Singleton<NetworkManager>
{
    // 로그아웃
    // 급수 조회
    // 코인 조회
    // 리더보드 조회

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    /// <summary>
    /// 회원가입
    /// </summary>
    /// <param name="signupData"></param>
    /// <param name="successCallback"></param>
    /// <param name="failureCallback"></param>
    public async UniTask Signup(SignupData signupData, Action successCallback, Action failureCallback)
    {
        string jsonStr = JsonUtility.ToJson(signupData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonStr);

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/signup", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            await www.SendWebRequest().ToUniTask();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + www.error);

                if (www.responseCode == 409)
                {
                    Debug.Log("중복사용자");
                    GameManager.Instance.OpenConfirmPanel("이미 존재하는 사용자입니다.", () => { failureCallback?.Invoke(); });
                }
            }
            else
            {
                var result = www.downloadHandler.text;
                Debug.Log("Result: " + result);

                GameManager.Instance.OpenConfirmPanel("회원 가입이 완료 되었습니다.", () => { successCallback?.Invoke(); });
            }
        }
    }

    /// <summary>
    /// 캐시를 통한 자동 로그인
    /// </summary>
    /// <param name="sid"></param>
    /// <returns></returns>
    public async UniTask<bool> SigninWithSidAsync(string sid)
    {
        using (UnityWebRequest www = new UnityWebRequest(Constants.ServerURL + "/users/signin/sid",
                   UnityWebRequest.kHttpVerbPOST))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", sid);

            // UnityWebRequest의 비동기 작업을 UniTask로 변환하여 await합니다.
            await www.SendWebRequest().ToUniTask();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + www.error);
                return false;
            }
            else
            {
                var resultStr = www.downloadHandler.text;
                var result = JsonUtility.FromJson<SigninResult>(resultStr);

                if (result.result == 2)
                {
                    Debug.Log("자동 로그인 성공");
                    GameManager.Instance.OpenConfirmPanel("로그인이 완료 되었습니다.", () => { });
                    return true;
                }
                else
                {
                    Debug.Log("쿠키 정보가 잘못되었습니다.");
                    return false;
                }
            }
        }
    }

    /// <summary>
    /// 로그인
    /// </summary>
    /// <param name="signinData"></param>
    /// <param name="successCallback"></param>
    /// <param name="failureCallback"></param>
    public async UniTask SigninWithSigninData(SigninData signinData, Action<string> successCallback,
        Action<int> failureCallback)
    {
        string jsonString = JsonUtility.ToJson(signinData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/signin", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            await www.SendWebRequest().ToUniTask();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error: " + www.error);
            }
            else
            {
                // 쿠키 저장
                var cookie = www.GetResponseHeader("set-cookie");
                if (!string.IsNullOrEmpty(cookie))
                {
                    int lastIndex = cookie.LastIndexOf(";");
                    if (lastIndex > 0)
                    {
                        string sid = cookie.Substring(0, lastIndex);
                        PlayerPrefs.SetString("sid", sid);
                    }
                }

                var resultString = www.downloadHandler.text;
                var result = JsonUtility.FromJson<SigninResult>(resultString);

                if (result.result == 0)
                {
                    // 유저네임이 유효하지 않음
                    GameManager.Instance.OpenConfirmPanel("유저네임이 유효하지 않습니다.", () => { failureCallback?.Invoke(result.result); });
                }
                else if (result.result == 1)
                {
                    // 패스워드가 유효하지 않음
                    GameManager.Instance.OpenConfirmPanel("패스워드가 유효하지 않습니다.", () => { failureCallback?.Invoke(result.result); });
                }
                else if (result.result == 2)
                {
                    // 로그인 성공
                    GameManager.Instance.OpenConfirmPanel("로그인에 성공하였습니다.", () => { successCallback?.Invoke(result.nickname); }, false);
                }
            }
        }
    }
}