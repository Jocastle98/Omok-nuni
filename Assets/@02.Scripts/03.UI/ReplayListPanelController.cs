using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReplayListPanelController : PopupPanelController
{
    [SerializeField] private Transform content;
    [SerializeField] private GameObject historyButtonPrefab;
    [SerializeField] private GameObject replayPanel;

    private async void OnEnable()
    {
        await RefreshList();
    }

    private async System.Threading.Tasks.Task RefreshList()
    {
        //  서버에서 기보 목록 가져오기
        var records = await NetworkManager.Instance.GetOmokRecords(
            successCallback: () => { Debug.Log("기보 목록 가져오기 성공"); },
            failureCallback: () => { Debug.LogWarning("기보 목록 가져오기 실패"); }
        );

        Debug.Log("records count: " + records.Count);
        // 기존 버튼 제거
        foreach (Transform child in content) 
        {
            Destroy(child.gameObject);
        }

        // records 목록 순회하여 버튼 생성
        foreach (var record in records)
        {
            // record.recordId, record.createdAt 등을 표시
            var buttonObj = Instantiate(historyButtonPrefab, content);
            var textTMP = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (textTMP != null)
            {
                textTMP.text = $"기보: {record.recordId}";
            }

            // 클릭 시 ReplayPanel 열기
            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                OpenReplay(record);
            });
        }
    }

    private void OpenReplay(OmokRecord record)
    {
        var replayPanelObj = Instantiate(replayPanel, transform.parent);
        var replayCtrl = replayPanelObj.GetComponent<ReplayPanelController>();
        if (replayCtrl != null)
        {
            var moveList = new List<(int, int, Enums.EPlayerType)>();
            foreach (var m in record.moves)
            {
                moveList.Add((m.y, m.x, (Enums.EPlayerType)m.stone));
            }
            replayCtrl.OpenReplayPanel(moveList);
        }

        // 현재 리스트 패널은 닫거나 Hide
        gameObject.SetActive(false);
    }

    public void OnCloseButtonClick()
    {
        Hide();
    }
}
