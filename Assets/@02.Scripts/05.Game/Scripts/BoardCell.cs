using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardCell : MonoBehaviour, IPointerClickHandler
{
    //프로퍼티로 만들어서 타입에 따라 이미지를 변경
    //클릭이 된 후에 셀렉트 이미지가 있었으면 좋겠음
    

    public Enums.EPlayerType playerType;
    public delegate void OnCellClicked(int index);
    public OnCellClicked onCellClicked;
    public int blockIndex;
    
    private Sprite mSprite;
    private Image mImage;

    private void Awake()
    {
        mImage = GetComponent<Image>();
    }

    public void InitBlockCell(int blockindex, OnCellClicked onCellClicked)
    {
        blockIndex = blockindex;
        this.onCellClicked = onCellClicked;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        onCellClicked?.Invoke(blockIndex);
    }

    public void SetMark(Enums.EPlayerType playerType)
    {
        switch (playerType)
        {
            case Enums.EPlayerType.PlayerA:
                //이미지 불러와서 스프라이트에 할당
                mImage.color =  Color.green;
                break;
            case Enums.EPlayerType.PlayerB:
                mImage.color =  Color.red;
                break;
        }
    }
}
