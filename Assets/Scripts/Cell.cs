using DG.Tweening;
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
        if (cellState.lockRotation == true)
        {
            this.GetComponent<SpriteRenderer>().color -= new Color(0.15f, 0.15f, 0.15f, 0f);
        }
    }

    public void Rotate()
    {
        if (cellState.lockRotation == false)
        {
            cellState = cellState.Rotate();
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