using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    public int count;
    public float speed;
    List<Cell> path;
    Field field;

    public Text countText;
    private bool isMovingPaused;
    public Cell currentCell;
    public Cell nextCell;
    public Color color;
    //public Action<void> currentCellChanged;

    public void GetDamage(int _damage)
    {
        count -= _damage;
        UserInterface.instance.ChangePowerText(count);
        countText.text = count.ToString();
    }

    public void Initialize(int _count, List<Cell> _path)
    {
        count = _count;
        countText.text = _count.ToString();
        path = _path;
        color = Color.cyan;
        field = FindObjectOfType<Field>();
        currentCell = _path[0];
        if (_path.Count > 1)
        {
            nextCell = _path[1];
        }
        this.transform.DOMove(_path[0].transform.position + new Vector3(0, 0, -1), speed / 4);
        StartCoroutine(Movement(_path));
    }

    public IEnumerator Movement(List<Cell> _path)
    {
        while (_path.Count > 0)
        {
            if (_path.Count > 1 && _path[0].isAttacking == true)
            {
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                yield return new WaitForSeconds(speed / 2);
                this.transform.DOMove(_path[0].transform.position + new Vector3(0, 0, -1), speed / 4);
                SoundManager.instance.PlayFootstep();
                yield return new WaitForSeconds(speed / 2);
                currentCell = _path[0];

                if (_path.Count > 1)
                {
                    nextCell = _path[1];
                }

                Field.instance.HeroCellChanged();
                _path.RemoveAt(0);
            }
        }

        if (_path.Count == 0)
        {
            Field.instance.CheckForWin();
        }
    }

    public void Move()
    {
        float distance = 0;
        Vector3[] pathInVectors = new Vector3[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            pathInVectors[i] = path[i].transform.position + new Vector3(0, 0, -1);

            if (i != 0)
            {
                distance += Vector3.Distance(pathInVectors[i], pathInVectors[i - 1]);
            }
        }

        //�������� ������������
        float speed = 2f;
        float duration = distance / speed;

        //this.transform.DOPath(pathInVectors, duration, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear).SetId<Tween>("Moving").OnWaypointChange(field.WaypointChanged);
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = 0.7f;
        path = new List<Cell>();
        //currentCellChanged += Field.instance.HeroCellChanged();
    }
}
