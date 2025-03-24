using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using UserDataStructs;

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

            try
            {
                await www.SendWebRequest().ToUniTask();
            }
            catch (Exception ex)
            {
                Debug.Log("Exception caught: " + ex.Message);

                if (www.responseCode == 409)
                {
                    GameManager.Instance.OpenConfirmPanel("이미 존재하는 사용자입니다.", () => { failureCallback?.Invoke(); },
                        false);
                }
                else
                {
                    GameManager.Instance.OpenConfirmPanel("서버와 통신 중 오류가 발생했습니다.", () => { failureCallback?.Invoke(); },
                        false);
                }

                return;
            }

            var result = www.downloadHandler.text;
            Debug.Log("Result: " + result);

            GameManager.Instance.OpenConfirmPanel("회원 가입이 완료 되었습니다.", () => { successCallback?.Invoke(); }, false);
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

            try
            {
                await www.SendWebRequest().ToUniTask();
            }
            catch (Exception ex)
            {
                Debug.Log("Exception caught: " + ex.Message);

                return;
            }

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
                GameManager.Instance.OpenConfirmPanel("유저네임이 유효하지 않습니다.",
                    () => { failureCallback?.Invoke(result.result); }, false);
            }
            else if (result.result == 1)
            {
                // 패스워드가 유효하지 않음
                GameManager.Instance.OpenConfirmPanel("패스워드가 유효하지 않습니다.",
                    () => { failureCallback?.Invoke(result.result); }, false);
            }
            else if (result.result == 2)
            {
                // 로그인 성공
                GameManager.Instance.OpenConfirmPanel("로그인에 성공하였습니다.",
                    () => { successCallback?.Invoke(result.nickname); }, false);
            }
        }
    }

    // 비동기 방식
    public async UniTask<UserInfoResult> GetUserInfo(Action successCallback, Action failureCallback)
    {
        string sid = PlayerPrefs.GetString("sid");
        if (sid == null)
        {
            Debug.Log("유저 데이터 불러오기에 실패했습니다. \n" +
                      "세션 데이터가 없습니다.");
            failureCallback?.Invoke();
            return new UserInfoResult();
        }

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/userinfo", UnityWebRequest.kHttpVerbGET))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", sid);
            try
            {
                await www.SendWebRequest().ToUniTask();
            }
            catch (Exception ex)
            {
                Debug.Log("Exception caught: " + ex.Message);
                if (www.responseCode == 400)
                {
                    GameManager.Instance.OpenConfirmPanel("사용자 검증 실패", () => { failureCallback?.Invoke(); },
                        false);
                }
                failureCallback?.Invoke();
            }

            var resultStr = www.downloadHandler.text;
            UserInfoResult userInfo = JsonUtility.FromJson<UserInfoResult>(resultStr);
            
            successCallback?.Invoke();
            
            return userInfo;
        }
    }
    
    // 동기적 방식 GetUserInfo 버전
    public UserInfoResult GetUserInfoSync(Action successCallback, Action failureCallback)
    {
        string sid = PlayerPrefs.GetString("sid");
        if (string.IsNullOrEmpty(sid))
        {
            Debug.Log("유저 데이터 불러오기에 실패했습니다. \n" +
                      "세션 데이터가 없습니다.");
            failureCallback?.Invoke();
            return new UserInfoResult();
        }

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/userinfo", UnityWebRequest.kHttpVerbGET))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", sid);

            // 동기적으로 요청 보내기
            www.SendWebRequest();

            // 요청이 완료될 때까지 대기
            while (!www.isDone) { }

            // 요청 결과 처리
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("유저 데이터 불러오기에 실패했습니다. \n" +
                          $"에러: {www.error}");
                if (www.responseCode == 400)
                {
                    GameManager.Instance.OpenConfirmPanel("사용자 검증 실패", () => { failureCallback?.Invoke(); },
                        false);
                }
                failureCallback?.Invoke();
                return new UserInfoResult();
            }

            var resultStr = www.downloadHandler.text;
            UserInfoResult userInfo = JsonUtility.FromJson<UserInfoResult>(resultStr);

            successCallback?.Invoke();
            return userInfo;
        }
    }
    
    public async UniTask ChangeProfileImage(ProfileImageData profileImageData, Action successCallback, Action failureCallback)
    {
        string sid = PlayerPrefs.GetString("sid");
        if (sid == null)
        {
            Debug.Log("유저 데이터 불러오기에 실패했습니다. \n" +
                      "세션 데이터가 없습니다.");
            return;
        }

        string jsonString = JsonUtility.ToJson(profileImageData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        
        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/changeprofileimage", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", sid);
            www.SetRequestHeader("Content-Type", "application/json");
            
            try
            {
                await www.SendWebRequest().ToUniTask();
            }
            catch (Exception ex)
            {
                Debug.Log("Exception caught: " + ex.Message);
                if (www.responseCode == 400)
                {
                    GameManager.Instance.OpenConfirmPanel("사용자 검증 실패", () => { failureCallback?.Invoke(); },
                        false);
                }
            }
        }
    }
    
    public async UniTask<UsersRankInfo> GetUsersRank(Action successCallback, Action failureCallback)
    {
        string sid = PlayerPrefs.GetString("sid");
        if (sid == null)
        {
            Debug.Log("유저 데이터 불러오기에 실패했습니다. \n" +
                      "세션 데이터가 없습니다.");
            failureCallback?.Invoke();
            return new UsersRankInfo();
        }

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/leaderboard", UnityWebRequest.kHttpVerbGET))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", sid);
            try
            {
                await www.SendWebRequest().ToUniTask();
            }
            catch (Exception ex)
            {
                Debug.Log("Exception caught: " + ex.Message);
                if (www.responseCode == 400)
                {
                    GameManager.Instance.OpenConfirmPanel("사용자 검증 실패", () => { failureCallback?.Invoke(); },
                        false);
                }
                failureCallback?.Invoke();
            }

            var resultStr = www.downloadHandler.text;
            UsersRankInfo userInfo = JsonUtility.FromJson<UsersRankInfo>(resultStr);
            
            successCallback?.Invoke();
            
            return userInfo;
        }
    }

    // 승리 카운트 업데이트
    public async UniTask AddWinCount(Action successCallback = null, Action failureCallback = null)
    {
        string sid = PlayerPrefs.GetString("sid"); 
        if (string.IsNullOrEmpty(sid))
        {
            Debug.LogWarning("세션 쿠키가 없습니다. 승리 카운트 업데이트 불가.");
            failureCallback?.Invoke();
            return;
        }
    
        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/addwincount", UnityWebRequest.kHttpVerbPOST))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", sid);
    
            try
            {
                await www.SendWebRequest().ToUniTask();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("서버 승리 카운트 업데이트 성공");
                    successCallback?.Invoke();
                }
                else
                {
                    Debug.LogWarning("승리 카운트 업데이트 실패: " + www.error);
                    failureCallback?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("승리 카운트 업데이트 중 예외 발생: " + ex.Message);
                failureCallback?.Invoke();
            }
        }
    }
    
    // 패배 카운트 업데이트
    public async UniTask AddLoseCount(Action successCallback = null, Action failureCallback = null)
    {
        string sid = PlayerPrefs.GetString("sid");
        if (string.IsNullOrEmpty(sid))
        {
            Debug.LogWarning("세션 쿠키가 없습니다. 패배 카운트 업데이트 불가.");
            failureCallback?.Invoke();
            return;
        }
    
        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/addlosecount", UnityWebRequest.kHttpVerbPOST))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", sid);
    
            try
            {
                await www.SendWebRequest().ToUniTask();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("서버 패배 카운트 업데이트 성공");
                    successCallback?.Invoke();
                }
                else
                {
                    Debug.LogWarning("패배 카운트 업데이트 실패: " + www.error);
                    failureCallback?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("패배 카운트 업데이트 중 예외 발생: " + ex.Message);
                failureCallback?.Invoke();
            }
        }
    }

    /// <summary>
    /// 코인 추가
    /// </summary>
    /// <param name="amount">추가할 코인 수량</param>
    /// <param name="successCallback"></param>
    /// <param name="failureCallback"></param>
    /// <returns>비동기</returns>
    public async UniTask<CoinResult> AddCoin(int amount, Action<int> successCallback = null,
        Action failureCallback = null)
    {
        string sid = PlayerPrefs.GetString("sid");
        if (string.IsNullOrEmpty(sid))
        {
            Debug.LogWarning("세션 쿠기가 없습니다. 코인 추가 불가");
            failureCallback?.Invoke();
            return new CoinResult { success = false, coin = 0, message = "세션 쿠키가 없습니다" };
        }

        CoinData coinData = new CoinData { amount = amount };
        string jsonStr = JsonUtility.ToJson(coinData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonStr);

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/addcoin", UnityWebRequest.kHttpVerbPOST))
        {
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", sid);
            www.SetRequestHeader("Content-Type", "application/json");

            try
            {
                await www.SendWebRequest().ToUniTask();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning("코인 추가 실패: " +www.error);
                    failureCallback?.Invoke();
                    return new CoinResult { success = false, coin = 0, message = www.error };
                }

                var resultStr = www.downloadHandler.text;
                CoinResult coinResult = JsonUtility.FromJson<CoinResult>(resultStr);

                if (coinResult.success)
                {
                    successCallback?.Invoke(coinResult.coin);
                }
                else
                {
                    failureCallback?.Invoke();
                }

                return coinResult;
            }
            catch (Exception ex)
            {
                Debug.LogError("코인 추가 중 예외 발생: " + ex.Message);
                failureCallback?.Invoke();
                return new CoinResult { success = false, coin = 0, message = ex.Message };
            }
        }
    }

    public async UniTask RemoveAds(Action successCallback, Action failureCallback)
    {
        string sid = PlayerPrefs.GetString("sid");
        if (string.IsNullOrEmpty(sid))
        {
            failureCallback?.Invoke();
            return;
        }

        using (UnityWebRequest www =
               new UnityWebRequest(Constants.ServerURL + "/users/removeads", UnityWebRequest.kHttpVerbPOST))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Cookie", sid);

            try
            {
                await www.SendWebRequest().ToUniTask();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    successCallback?.Invoke();
                }
                else
                {
                    failureCallback?.Invoke();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("RemoveAds 예외 발생" + ex.Message);
                failureCallback?.Invoke();
            }
        }
    }
    
}