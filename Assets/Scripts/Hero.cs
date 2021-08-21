using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    int count;
    List<Cell> path;
    Field field;

    public Text countText;
    private bool isMovingPaused;

    public void Initialize(int _count, List<Cell> _path)
    {
        count = _count;
        countText.text = _count.ToString();
        path = _path;
        field = FindObjectOfType<Field>();
        Move();
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

        //Скорость передвижения
        float speed = 0.5f;
        float duration = distance / speed;

        this.transform.DOPath(pathInVectors, duration, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear).SetId<Tween>("Moving").OnWaypointChange(field.WaypointChanged);
    }

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        path = new List<Cell>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (isMovingPaused)
            {
                DOTween.Play("Moving");
            }
            else
            {
                DOTween.Pause("Moving");
            }

            isMovingPaused = !isMovingPaused;
        }
    }
}
