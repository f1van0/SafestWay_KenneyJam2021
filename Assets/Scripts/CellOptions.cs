using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using UnityEngine;

public enum CellType
{
    none = 0,
    startPoint,
    endPoint,
    impasse,
    line,
    turn,
    tRoad,
    cross,
}

public enum CellDirection
{
    Bottom = 0,
    Right,
    Top,
    Left
}

public struct CellState
{
    public CellDirection Direction => direction;
    public readonly CellType Type;
    public bool[] entrances { get; private set; }
    CellDirection direction;
    public bool lockRotation;

    public CellState(XmlNode _node)
    {
        Type = (CellType)Convert.ToInt32(_node.Attributes["type"].InnerText);
        entrances = GetEntrancesByType(Type);
        direction = CellDirection.Bottom;

        if (_node.Attributes["lock"] != null)
        {
            lockRotation = Convert.ToBoolean(_node.Attributes["lock"].InnerText);
        }
        else
        {
            if (Type == CellType.startPoint || Type == CellType.endPoint || Type == CellType.none)
                lockRotation = true;
            else
                lockRotation = false;
        }

        CellDirection neededDirection = (CellDirection)Convert.ToInt32(_node.Attributes["direction"].InnerText);
        Rotate(neededDirection);
    }

    public CellState(CellType _type, CellDirection _direction)
    {
        Type = _type;
        entrances = GetEntrancesByType(_type);
        direction = CellDirection.Bottom;

        if (_type == CellType.startPoint || _type == CellType.endPoint || _type == CellType.none)
            lockRotation = true;
        else
            lockRotation = false;
        
        Rotate(_direction);
    }

    public CellState(CellType _type, CellDirection _direction, bool _lockRotation)
    {
        Type = _type;
        entrances = GetEntrancesByType(_type);
        direction = CellDirection.Bottom;
        lockRotation = _lockRotation;
        Rotate(_direction);
    }

    public CellState Rotate(CellDirection lookupDirection)
    {
        /*
        int offset = (4 + (int)lookupDirection - (int)direction) % 4;
        bool[] temp = entrances.Skip(4 - offset).ToArray();

        for (int i = offset - 1; i >= 0; i--)
            entrances[i + 1] = entrances[i];

        for (int i = 0; i < temp.Length; i++)
            entrances[i] = temp[i];
        direction = lookupDirection;

        Debug.Log($"bottom: {entrances[0]}  right: {entrances[1]}  top: {entrances[2]}  left: {entrances[3]}");
        */

        while (direction != lookupDirection)
        {
            this = Rotate();
        }

        return this;
    }

    public CellState Rotate() //=> 
                              //this.Rotate((CellDirection)(((int)direction + 1) % 4));
    {
        bool exchange = entrances[3];
        entrances[3] = entrances[2];
        entrances[2] = entrances[1];
        entrances[1] = entrances[0];
        entrances[0] = exchange;
        direction = (CellDirection)(((int)direction + 1) % 4);
        
        return this;
    }

    public bool IsAvailable(CellDirection direction) =>
        entrances[(int)direction];

    public bool IsAvailable(int direction) =>
        entrances[direction];

    private static bool[] GetEntrancesByType(CellType _type)
    {
        switch (_type)
        {
            case CellType.cross: return new bool[] { true, true, true, true };
            case CellType.tRoad: return new bool[] { true, true, true, false };
            case CellType.line: return new bool[] { true, false, true, false };
            case CellType.turn: return new bool[] { true, true, false, false };
            case CellType.none: return new bool[] { false, false, false, false };
            case CellType.startPoint:
            case CellType.endPoint:
            case CellType.impasse:
                return new bool[] { true, false, false, false };
            default:
                return null;
        }
    }
}

/*
public static class CellOptions
{
    public static (bool, bool, bool, bool) GetCellPassage(CellType _type, int _rotation)
    {
        bool bottom = false,
            right = false,
            top = false,
            left = false;

        switch (_type)
        {
            case CellType.cross:
                {
                    bottom = true;
                    right = true;
                    top = true;
                    left = true;
                    break;
                }
            case CellType.none:
                {
                    break;
                }
            case CellType.turn:
                {
                    switch (_rotation)
                    {
                        case 0:
                            {
                                right = true;
                                bottom = true;
                                break;
                            }
                        case 1:
                            {
                                right = true;
                                top = true;
                                break;
                            }
                        case 2:
                            {
                                left = true;
                                top = true;
                                break;
                            }
                        default:
                            {
                                left = true;
                                bottom = true;
                                break;
                            }
                    }

                    break;
                }
            case CellType.line:
                {
                    if (_rotation % 2 == 0)
                    {
                        bottom = true;
                        top = true;
                    }
                    else
                    {
                        left = true;
                        right = true;
                    }

                    break;
                }
            case CellType.tRoad:
                {
                    switch (_rotation)
                    {
                        case 0:
                            {
                                top = true;
                                bottom = true;
                                right = true;
                                break;
                            }
                        case 1:
                            {
                                bottom = true;
                                top = true;
                                left = true;
                                break;
                            }
                        case 2:
                            {
                                left = true;
                                bottom = true;
                                right = true;
                                break;
                            }
                        default:
                            {
                                left = true;
                                bottom = true;
                                right = true;
                                break;
                            }
                    }

                    break;
                }
            default:
                {
                    switch (_rotation)
                    {
                        case 0:
                            {
                                bottom = true;
                                break;
                            }
                        case 1:
                            {
                                right = true;
                                break;
                            }
                        case 2:
                            {
                                top = true;
                                break;
                            }
                        default:
                            {
                                left = true;
                                break;
                            }
                    }

                    break;
                }
        }

        return (bottom, right, top, left);
    }
}
*/