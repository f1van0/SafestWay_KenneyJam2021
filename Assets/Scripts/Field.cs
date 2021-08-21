using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using UnityEngine;

public class Field : MonoBehaviour
{
    public static Field instance;

    private PathFinder pathfinder;
    private UserInterface userInterface;

    public GameObject heroPrefab;
    public GameObject enemyPrefab;

    public GameObject crossPrefab;
    public GameObject impassePrefab;
    public GameObject turnPrefab;
    public GameObject linePrefab;
    public GameObject tRoadPrefab;
    public GameObject nonePrefab;
    public GameObject startPointPrefab;
    public GameObject endPointPrefab;

    public Cell[,] cells { get; private set; }
    private int sizeX;
    private int sizeY;

    public List<Enemy> enemies;
    public List<Cell> path { get; private set; }
    private Hero hero;
    private int heroPower;

    private bool isRotationAvailable;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            Debug.Log("Field is already exists!");
        }
        else
        {
            instance = this.GetComponent<Field>();
        }
    }

    public Hero GetHero()
    {
        return hero;
    }

    public void CreateField(int _currentLevel)
    {
        XmlDocument levelsXml = new XmlDocument();
        levelsXml.Load("Assets\\scripts\\levels.xml");

        XmlNode fieldNode = levelsXml.DocumentElement.SelectSingleNode("/level1/field");
        sizeX = Convert.ToInt32(fieldNode.Attributes["x"].InnerText);
        sizeY = Convert.ToInt32(fieldNode.Attributes["y"].InnerText);
        cells = new Cell[sizeX, sizeY];

        CellState cellState;
        heroPower = Convert.ToInt32(fieldNode.Attributes["power"].InnerText);

        //Создание поля
        for (int j = 0; j < sizeY; j++)
        {
            for (int i = 0; i < sizeX; i++)
            {
                cellState = new CellState(levelsXml.DocumentElement.SelectSingleNode($"/level1/field/line{j + 1}/cell{i + 1}"));
                GameObject currentCell = Instantiate(GetPrefabByType(cellState.Type), new Vector3(i - sizeX / 2, j - sizeX / 2, 0), new Quaternion(0, 0, 0, 0));
                //Debug.Log(currentCell.TryGetComponent<Cell>(out field[i, j]));
                cells[i, j] = currentCell.GetComponent<Cell>();
                cells[i, j].Initialize(cellState, new Vector2Int(i, j));
                cells[i, j].transform.SetParent(this.transform);
                cells[i, j].name += $"[{i}, {j}]";
            }
        }

        //Создание врагов на поле
        int enemiesCount = Convert.ToInt32(levelsXml.DocumentElement.SelectSingleNode("/level1/enemies").Attributes["count"].InnerText);
        for (int i = 0; i < enemiesCount; i++)
        {
            XmlNode enemyNode = levelsXml.DocumentElement.SelectSingleNode($"/level1/enemies/enemy{i + 1}");
            EnemyStats enemyStats = new EnemyStats(enemyNode);
            Enemy enemy = Instantiate(enemyPrefab, cells[enemyStats.position.x, enemyStats.position.y].gameObject.transform.position + new Vector3(0, 0, -1), new Quaternion(0, 0, 0, 0)).GetComponent<Enemy>();
            enemy.Initialize(enemyStats);
            enemies.Add(enemy);
        }
    }

    public GameObject GetPrefabByType(CellType _type)
    {
        switch(_type)
        {
            case CellType.cross:
                return crossPrefab;
            case CellType.impasse:
                return impassePrefab;
            case CellType.turn:
                return turnPrefab;
            case CellType.line:
                return linePrefab;
            case CellType.tRoad:
                return tRoadPrefab;
            case CellType.startPoint:
                return startPointPrefab;
            case CellType.endPoint:
                return endPointPrefab;
            default:
                return nonePrefab;
        }
    }

    public void Rotate(Cell _cell)
    {
        if (isRotationAvailable)
        {
            isRotationAvailable = false;

            Vector2Int _cellPosition = _cell.position;
            cells[_cellPosition.x, _cellPosition.y].Rotate();
            //cells[_cellPosition.x, _cellPosition.y].ShowDebugDistance(-1);
            var result = pathfinder.WaveFind(cells, FindStartPoint().position, Color.green);
            path = result.Item2;
            userInterface.SetInteractiveToStartJourneyButton(result.Item1);

            isRotationAvailable = true;
        }
    }

    public Cell FindStartPoint()
    {
        for (int j = 0; j < cells.GetLength(1); j++)
        {
            for (int i = 0; i < cells.GetLength(0); i++)
            {
                if (cells[i, j].cellState.Type == CellType.startPoint)
                    return cells[i, j];
            }
        }

        return null;
    }

    public void StartJourney()
    {
        hero = Instantiate(heroPrefab, path[0].transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Hero>();
        hero.Initialize(heroPower, path);
        foreach (var enemy in enemies)
        {
            enemy.Pursuit(cells, path[0]);
        }
        isRotationAvailable = false;
        userInterface.SetInteractiveToStartJourneyButton(false);
    }

    public void HeroCellChanged()
    {
        Debug.Log("changed");
        CheckForAttack();
        foreach (var enemy in enemies)
        {
            enemy.SetTargetPosition(hero.currentCell);
        }
    }

    public void CheckForAttack()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.enemyStats.position == hero.currentCell.position)
            {
                StartCoroutine(Attack(hero, enemy));
            }
        }
    }

    public IEnumerator Attack(Hero hero, Enemy enemy)
    {
        hero.StopAllCoroutines();
        enemy.StopAllCoroutines();
        //Take places to fight
        hero.transform.DOMove(hero.currentCell.transform.position + new Vector3(-0.25f, 0, -1), 0.15f);
        enemy.transform.DOMove(hero.currentCell.transform.position + new Vector3(0.25f, 0, -1), 0.15f);
        yield return new WaitForSeconds(0.2f);

        //Enemy is attacking hero
        enemy.transform.DOMove(hero.currentCell.transform.position + new Vector3(0f, 0, -1), 0.1f);
        yield return new WaitForSeconds(0.1f);
        enemy.transform.DOMove(hero.currentCell.transform.position + new Vector3(-0.25f, 0, -1), 0.2f);
        yield return new WaitForSeconds(0.1f);
        //Hero took damage and started blinking 4 times
        hero.transform.DOMove(hero.currentCell.transform.position + new Vector3(-0.3f, 0, -1), 0.2f);
        hero.GetDamage(enemy.enemyStats.count);
        for (int i = 0; i < 4; i++)
        {
            hero.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.05f);
            yield return new WaitForSeconds(0.05f);
            hero.GetComponent<SpriteRenderer>().DOColor(Color.cyan, 0.05f);
            yield return new WaitForSeconds(0.05f);
        }

        enemy.transform.DOMove(hero.currentCell.transform.position + new Vector3(0.25f, 0, -1), 0.1f);
        yield return new WaitForSeconds(0.2f);

        if (hero.count < 0)
        {
            userInterface.Lose();
        }

        else
        {
            //Hero is attacking enemy
            hero.transform.DOMove(hero.currentCell.transform.position + new Vector3(0f, 0, -1), 0.1f);
            yield return new WaitForSeconds(0.1f);
            hero.transform.DOMove(hero.currentCell.transform.position + new Vector3(0.25f, 0, -1), 0.2f);
            yield return new WaitForSeconds(0.1f);
            //Hero took damage and started blinking 4 times
            enemy.transform.DOMove(hero.currentCell.transform.position + new Vector3(0.3f, 0, -1), 0.2f);
            enemy.GetDamage(enemy.enemyStats.count);
            for (int i = 0; i < 4; i++)
            {
                enemy.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.05f);
                yield return new WaitForSeconds(0.05f);
                enemy.GetComponent<SpriteRenderer>().DOColor(Color.red, 0.05f);
                yield return new WaitForSeconds(0.05f);
            }

            hero.transform.DOMove(hero.currentCell.transform.position + new Vector3(-0.25f, 0, -1), 0.1f);
            yield return new WaitForSeconds(0.2f);


            var result = pathfinder.WaveFind(cells, hero.currentCell.position, Color.green);
            path = result.Item2;
            hero.StartCoroutine(hero.Movement(path));
            Destroy(enemy);
        }
    }
                /*
                public Cell GetCellWithHero()
                {
                    Vector2 mostNearPosition;

                }
                */

                // Start is called before the first frame update
    void Start()
    {
        path = new List<Cell>();
        pathfinder = new PathFinder();
        userInterface = FindObjectOfType<UserInterface>();
        isRotationAvailable = true;
        CreateField(1);
    }

    public void CheckForWin()
    {
        userInterface.Win();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
