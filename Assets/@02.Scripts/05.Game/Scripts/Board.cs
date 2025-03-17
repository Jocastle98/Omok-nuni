using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Serialization;

public class Board : MonoBehaviour
{
    //초기기화 작업도 있으면 좋을듯 함

    public BoardCell[,] cells;
    public delegate void OnCellClicked(int X, int Y);
    public OnCellClicked onCellClicked;
    public int size = 13;
    
    private RectTransform mGrid;
    [SerializeField] private GameObject mCellPrefab;

    private readonly string mGirdStr = "Grid";

    private void Awake()
    {
        mGrid = Util.GetChildComponent<RectTransform>(gameObject, mGirdStr);
    }

    void Start()
    {
        InitBoard();
    }
    

    public void InitBoard()
    {
        if (cells != null)
        {
            for (int i = 0; i < size + 1; i++)
            {
                for (int j = 0; j < size + 1; j++)
                {
                    cells[i, j].playerType = Enums.EPlayerType.None;
                    //이미지 혹은 스프라이트 초기화
                }
            }
        }
        cells = new BoardCell[size + 1, size + 1];
        
        //그리드 총 길이
        float gridWidth = mGrid.rect.width;
        float gridHeight = mGrid.rect.height;
        
        //한칸 길이
        float cellWSize = gridWidth / size;
        float cellHSize = gridHeight / size;
        
        //왼쪽하단
        float originX = gridWidth * -0.5f;
        float originY = gridHeight * -0.5f;

        int blockCount = 0;
        
        for (int y = 0; y < size + 1; y++)
        {
            for (int x = 0; x < size + 1; x++)
            {
                GameObject cell = Instantiate(mCellPrefab, mGrid.transform);
                
                //위치 크기 정렬
                RectTransform cellRect = cell.GetComponent<RectTransform>();
                float posX = originX + (gridWidth / size) * x;
                float posY = originY + (gridHeight / size) * y;
        
                cellRect.localPosition = new Vector2(posX, posY);
                cellRect.sizeDelta = new Vector2(cellWSize, cellHSize);
                
                //셀 초기화
                BoardCell boardCell = cell.GetComponent<BoardCell>();
                cells[y, x] = boardCell;
                boardCell.InitBlockCell(blockCount++, (blockIndex) =>
                {
                    int X = blockIndex % (size + 1);
                    int Y = blockIndex / (size + 1);
                    
                    onCellClicked?.Invoke(Y,X);
                });
                
            }
        }
    }
    
}
