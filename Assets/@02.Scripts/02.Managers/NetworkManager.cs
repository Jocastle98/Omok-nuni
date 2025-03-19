using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class NetworkManager : Singleton<NetworkManager>
{
    // 회원가입
    
    // 로그인
    
    // 로그아웃
    
    // 자동 로그인
    
    // 급수 조회
    // 코인 조회
    // 리더보드 조회

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    public void Signup(SignupData signupData, Action successCallback, Action failureCallback)
    {
        
    }

    public async Task<bool> SigninWithSidAsync(string sid)
    {
        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/signin/sid", UnityWebRequest.kHttpVerbPOST))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", sid);

            var tcs = new TaskCompletionSource<bool>();
            var operation = www.SendWebRequest();
            operation.completed += (asyncOp) => { tcs.SetResult(true); };
            
            await tcs.Task;

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
                    GameManager.Instance.OpenConfirmPanel("로그인이 완료 되었습니다.", () =>{ });
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

    public void SigninWithSigninData(SigninData signinData, Action successCallback, Action<int> failureCallback)
    {
        string jsonStr = JsonUtility.ToJson(signinData);
        
    }
}
