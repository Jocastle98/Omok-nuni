using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScorePanelController : PopupPanelController
{
    [SerializeField] private Transform addScoreObjects;    
    [SerializeField] private Transform deleteScoreObjects; 
    [SerializeField] private GameObject scoreImagePrefab;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI upgradeText;
    
    [SerializeField] private Button ResignButton;
 
    /// <summary>
    /// 점수 패널 초기화
    /// </summary>
    /// <param name="currentScore">서버에서 가져온 (wincount - losecount)</param>
    /// <param name="isWin">이번 게임 승/패</param>
    /// <param name="addDelete">이번 게임 점수 증감</param>
    /// <param name="rank">현재 등급(1급 ~ 18급)</param>
    /// <param name="rankuppoints">현재 승급 포인트</param>
    public void InitializePanel(int currentScore, bool isWin, int addDelete, int rank, int rankuppoints)
    {
        Show();

        ResignButton.gameObject.SetActive(GameManager.Instance.GetIsMultiPlay());
        GameManager.Instance.OnCloseScorePanel += Hide;
        
        // 승/패 메시지
        if (isWin)
            messageText.text = $"오목에서 승리했습니다. {addDelete * 10}점을 획득!";
        else
            messageText.text = $"오목에서 패배했습니다.\n {Mathf.Abs(addDelete * 10)}점이 차감되었습니다.";

        RefreshIcons(rankuppoints);

        // 현재 등급에 따른 승급 기준 결정
        int threshold;
        if (rank >= 10)
        {
            threshold = 3;
        }
        else if (rank >= 5)
        {
            threshold = 5;
        }
        else
        {
            threshold = 10;
        }

        // 승급/강등 메시지 및 아이콘 리셋 처리
        // 승급: 승급 포인트가 threshold 이상이면 
        if (rank > 1 && rankuppoints >= threshold)
        {
            upgradeText.text = $"승급합니다!\n\n현재 등급: {rank}급.";
            RefreshIcons(0); // 승급 시 승점 아이콘 리셋 
            return; 
        }
        // 강등: 승급 포인트가 -3 이하이면 
        else if (rank < 18 && rankuppoints <= -3)
        {
            upgradeText.text = "강등합니다!";
            RefreshIcons(0); // 강등 시 아이콘 리셋
            return; 
        }
        else
        {
            int remain;
            if (rankuppoints >= 0)
            {
                remain = threshold - rankuppoints;
                upgradeText.text = $"승급까지 {remain}게임  \n\n 현재 등급: {rank}급.";
            }
            else
            {
                remain = threshold - Mathf.Abs(rankuppoints);  
                upgradeText.text = $"강등까지 {remain}게임  \n\n 현재 등급: {rank}급.";
            }
        }
    }


/// <summary>
/// 현재 점수에 따라 아이콘 갱신 
/// </summary>
private void RefreshIcons(int currentScore)
{
    // 기존 아이콘 제거
    foreach (Transform child in addScoreObjects) Destroy(child.gameObject);
    foreach (Transform child in deleteScoreObjects) Destroy(child.gameObject);

    if (currentScore > 0)
    {
        for (int i = 0; i < currentScore; i++)
        {
            Instantiate(scoreImagePrefab, addScoreObjects);
        }
    }
    else if (currentScore < 0)
    {
        int absVal = Mathf.Abs(currentScore);
        for (int i = 0; i < absVal; i++)
        {
            Instantiate(scoreImagePrefab, deleteScoreObjects);
        }
    }
}

    public void OnClickCloseButton()
    {
        Hide(() =>
        {
            GameManager.Instance.ChangeToMainScene();
            Debug.Log("메인씬으로 전환");
        });
    }

    public void OnClickRematchButton()
    {
        Hide(() =>
        {
            UniTask.Void(async () =>
            {
                await NetworkManager.Instance.ConsumeCoin(Constants.ConsumeCoin, 
                    successCallback: (remainingCoins) => 
                    {
                        GameManager.Instance.OpenConfirmPanel($"남은 코인은 {remainingCoins} 입니다.", () =>
                        {
                            GameManager.Instance.OnRematchGame?.Invoke();
                        }, false);
                    },
                    failureCallback: () =>
                    {
                        GameManager.Instance.OpenConfirmPanel("코인이 부족합니다.", () =>
                        {
                            GameManager.Instance.ChangeToMainScene();
                        }, false);
                    });
            });
        });
    }

    private void Hide()
    {
        base.Hide();
    }
}
