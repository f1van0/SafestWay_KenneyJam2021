using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public CellState cellState { get; private set; }
    public Vector2Int position { get; private set; }

    private int debugDistance;
    public Text debugDistanceText;
    public GameObject locked;


    public Cell(CellState _cellState)
    {
        cellState = _cellState;
    }

    public Cell()
    {
        cellState = new CellState();
    }

    public void Initialize(CellState _cellState, Vector2Int _position)
    {
        cellState = _cellState;
        this.transform.rotation = Quaternion.Euler(0, 0, (int)cellState.Direction * 90);
        position = _position;
        if (cellState.lockRotation == true && cellState.Type != CellType.startPoint && cellState.Type != CellType.endPoint && cellState.Type != CellType.none && cellState.Type != CellType.castleWalls && cellState.Type != CellType.river && cellState.Type != CellType.bridge)
        {
            this.GetComponent<SpriteRenderer>().color -= new Color(0.15f, 0.15f, 0.15f, 0f);
            locked.transform.rotation = Quaternion.Euler(0, 0, ((int)cellState.Direction + 1) * 90);
            locked.transform.position += new Vector3(0, 0, -0.01f);
            locked.SetActive(true);
        }
        else
        {
            locked.SetActive(false);
        }
    }

    public void Rotate()
    {
        if (cellState.lockRotation == false)
        {
            cellState = cellState.Rotate();
            SoundManager.instance.PlayRotate();
            this.transform.DORotate(new Vector3(0, 0, (int)cellState.Direction * 90), 0.1f, RotateMode.Fast);
        }
    }

    public void ShowDebugDistance(int _distance, Color _color)
    {
        debugDistance = _distance;
        debugDistanceText.text = debugDistance.ToString();
        debugDistanceText.color = _color;
    }

    public void Start()
    {
        debugDistanceText = GetComponentInChildren<Text>();
    }
}
