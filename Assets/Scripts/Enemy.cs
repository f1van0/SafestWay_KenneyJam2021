using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public struct EnemyStats
{
    public bool lockMove;
    public int count;
    public float speed;
    public Vector2Int position;
    public int team;

    public EnemyStats(XmlNode _node)
    {
        speed = 0.7f;
        count = Convert.ToInt32(_node.Attributes["count"].InnerText);
        position = new Vector2Int(Convert.ToInt32(_node.Attributes["x"].InnerText) - 1, Convert.ToInt32(_node.Attributes["y"].InnerText) - 1);
        lockMove = Convert.ToBoolean(_node.Attributes["lock"].InnerText);
        team = Convert.ToInt32(_node.Attributes["team"].InnerText);
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
    public bool isAttacking;
    public Color color;

    public void GetDamage(int _damage)
    {
        enemyStats.count -= _damage;
        countText.text = enemyStats.count.ToString();
    }

    public void Initialize(EnemyStats _enemyStats)
    {
        enemyStats = _enemyStats;
        countText = GetComponentInChildren<Text>();
        countText.text = enemyStats.count.ToString();
        pathfinder = new PathFinder();
        isAttacking = false;
        color = this.GetComponent<SpriteRenderer>().color;
    }

    public void SetTargetPosition(Cell[,] _cells, Cell _currentCell)
    {
        if (path != null && enemyStats.lockMove == true)
        {
            if (path.Contains(_currentCell) && path.IndexOf(_currentCell) < path.Count - 1)
            {
                path.RemoveAt(path.IndexOf(_currentCell) + 1);
            }
            else if (!path.Contains(_currentCell))
            {
                path.Add(_currentCell);
            }
        }

        //var result = pathfinder.WaveFind(_cells, enemyStats.position, _currentCell.position, Color.red);
    }

    public IEnumerator Pursuiting()
    {
        while (path.Count > 1)
        {
            yield return new WaitForSeconds(enemyStats.speed / 1.5f);
            if (Field.instance.GetHero().nextCell == path[0])
            {
                yield return new WaitForSeconds(enemyStats.speed * 2f);
            }
            else if (path.Count > 2 && path[1].isAttacking == true)
            {
                yield return new WaitForSeconds(0.3f);
            }
            else
            {
                path.RemoveAt(0);
                this.transform.DOMove(path[0].transform.position + new Vector3(0, 0, -1), enemyStats.speed / 4);
                yield return new WaitForSeconds(enemyStats.speed/2);
                SoundManager.instance.PlayFootstep();
                enemyStats.position = path[0].position;
                Field.instance.CheckForAttack();
            }
        }
    }

    public void Pursuit(Cell[,] _cells, Cell _heroPos)
    {
        if (enemyStats.lockMove == false)
        {
            cells = _cells;
            path = pathfinder.WaveFind(_cells, enemyStats.position, _heroPos.position).Item2;
            if (path.Count != 0)
            {
                StartCoroutine(Pursuiting());
            }
        }
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
