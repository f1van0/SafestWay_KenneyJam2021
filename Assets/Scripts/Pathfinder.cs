using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    /*
    public (bool, List<Cell>) FindPath(Cell[,] _cells)
    {
        List<Cell> _path = new List<Cell>();
        _path.Add(FindStartPoint(_cells));
        Debug.Log("___Start___");
        (bool isFound, List<Cell> path) result = FindPath(_cells, _path, CellDirection.Top);
        return result;
    }
    */

    public (bool, List<Cell>) WaveFind(Cell[,] _cells, Vector2Int _startPoint)
    {
        return (WaveFind(_cells, _startPoint, new Vector2Int(-1, -1), Color.white));
    }

    public (bool, List<Cell>) WaveFind(Cell[,] _cells, Vector2Int _startPoint, Color _color)
    {
        return (WaveFind(_cells, _startPoint, new Vector2Int(-1, -1), _color));
    }

    public (bool, List<Cell>) WaveFind(Cell[,] _cells, Vector2Int _startPoint, Vector2Int _endPoint)
    {
        return (WaveFind(_cells, _startPoint, new Vector2Int(-1, -1), Color.white));
    }

    public (bool, List<Cell>) WaveFind(Cell[,] _cells, Vector2Int _startPoint, Vector2Int _endPoint, Color _color)
    {
        (int distance, Cell cell)[,] dataCells = new (int, Cell)[_cells.GetLength(0), _cells.GetLength(1)];

        for (int j = 0; j < _cells.GetLength(1); j++)
        {
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                dataCells[i, j].distance = -1;
                dataCells[i, j].cell = _cells[i, j];
            }
        }

        dataCells[_startPoint.x, _startPoint.y].distance = 0;

        //dataCells[_startPoint.x, _startPoint.y].cell.ShowDebugDistance(dataCells[_startPoint.x, _startPoint.y].distance, _color);

        bool whileNotFinished = true;
        bool newCellsAvailable = true;

        List<(int distance, Cell cell)> currentCells = TrySpreadWave(dataCells, _startPoint, _color);
        List<(int distance, Cell cell)> nextCells;
        Vector2Int endPointPosition = new Vector2Int(-1, -1);

        while (whileNotFinished && newCellsAvailable)
        {
            nextCells = new List<(int distance, Cell cell)>();

            foreach (var element in currentCells)
            {
                if (element.cell.cellState.Type == CellType.endPoint || element.cell.position == _endPoint)
                {
                    whileNotFinished = false;
                    endPointPosition = element.cell.position;
                    break;
                }
                else
                {
                    nextCells.AddRange(TrySpreadWave(dataCells, element.cell.position, _color));
                }
            }

            if (whileNotFinished == false)
            {
                break;
            }

            if (nextCells.Count != 0)
            {
                currentCells = nextCells;
            }
            else
            {
                newCellsAvailable = false;
                break;
            }
        }

        if (whileNotFinished == true)
        {
            return (false, new List<Cell>());
        }
        else
        {
            List<Cell> path = CreatePath(dataCells, endPointPosition);
            return (true, path);
        }
    }

    private List<Cell> CreatePath((int distance, Cell cell)[,] _dataCells, Vector2Int endPointPosition)
    {
        (int distance, Cell cell) currentDataCell = _dataCells[endPointPosition.x, endPointPosition.y];
        List<Cell> path = new List<Cell>();
        path.Add(currentDataCell.cell);
        Vector2Int currentCellPosition;
        Vector2Int nextCellPosition;
        CellDirection direction;

        while (currentDataCell.distance != 0)
        {
            currentCellPosition = currentDataCell.cell.position;

            for (int i = 0; i < 4; i++)
            {
                direction = (CellDirection)i;
                if (CheckLimits(_dataCells, currentCellPosition, direction))
                {
                    nextCellPosition = currentCellPosition + GetVector2IntFromCellDirection((CellDirection)i);
                    if (_dataCells[currentCellPosition.x, currentCellPosition.y].distance == _dataCells[nextCellPosition.x, nextCellPosition.y].distance + 1 && isCellsConverge(currentCellPosition, nextCellPosition, _dataCells))
                    {
                        currentDataCell = _dataCells[nextCellPosition.x, nextCellPosition.y];
                        path.Add(currentDataCell.cell);
                        break;
                    }
                }
            }
        }

        path.Reverse();
        return path;
    }

    public List<(int distance, Cell cell)> TrySpreadWave((int distance, Cell cell)[,] _dataCells, Vector2Int _currentCell, Color _color)
    {
        Vector2Int nextPosition;
        List<(int distance, Cell cell)> wavedCells = new List<(int distance, Cell cell)>();
        CellDirection direction;
        for (int i = 0; i < 4; i++)
        {
            direction = (CellDirection)i;
            if (isCellsAvailableInDirection(_dataCells, _currentCell, direction))
            {
                nextPosition = _currentCell + GetVector2IntFromCellDirection(direction);
                _dataCells[nextPosition.x, nextPosition.y].distance = _dataCells[_currentCell.x, _currentCell.y].distance + 1;
                //_dataCells[nextPosition.x, nextPosition.y].cell.ShowDebugDistance(_dataCells[nextPosition.x, nextPosition.y].distance, _color);
                wavedCells.Add(_dataCells[nextPosition.x, nextPosition.y]);
            }
        }

        /*
        if (isCellsAvailableInDirection(_dataCells, _currentCell, CellDirection.Bottom))
        {
            nextPosition = _currentCell + Vector2Int.down;
            _dataCells[nextPosition.x, nextPosition.y].distance = _dataCells[_currentCell.x, _currentCell.y].distance + 1;
            wavedCells.Add(_dataCells[nextPosition.x, nextPosition.y]);
        }
        else if (isCellsAvailableInDirection(_dataCells, _currentCell, CellDirection.Right))
        {
            nextPosition = _currentCell + Vector2Int.right;
            _dataCells[nextPosition.x, nextPosition.y].distance = _dataCells[_currentCell.x, _currentCell.y].distance + 1;
            wavedCells.Add(_dataCells[nextPosition.x, nextPosition.y]);
        }
        else if (isCellsAvailableInDirection(_dataCells, _currentCell, CellDirection.Top))
        {
            nextPosition = _currentCell + Vector2Int.up;
            _dataCells[nextPosition.x, nextPosition.y].distance = _dataCells[_currentCell.x, _currentCell.y].distance + 1;
            wavedCells.Add(_dataCells[nextPosition.x, nextPosition.y]);
        }
        else if (isCellsAvailableInDirection(_dataCells, _currentCell, CellDirection.Left))
        {
            nextPosition = _currentCell + Vector2Int.left;
            _dataCells[nextPosition.x, nextPosition.y].distance = _dataCells[_currentCell.x, _currentCell.y].distance + 1;
            wavedCells.Add(_dataCells[nextPosition.x, nextPosition.y]);
        }
        */

        return wavedCells;
    }

    /*
    public (bool, List<Cell>) FindPath(Cell[,] _cells, List<Cell> _path, CellDirection _previousDirection)
    {
        if (_path[_path.Count - 1].cellState.Type == CellType.endPoint)
        {
            return (true, _path);
        }
        else
        {
            Cell lastItemInList = _path[_path.Count - 1];
            Debug.Log($"current cell: ({lastItemInList.position.x}, {lastItemInList.position.y}) from direction - {_previousDirection}");
            (bool isFound, List<Cell> path) bottomResults;
            (bool isFound, List<Cell> path) rightResults;
            (bool isFound, List<Cell> path) topResults;
            (bool isFound, List<Cell> path) leftResults;

            bottomResults.path = rightResults.path = topResults.path = leftResults.path = _path;

            int minPathLength = _cells.GetLength(0) * _cells.GetLength(1) + 1;
            List<Cell> minPath = null;

            //≈сли прошлый шаг был не вверх и снизу есть проход
            if (_previousDirection != CellDirection.Top && lastItemInList.cellState.IsAvailable(CellDirection.Bottom))
            {
                if (lastItemInList.position.y > 0 && isCellsSuits(bottomResults.path, lastItemInList, _cells[lastItemInList.position.x, lastItemInList.position.y - 1]))
                {
                    bottomResults.path.Add(_cells[lastItemInList.position.x, lastItemInList.position.y - 1]);
                    bottomResults = FindPath(_cells, bottomResults.path, CellDirection.Bottom);

                    if (bottomResults.isFound == true && minPathLength > bottomResults.path.Count)
                    {
                        minPath = bottomResults.path;
                        minPathLength = minPath.Count;
                    }
                }
            }

            if (_previousDirection != CellDirection.Left && lastItemInList.cellState.IsAvailable(CellDirection.Right))
            {
                if (lastItemInList.position.x < _cells.GetLength(0) - 1 && isCellsSuits(rightResults.path, lastItemInList, _cells[lastItemInList.position.x + 1, lastItemInList.position.y]))
                {
                    rightResults.path.Add(_cells[lastItemInList.position.x + 1, lastItemInList.position.y]);
                    rightResults = FindPath(_cells, rightResults.path, CellDirection.Right);

                    if (rightResults.isFound == true && minPathLength > rightResults.path.Count)
                    {
                        minPath = rightResults.path;
                        minPathLength = minPath.Count;
                    }
                }
            }

            if (_previousDirection != CellDirection.Bottom && lastItemInList.cellState.IsAvailable(CellDirection.Top))
            {
                if (lastItemInList.position.y < _cells.GetLength(1) - 1 && isCellsSuits(topResults.path, lastItemInList, _cells[lastItemInList.position.x, lastItemInList.position.y + 1])) 
                {
                    topResults.path.Add(_cells[lastItemInList.position.x, lastItemInList.position.y + 1]);
                    topResults = FindPath(_cells, topResults.path, CellDirection.Top);

                    if (topResults.isFound == true && minPathLength > topResults.path.Count)
                    {
                        minPath = topResults.path;
                        minPathLength = minPath.Count;
                    }
                }
            }

            if (_previousDirection != CellDirection.Right && lastItemInList.cellState.IsAvailable(CellDirection.Left))
            {
                if (lastItemInList.position.x > 0 && isCellsSuits(leftResults.path, lastItemInList, _cells[lastItemInList.position.x - 1, lastItemInList.position.y]))
                {
                    leftResults.path.Add(_cells[lastItemInList.position.x - 1, lastItemInList.position.y]);
                    leftResults = FindPath(_cells, leftResults.path, CellDirection.Left);

                    if (leftResults.isFound == true && minPathLength > leftResults.path.Count)
                    {
                        minPath = leftResults.path;
                        minPathLength = minPath.Count;
                    }
                }
            }

            if (minPath == null)
            {
                return (false, minPath);
            }
            else
            {
                return (true, minPath);
            }
        }
    }
    */
    private bool CheckLimits((int distance, Cell cell)[,] _dataCells, Vector2Int _currentCell, CellDirection _direction)
    {
        if (_direction == CellDirection.Bottom)
        {
            if (_currentCell.y > 0)
                return true;
            else
                return false;
        }
        else if (_direction == CellDirection.Top)
        {
            if (_currentCell.y < _dataCells.GetLength(1) - 1)
                return true;
            else
                return false;
        }
        else if (_direction == CellDirection.Left)
        {
            if (_currentCell.x > 0)
                return true;
            else
                return false;
        }
        else
        {
            if (_currentCell.x < _dataCells.GetLength(0) - 1)
                return true;
            else
                return false;
        }
    }

    private bool isCellsAvailableInDirection((int distance, Cell cell)[,] _dataCells, Vector2Int _currentCell, CellDirection _direction)
    {
        Vector2Int vectorDirection = GetVector2IntFromCellDirection(_direction);
        Vector2Int nextPosition = _currentCell + vectorDirection;
        //≈сли направление движени€ вниз и мы не вышли за пределы массива и клетки соединены вместе и следующа€ клетка еще не была помечена числом
        if (CheckLimits(_dataCells, _currentCell, _direction) && _dataCells[nextPosition.x, nextPosition.y].distance == -1 && isCellsConverge(_dataCells, _currentCell, vectorDirection))
            return true;
        else
            return false;
    }

    /*
    private bool isCellsSuits(List<Cell> _path, Cell _from, Cell _to)
    {
        bool isNotFound = true;

        for (int i = 0; i < _path.Count; i++)
        {
            if (_path[i].position == _to.position)
                isNotFound = false;
        }

        return (isCellsConverge(_from, _to) && isNotFound);
    }
    */

    private Vector2Int GetVector2IntFromCellDirection(CellDirection _direction)
    {
        if (_direction == CellDirection.Bottom)
            return new Vector2Int(0, -1);
        else if (_direction == CellDirection.Top)
            return new Vector2Int(0, 1);
        else if (_direction == CellDirection.Left)
            return new Vector2Int(-1, 0);
        else
            return new Vector2Int(1, 0);
    }

    public bool isCellsConverge((int distance, Cell cell)[,] _dataCells, Vector2Int _currentCell, Vector2Int _vectorDirection)
    {
        Cell from = _dataCells[_currentCell.x, _currentCell.y].cell;
        Vector2Int _toPosition = _currentCell + _vectorDirection;
        Cell to = _dataCells[_toPosition.x, _toPosition.y].cell;

        if (_vectorDirection.y != 0)
        {
            return (from.cellState.IsAvailable(1 + _vectorDirection.y) == true && to.cellState.IsAvailable(1 - _vectorDirection.y) == true);
        }
        else if (_vectorDirection.x != 0)
        {
            return (from.cellState.IsAvailable(2 - _vectorDirection.x) == true && to.cellState.IsAvailable(2 + _vectorDirection.x) == true);
        }
        else
        {
            return false;
        }
    }

    public bool isCellsConverge(Vector2Int _fromVector, Vector2Int _toVector, (int distance, Cell cell)[,] _dataCells)
    {
        Vector2Int offset = _toVector - _fromVector;
        Cell _fromCell = _dataCells[_fromVector.x, _fromVector.y].cell;
        Cell _toCell = _dataCells[_toVector.x, _toVector.y].cell;
        if (offset.y != 0)
        {
            return (_fromCell.cellState.IsAvailable(1 + offset.y) == true && _toCell.cellState.IsAvailable(1 - offset.y) == true);
        }
        else if (offset.x != 0)
        {
            return (_fromCell.cellState.IsAvailable(2 - offset.x) == true && _toCell.cellState.IsAvailable(2 + offset.x) == true);
        }
        else
        {
            return false;
        }
    }

    /*
    public bool isCellsConverge(Cell _from, Cell _to)
    {
        Vector2Int offset = _to.position - _from.position;
        if (offset.y != 0)
        {
            return (_from.cellState.IsAvailable(1 + offset.y) == true && _to.cellState.IsAvailable(1 - offset.y) == true);
        }
        else if (offset.x != 0)
        {
            return (_from.cellState.IsAvailable(2 - offset.x) == true && _to.cellState.IsAvailable(2 + offset.x) == true);
        }
        else
        {
            return false;
        }
    }
    */
    /*
    public Cell FindStartPoint(Cell[,] _field)
    {
        for (int j = 0; j < _field.GetLength(1); j++)
        {
            for (int i = 0; i < _field.GetLength(0); i++)
            {
                if (_field[i, j].cellState.Type == CellType.startPoint)
                    return _field[i, j];
            }
        }

        return null;
    }
    */
}
