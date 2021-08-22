using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Field : MonoBehaviour
{
    public static Field instance;

    private PathFinder pathfinder;
    private UserInterface userInterface;

    public GameObject heroPrefab;

    public GameObject enemyPrefab1;
    public GameObject enemyPrefab2;
    public GameObject enemyPrefab3;

    public GameObject crossPrefab;
    public GameObject impassePrefab;
    public GameObject turnPrefab;
    public GameObject linePrefab;
    public GameObject tRoadPrefab;
    public GameObject nonePrefab;
    public GameObject startPointPrefab;
    public GameObject endPointPrefab;
    public GameObject bridgePrefab;
    public GameObject riverPrefab;
    public GameObject castleWallsPrefab;

    public GameObject deathPrefab;

    public Cell[,] cells { get; private set; }
    private int sizeX;
    private int sizeY;

    public int currentLevel;

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

    public void CreateField()
    {
        switch(currentLevel)
        {
            case 1:
                userInterface.OpenTutorial1();
                break;
            case 2:
                userInterface.OpenTutorial2();
                break;
            case 3:
                userInterface.OpenTutorial3();
                break;
            case 4:
                userInterface.OpenTutorial4();
                break;
            case 5:
                userInterface.OpenTutorial5();
                break;
            case 6:
                userInterface.OpenTutorial6();
                break;
            default:
                break;
        }

        //levelsXml.Load("Assets\\scripts\\levels.xml");
        var xmlText = Resources.Load("LevelData/levels") as TextAsset;
        var xmlReader = new StringReader(xmlText.text);

        XmlDocument levelsXml = new XmlDocument();
        levelsXml.Load(xmlReader);

        XmlNode fieldNode = levelsXml.DocumentElement.SelectSingleNode($"/levels/level{currentLevel}/field");
        sizeX = Convert.ToInt32(fieldNode.Attributes["x"].InnerText);
        sizeY = Convert.ToInt32(fieldNode.Attributes["y"].InnerText);
        cells = new Cell[sizeX, sizeY];

        CellState cellState;
        heroPower = Convert.ToInt32(fieldNode.Attributes["power"].InnerText);
        userInterface.ChangePowerText(heroPower);

        this.transform.position = new Vector3(0, 0, 0);

        //Создание поля
        for (int j = 0; j < sizeY; j++)
        {
            for (int i = 0; i < sizeX; i++)
            {
                cellState = new CellState(levelsXml.DocumentElement.SelectSingleNode($"/levels/level{currentLevel}/field/line{j + 1}/cell{i + 1}"));
                GameObject currentCell = Instantiate(GetPrefabByType(cellState.Type), new Vector3(i - (float)sizeX / 2 + 0.5f, (float)j - sizeY / 2 + 0.5f, 0), new Quaternion(0, 0, 0, 0));
                //Debug.Log(currentCell.TryGetComponent<Cell>(out field[i, j]));
                cells[i, j] = currentCell.GetComponent<Cell>();
                cells[i, j].Initialize(cellState, new Vector2Int(i, j));
                cells[i, j].transform.SetParent(this.transform);
                cells[i, j].name += $"[{i}, {j}]";
            }
        }

        //Создание врагов на поле
        int enemiesCount = Convert.ToInt32(levelsXml.DocumentElement.SelectSingleNode($"/levels/level{currentLevel}/enemies").Attributes["count"].InnerText);
        for (int i = 0; i < enemiesCount; i++)
        {
            XmlNode enemyNode = levelsXml.DocumentElement.SelectSingleNode($"/levels/level{currentLevel}/enemies/enemy{i + 1}");
            EnemyStats enemyStats = new EnemyStats(enemyNode);
            
            Enemy enemy = Instantiate(GetPrefabByTeam(enemyStats.team), cells[enemyStats.position.x, enemyStats.position.y].gameObject.transform.position + new Vector3(0, 0, -1), new Quaternion(0, 0, 0, 0)).GetComponent<Enemy>();
            enemy.Initialize(enemyStats);
            enemies.Add(enemy);
        }
    }

    public GameObject GetPrefabByTeam(int team)
    {
        switch (team)
        {
            case 0:
                return enemyPrefab1;
            case 1:
                return enemyPrefab2;
            default:
                return enemyPrefab3;
        }
    }

    public void DestroyField()
    {
        for (int j = 0; j < sizeY; j++)
        {
            for (int i = 0; i < sizeX; i++)
            {
                Destroy(cells[i, j].gameObject);
            }
        }
        cells = new Cell[0, 0];

        foreach (Enemy enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        enemies = new List<Enemy>();

        if (hero != null)
        {
            hero.StopAllCoroutines();
            Destroy(hero.gameObject);
        }

        //TODO: destroy decorative things!
    }

    public void ToNextLevel()
    {
        if (cells != null)
            DestroyField();

        if (currentLevel < 7)
        {
            currentLevel++;
            CreateField();
            isRotationAvailable = true;
        }
        else
        {
            SceneManager.LoadScene("FinalScene");
        }
    }

    public void RestartLevel()
    {
        StopAllCoroutines();
        if (enemies != null)
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].StopAllCoroutines();
            }

        if (cells != null)
            DestroyField();

        CreateField();
        isRotationAvailable = true;
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
            case CellType.bridge:
                return bridgePrefab;
            case CellType.river:
                return riverPrefab;
            case CellType.castleWalls:
                return castleWallsPrefab;
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
            enemy.SetTargetPosition(cells, hero.currentCell);
        }
    }

    public void CheckForAttack()
    {
        for (int j = 0; j < enemies.Count; j++)
        {
            if (enemies[j].enemyStats.position == hero.currentCell.position)
            {
                hero.currentCell.isAttacking = true;
                StartCoroutine(Attack(hero, enemies[j]));
            }
            else
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (i != j && enemies[j].enemyStats.position == enemies[i].enemyStats.position && enemies[j].isAttacking == false && enemies[i].isAttacking == false)
                    {
                        if (enemies[j].enemyStats.team != enemies[i].enemyStats.team)
                        {
                            enemies[j].isAttacking = true;
                            enemies[i].isAttacking = true;
                            cells[enemies[j].enemyStats.position.x, enemies[j].enemyStats.position.y].isAttacking = true;
                            if (enemies[j].enemyStats.count > enemies[i].enemyStats.count)
                            {
                                StartCoroutine(Attack(enemies[j], enemies[i]));
                            }
                            else
                            {
                                StartCoroutine(Attack(enemies[i], enemies[j]));
                            }
                        }
                        else if (enemies[j].enemyStats.team == enemies[i].enemyStats.team)
                        {
                            StartCoroutine(Merge(enemies[j], enemies[i]));
                        }
                    }
                }
            }
        }
    }

    public IEnumerator Merge(Enemy teammate1, Enemy teammate2)
    {
        teammate1.StopAllCoroutines();
        teammate2.StopAllCoroutines();

        teammate2.enemyStats.count += teammate1.enemyStats.count;
        enemies.Remove(teammate1);
        Destroy(teammate1.gameObject);
        yield return new WaitForSeconds(0.1f);
    }

    public IEnumerator Attack(Hero hero, Enemy enemy)
    {
        hero.StopAllCoroutines();
        enemy.StopAllCoroutines();

        int heroDamage = hero.count;
        int enemyDamage = enemy.enemyStats.count;

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
        SoundManager.instance.PlayHit();
        hero.GetDamage(enemyDamage);
        for (int i = 0; i < 4; i++)
        {
            hero.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.05f);
            yield return new WaitForSeconds(0.05f);
            hero.GetComponent<SpriteRenderer>().DOColor(hero.color, 0.05f);
            yield return new WaitForSeconds(0.05f);
        }

        enemy.transform.DOMove(hero.currentCell.transform.position + new Vector3(0.25f, 0, -1), 0.1f);
        yield return new WaitForSeconds(0.2f);

        if (hero.count < 0)
        {
            hero.currentCell.isAttacking = false;
            //Death deathEffect = Instantiate(hero.transform).GetComponent<Death>();
            //deathEffect.Initialize(Color.cyan);
            SoundManager.instance.PlayLoseJingle();
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
            SoundManager.instance.PlayHit();
            enemy.GetDamage(heroDamage);
            for (int i = 0; i < 4; i++)
            {
                enemy.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.05f);
                yield return new WaitForSeconds(0.05f);
                enemy.GetComponent<SpriteRenderer>().DOColor(enemy.color, 0.05f);
                yield return new WaitForSeconds(0.05f);
            }

            hero.transform.DOMove(hero.currentCell.transform.position + new Vector3(-0.25f, 0, -1), 0.1f);
            yield return new WaitForSeconds(0.2f);

            hero.currentCell.isAttacking = false;
            var result = pathfinder.WaveFind(cells, hero.currentCell.position, Color.green);
            path = result.Item2;
            hero.StartCoroutine(hero.Movement(path));
            //Death deathEffect = Instantiate(deathPrefab, enemy.transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Death>();
            //deathEffect.transform.position = enemy.transform.position;
            //deathEffect.Initialize(Color.red);
            enemies.Remove(enemy);
            Destroy(enemy.gameObject);
        }
    }

    public IEnumerator Attack(Enemy enemy1, Enemy enemy2)
    {
        enemy2.StopAllCoroutines();
        enemy1.StopAllCoroutines();

        int enemy1Damage = enemy1.enemyStats.count;
        int enemy2Damage = enemy2.enemyStats.count;

        Cell cell = cells[enemy1.enemyStats.position.x, enemy1.enemyStats.position.y];
        //Take places to fight
        enemy1.transform.DOMove(cell.transform.position + new Vector3(-0.25f, 0, -1), 0.15f);
        enemy2.transform.DOMove(cell.transform.position + new Vector3(0.25f, 0, -1), 0.15f);
        yield return new WaitForSeconds(0.2f);

        //Enemy2 is attacking enemy1
        enemy2.transform.DOMove(cell.transform.position + new Vector3(0f, 0, -1), 0.1f);
        yield return new WaitForSeconds(0.1f);
        enemy2.transform.DOMove(cell.transform.position + new Vector3(-0.25f, 0, -1), 0.2f);
        yield return new WaitForSeconds(0.1f);
        //enemy1 took damage and started blinking 4 times
        enemy1.transform.DOMove(cell.transform.position + new Vector3(-0.3f, 0, -1), 0.2f);
        SoundManager.instance.PlayHit();
        enemy1.GetDamage(enemy2Damage);
        for (int i = 0; i < 4; i++)
        {
            enemy1.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.05f);
            yield return new WaitForSeconds(0.05f);
            enemy1.GetComponent<SpriteRenderer>().DOColor(enemy1.color, 0.05f);
            yield return new WaitForSeconds(0.05f);
        }

        enemy1.transform.DOMove(cell.transform.position + new Vector3(0.25f, 0, -1), 0.1f);
        yield return new WaitForSeconds(0.2f);

        if (enemy1.enemyStats.count < 0)
        {
            //SoundManager.instance.PlayLoseJingle();
            //userInterface.Lose();
            enemy2.Pursuit(cells, hero.currentCell);
            enemies.Remove(enemy1);
            Destroy(enemy1.gameObject);
        }
        else
        {
            //Enemy1 is attacking enemy2
            enemy1.transform.DOMove(cell.transform.position + new Vector3(0f, 0, -1), 0.1f);
            yield return new WaitForSeconds(0.1f);
            enemy1.transform.DOMove(cell.transform.position + new Vector3(0.25f, 0, -1), 0.2f);
            yield return new WaitForSeconds(0.1f);
            //Enemy2 took damage and started blinking 4 times
            enemy2.transform.DOMove(cell.transform.position + new Vector3(0.3f, 0, -1), 0.2f);
            SoundManager.instance.PlayHit();
            enemy2.GetDamage(enemy1Damage);
            for (int i = 0; i < 4; i++)
            {
                enemy2.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.05f);
                yield return new WaitForSeconds(0.05f);
                enemy2.GetComponent<SpriteRenderer>().DOColor(enemy2.color, 0.05f);
                yield return new WaitForSeconds(0.05f);
            }

            enemy1.transform.DOMove(cell.transform.position + new Vector3(-0.25f, 0, -1), 0.1f);
            yield return new WaitForSeconds(0.2f);


            enemy1.Pursuit(cells, hero.currentCell);
            //Death deathEffect = Instantiate(deathPrefab, enemy2.transform.position, new Quaternion(0, 0, 0, 0)).GetComponent<Death>();
            //deathEffect.transform.position = enemy2.transform.position;
            //deathEffect.Initialize(Color.red);
            enemies.Remove(enemy2);
            Destroy(enemy2.gameObject);
        }

        enemy1.isAttacking = false;
        enemy2.isAttacking = false;
        cell.isAttacking = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        path = new List<Cell>();
        pathfinder = new PathFinder();
        userInterface = FindObjectOfType<UserInterface>();
        isRotationAvailable = true;
        CreateField();
    }

    public void CheckForWin()
    {
        StopAllCoroutines();
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].StopAllCoroutines();
        }
        hero.StopAllCoroutines();

        SoundManager.instance.PlayWinJingle();
        userInterface.Win();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
