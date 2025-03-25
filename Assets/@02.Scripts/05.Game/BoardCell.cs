using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;

public class BoardCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]private Image mStoneImage;
    [SerializeField]private Image mUtilImage;
    [SerializeField]private List<Sprite> mImages;
    
    public Enums.EPlayerType playerType = Enums.EPlayerType.None;
    public delegate void OnCellClicked(int index);
    public OnCellClicked onCellClicked;
    public int cellIndex;
    public bool IsForbidden = false;
    
    public void InitBlockCell(int blockindex, OnCellClicked onCellClicked)
    {
        cellIndex = blockindex;
        this.onCellClicked = onCellClicked;
    }

    public void ResetCell()
    {
        mStoneImage.DOFade(0,0);
        mStoneImage.sprite = GetImage(Enums.EGameImage.None);
        IsForbidden = false;
        playerType = Enums.EPlayerType.None;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        onCellClicked?.Invoke(cellIndex);
    }

    public void SetMark(Enums.EPlayerType playerType)
    {
        switch (playerType)
        {
            case Enums.EPlayerType.Player_Black:
                mStoneImage.DOFade(1,0);
                mStoneImage.sprite = GetImage(Enums.EGameImage.BlackStone);
                break;
            case Enums.EPlayerType.Player_White:
                mStoneImage.DOFade(1,0);
                mStoneImage.sprite = GetImage(Enums.EGameImage.WhiteStone);
                break;
        }
    }

    public void SelectMark(bool onMark)
    {
        if (onMark)
        {
            mUtilImage.DOFade(1,0);
            mUtilImage.sprite = GetImage(Enums.EGameImage.Selector);
        }
        else
        {
            mUtilImage.DOFade(0,0);
            mUtilImage.sprite = GetImage(Enums.EGameImage.None);
        }
    }

    public void OnForbbiden(bool isForbidden,BasePlayerState Player_Black)
    {
        if (isForbidden)
        {
            IsForbidden = true;
            PlayerState player = Player_Black as PlayerState;
            player.onForbbidenMark += ForbbidneMark;

        }
        else
        {
            IsForbidden = false;
            PlayerState player = Player_Black as PlayerState;
            player.onForbbidenMark -= ForbbidneMark;
            
            mUtilImage.DOFade(0,0);
            mUtilImage.sprite = GetImage(Enums.EGameImage.None);
        }
    }

    public void ForbbidneMark(bool onMark)
    {
        if (onMark)
        {
            mUtilImage.DOFade(1,0);
            mUtilImage.sprite = GetImage(Enums.EGameImage.XMarker);
        }
        else
        {
            mUtilImage.DOFade(0,0);
            mUtilImage.sprite = GetImage(Enums.EGameImage.None);
        }
    }

    public Sprite GetImage(Enums.EGameImage GameImage)
    {
        if (GameImage == Enums.EGameImage.None)
        {
            return null;
        }
        return mImages[(int)GameImage];
    }
}
