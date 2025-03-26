using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UserDataStructs; 

public class ReplayPanelController : PopupPanelController
{
    [SerializeField] private BoardCellController boardCellController;

    [SerializeField] private Image blackPlayerProfileImage;
    [SerializeField] private TextMeshProUGUI blackPlayerProfileText;  
    [SerializeField] private CanvasGroup blackTurnPanel;    

    [SerializeField] private Image whitePlayerProfileImage;
    [SerializeField] private TextMeshProUGUI whitePlayerProfileText;
    [SerializeField] private CanvasGroup whiteTurnPanel;

    private List<(int y, int x, Enums.EPlayerType stone)> _moves;
    private int _currentIndex = -1;

    private UserInfoResult blackUserInfo;
    private UserInfoResult whiteUserInfo;

    public void OpenReplayPanel(
        List<(int y, int x, Enums.EPlayerType stone)> moves,
        UserInfoResult blackInfo = default,
        UserInfoResult whiteInfo = default)
    {
        if (moves == null || moves.Count == 0) return;
        _moves = moves;
        _currentIndex = -1;

        blackUserInfo = blackInfo;
        whiteUserInfo = whiteInfo;

        boardCellController.InitBoard();

        SetPlayerProfiles();

        HighlightCurrentPlayer(Enums.EPlayerType.None);
    }

    private bool IsValidUserInfo(UserInfoResult info)
    {
        return !string.IsNullOrEmpty(info.username);
    }
    /// <summary>
    /// 흑/백 플레이어의 프로필 이미지를 세팅하고, 급수 ,닉네임 표시
    /// </summary>
    private void SetPlayerProfiles()
    {
        if (IsValidUserInfo(blackUserInfo))
        {
            blackPlayerProfileImage.sprite = GameManager.Instance.GetProfileSprite(blackUserInfo.profileimageindex);
            blackPlayerProfileText.text = $"{blackUserInfo.rank}급 {blackUserInfo.nickname}";
        }
        else
        {
            blackPlayerProfileImage.sprite = GameManager.Instance.GetProfileSprite(0);
            blackPlayerProfileText.text = "흑(Black)";
        }

        if (IsValidUserInfo(whiteUserInfo))
        {
            whitePlayerProfileImage.sprite = GameManager.Instance.GetProfileSprite(whiteUserInfo.profileimageindex);
            whitePlayerProfileText.text = $"{whiteUserInfo.rank}급 {whiteUserInfo.nickname}";
        }
        else
        {
            whitePlayerProfileImage.sprite = GameManager.Instance.GetProfileSprite(1);
            whitePlayerProfileText.text = "백(White)";
        }
    }


    /// <summary>
    /// 다음 수로 이동 
    /// </summary>
    public void OnNextButtonClick()
    {
        if (_currentIndex + 1 < _moves.Count)
        {
            _currentIndex++;
            var move = _moves[_currentIndex];
            boardCellController.cells[move.y, move.x].SetMark(move.stone);
            boardCellController.cells[move.y, move.x].playerType = move.stone;

            HighlightCurrentPlayer(move.stone);
        }
    }

    /// <summary>
    /// 이전 수로 이동 
    /// </summary>
    public void OnPrevButtonClick()
    {
        if (_currentIndex >= 0)
        {
            var move = _moves[_currentIndex];
            boardCellController.cells[move.y, move.x].SetMark(Enums.EPlayerType.None);
            boardCellController.cells[move.y, move.x].playerType = Enums.EPlayerType.None;
            _currentIndex--;

            if (_currentIndex >= 0)
            {
                var prevMove = _moves[_currentIndex];
                HighlightCurrentPlayer(prevMove.stone);
            }
            else
            {
                HighlightCurrentPlayer(Enums.EPlayerType.None);
            }
        }
    }

    /// <summary>
    /// 흑/백 중 어느 쪽이 현재 수를 뒀는지 UI로 강조
    /// </summary>
    private void HighlightCurrentPlayer(Enums.EPlayerType currentStone)
    {
        float activeAlpha = 1f;
        float inactiveAlpha = 0.3f;

        if (currentStone == Enums.EPlayerType.Player_Black)
        {
            blackTurnPanel.alpha = activeAlpha;
            whiteTurnPanel.alpha = inactiveAlpha;
        }
        else if (currentStone == Enums.EPlayerType.Player_White)
        {
            blackTurnPanel.alpha = inactiveAlpha;
            whiteTurnPanel.alpha = activeAlpha;
        }
        else
        {
            blackTurnPanel.alpha = inactiveAlpha;
            whiteTurnPanel.alpha = inactiveAlpha;
        }
    }

    public void OnCloseButtonClick()
    {
        Hide();
    }
}
