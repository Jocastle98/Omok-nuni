using UnityEngine;
using TMPro;

public class ScorePanelController : PopupPanelController
{
    [SerializeField] private Transform addScoreObjects;    
    [SerializeField] private Transform deleteScoreObjects; 
    [SerializeField] private GameObject scoreImagePrefab;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private TextMeshProUGUI upgradeText;

    /// <summary>
    /// 점수 패널 초기화
    /// </summary>
    public void InitializePanel(int currentScore, bool isWin, int addDelete)
    {
    
        Show();
        
        // 승/패 메시지
        if (isWin)
            messageText.text = $"오목에서 승리했습니다.\n{addDelete * 10}점을 획득!";
        else
            messageText.text = $"오목에서 패배했습니다.\n{Mathf.Abs(addDelete * 10)}점이 차감되었습니다.";

        // 아이콘 표시
        RefreshIcons(currentScore);

        // 승급, 강등 메세지
        if (currentScore >= 30)
        {
            upgradeText.text = $"현재 점수: {currentScore * 10}\n 승급합니다!";
        }
        else if (currentScore <= -30)
        {
            upgradeText.text = $"현재 점수: {currentScore * 10}\n 강등합니다!";
        }
        else
        {
            int remainToMax = 30 - currentScore * 10;
            upgradeText.text = $"현재 점수: {currentScore * 10}\n"
                               + $"{remainToMax}점 더 얻으면 승급";
        }
    }

    /// <summary>
    /// 현재 점수에 따라 아이콘갱신
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
        Hide();
    }
}
