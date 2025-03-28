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
    [SerializeField] private HorizontalLayoutGroup addScoreLayoutGroup;
    [SerializeField] private HorizontalLayoutGroup deleteScoreLayoutGroup;
    [SerializeField] private TextMeshProUGUI leftScoreText;
    [SerializeField] private TextMeshProUGUI rightScoreText;
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

        ResignButton.gameObject.SetActive(GameManager.Instance.bIsMultiplay);
        GameManager.Instance.OnCloseScorePanel += this.Hide;
        
        // 승/패 메시지
        if (isWin)
            messageText.text = $"오목에서 승리했습니다.\n {addDelete*10}점을 획득!";
        else
            messageText.text = $"오목에서 패배했습니다.\n {Mathf.Abs(addDelete)*10}점을 잃었습니다.";

        int minScore, maxScore, threshold;
        if (rank >= 11) 
        {
            minScore = -3; 
            maxScore = 3;  
            threshold = 3;
            leftScoreText.text = "-30";
            rightScoreText.text = "30";
        }
        else if (rank >= 6) // 10~6급
        {
            minScore = -5; 
            maxScore = 5;
            threshold = 5;   
            leftScoreText.text = "-50";
            rightScoreText.text = "50";
        }
        else // 5~1급
        {
            minScore = -10; 
            maxScore = 10;
            threshold = 10;  
            leftScoreText.text = "-100";
            rightScoreText.text = "100";
        }

        if (rank == 18 && rankuppoints < -3)
        {
            rankuppoints = -3;
        }
        rankuppoints = Mathf.Clamp(rankuppoints, minScore, maxScore);
        
        RefreshIcons(rankuppoints, threshold);

        if (rank > 1 && rankuppoints >= threshold)
        {
            upgradeText.text = $"승급합니다!\n\n현재 등급: {rank}급.";
            RefreshIcons(0, threshold);
            return; 
        }
        else if (rank < 18 && rankuppoints <= -threshold)
        {
            upgradeText.text = "강등합니다!";
            RefreshIcons(0, threshold);
            return; 
        }
        else
        {
            // 남은 승/패 횟수
            int remain;
            if (rankuppoints >= 0)
            {
                remain = threshold - rankuppoints;
                upgradeText.text = $"승급까지 {remain}번 남음\n현재 등급: {rank}급.";
            }
            else
            {
                remain = threshold - Mathf.Abs(rankuppoints);
                upgradeText.text = $"강등까지 {remain}번 남음\n현재 등급: {rank}급.";
            }
        }
    }

    /// <summary>
    /// 아이콘 설정
    /// </summary>
    private void RefreshIcons(int currentScore, int threshold)
    {

        foreach (Transform child in addScoreObjects) Destroy(child.gameObject);
        foreach (Transform child in deleteScoreObjects) Destroy(child.gameObject);

        int iconCount = Mathf.Abs(currentScore);
        if (iconCount == 0) return;

        HorizontalLayoutGroup layoutGroup = (currentScore >= 0) ? addScoreLayoutGroup : deleteScoreLayoutGroup;
        Transform parentToUse = (currentScore >= 0) ? addScoreObjects : deleteScoreObjects;

        float parentWidth = layoutGroup.GetComponent<RectTransform>().rect.width; 
        float spacing = layoutGroup.spacing; // Inspector에서 설정

        float totalSpacing = (threshold - 1) * spacing;
        float baseIconSize = (parentWidth - totalSpacing) / threshold;
        if (baseIconSize < 0) baseIconSize = 0;

        float scaleFactor = 1f;
        if (iconCount > threshold)
        {
            scaleFactor = (float)threshold / iconCount;
        }
        float finalSize = baseIconSize * scaleFactor;

        for (int i = 0; i < iconCount; i++)
        {
            var icon = Instantiate(scoreImagePrefab, parentToUse);
            LayoutElement le = icon.AddComponent<LayoutElement>();
            le.preferredWidth = finalSize;
            le.preferredHeight = finalSize;
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
        if (GameManager.Instance.bIsRecieveRematch)
        {
            return;
        }
        else
        {
            Hide(() =>
            {
                UniTask.Void(async () =>
                {
                    await NetworkManager.Instance.ConsumeCoin(Constants.ConsumeCoin, 
                        successCallback: (remainingCoins) => 
                        {
                            GameManager.Instance.OpenConfirmPanel($"남은 코인은 {remainingCoins} 입니다.",
                                () =>
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
    }

    private void Hide()
    {
        base.Hide();
    }
}
