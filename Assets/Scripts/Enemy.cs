using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public struct EnemyStats
{
    private bool lockMove;
    public int count { get; private set; }
    public Vector2Int position;

    public EnemyStats(XmlNode _node)
    {

        count = Convert.ToInt32(_node.Attributes["count"].InnerText);
        position = new Vector2Int(Convert.ToInt32(_node.Attributes["x"].InnerText) - 1, Convert.ToInt32(_node.Attributes["y"].InnerText) - 1);
        lockMove = Convert.ToBoolean(_node.Attributes["lock"].InnerText);
    }
}

public class Enemy : MonoBehaviour
{
    public EnemyStats enemyStats;
    private Text countText;
    public Vector2Int targetPosition;
    PathFinder pathfinder;
    List<Cell> path;
    Cell[,] cells;

    public void Initialize(EnemyStats _enemyStats)
    {
        enemyStats = _enemyStats;
        countText = GetComponentInChildren<Text>();
        countText.text = enemyStats.count.ToString();
        pathfinder = new PathFinder();
    }

    public void SetTargetPosition(Cell[,] _cells, Vector2Int _targetPosition)
    {
        cells = _cells;
        targetPosition = _targetPosition;
        path = pathfinder.WaveFind(_cells, enemyStats.position, targetPosition, Color.red).Item2;
        
        if (path.Count != 0)
        {
            DOTween.Kill("Pursuing");
            Pursuit();
        }
    }

    public void Pursuit()
    {
        if (path.Count != 0)
        {
            float distance = 0;
            Vector3[] pathInVectors = new Vector3[path.Count];
            pathInVectors[0] = this.gameObject.transform.position;

            for (int i = 1; i < path.Count; i++)
            {
                pathInVectors[i] = path[i].transform.position + new Vector3(0, 0, -1);
                distance += Vector3.Distance(pathInVectors[i], pathInVectors[i - 1]);
            }

            float speed = 0.5f;
            float duration = distance / speed;

            this.transform.DOPath(pathInVectors, duration, PathType.Linear, PathMode.TopDown2D).SetEase(Ease.Linear).SetId<Tween>("Pursuing").OnWaypointChange(WaypointChanged);
        }
    }

    public void WaypointChanged(int _waypointNumber)
    {
        if (_waypointNumber != 0)
        {
            Debug.Log($"enemy waypoint number: {_waypointNumber}");
            enemyStats.position = path[_waypointNumber].position;
        }
        //var result = pathfinder.WaveFind(cells, enemyStats.position, targetPosition);
        //DOTween.Kill("Pursuing");
        //Pursuit();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
