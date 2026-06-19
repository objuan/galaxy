
using System;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

/*
[ExecuteInEditMode]
public class EnemyPathConfig : MonoBehaviour
{
    Dictionary<string, EnemyPathDef> maps = new();
    List<EnemyPathDef> list = new();    

    private void Awake()
    {
        Add(empty());
        Add(left_spiral());
        Add(right_spiral());
        Add(wide_s());
        Add(dive_attack());
        Add(boss_escort());
        Add(figure_eight());

    }

    void Add(EnemyPathDef def)
    {
        maps[def.id] = def;
        list.Add(def);
    }
    public EnemyPathDef GetByID(string name)
    {
        return maps[name];
    }

    public EnemyPathDef[] GetAll()
    {
        return list.ToArray();
    }
    public static  EnemyPathDef empty()
    {
        return new EnemyPathDef
        {
            id = "",
            points = { }
        };
      }
    public static EnemyPathDef left_spiral()
    {
        return new EnemyPathDef
        {
            id = "left_spiral",
            points =
                {
                    new(){ pos = new Vector2(-8,10), time = 0.0f },
                    new(){ pos = new Vector2(-6, 9), time = 0.3f },
                    new(){ pos = new Vector2(-3, 6), time = 0.8f },
                    new(){ pos = new Vector2(-5, 2), time = 1.3f },
                    new(){ pos = new Vector2(-2, 3), time = 1.8f },
                    new(){ pos = new Vector2( 0, 0), time = 2.5f }
                }
        };
    }
    public static EnemyPathDef right_spiral()
    {
        return new EnemyPathDef
        {
            id = "right_spiral",
            points =
    {
        new(){ pos = new Vector2( 8,10), time = 0.0f },
        new(){ pos = new Vector2( 6, 9), time = 0.3f },
        new(){ pos = new Vector2( 3, 6), time = 0.8f },
        new(){ pos = new Vector2( 5, 2), time = 1.3f },
        new(){ pos = new Vector2( 2, 3), time = 1.8f },
        new(){ pos = new Vector2( 0, 0), time = 2.5f }
    }
        };
    }

    public static EnemyPathDef wide_s()
    {
        return new EnemyPathDef
        {
            id = "wide_s",
            points =
    {
        new(){ pos = new Vector2(-8,10), time = 0.0f },
        new(){ pos = new Vector2(-5, 8), time = 0.5f },
        new(){ pos = new Vector2( 2, 5), time = 1.0f },
        new(){ pos = new Vector2(-2, 2), time = 1.5f },
        new(){ pos = new Vector2( 0, 0), time = 2.0f }
    }
        };
    }

    public static EnemyPathDef dive_attack()
    {
        return new EnemyPathDef
        {
            id = "dive_attack",
            points =
            {
                new(){ pos = new Vector2( 0, 0), time = 0.0f },
                new(){ pos = new Vector2(-2,-2), time = 0.3f },
                new(){ pos = new Vector2( 1,-5), time = 0.8f },
                new(){ pos = new Vector2( 0,-9), time = 1.5f }
            }
        };
    }

    public static EnemyPathDef boss_escort()
    {
        return new EnemyPathDef
        {
            id = "boss_escort",
            points =
    {
        new(){ pos = new Vector2( 0, 0), time = 0.0f },
        new(){ pos = new Vector2( 3,-2), time = 0.5f },
        new(){ pos = new Vector2(-3,-4), time = 1.2f },
        new(){ pos = new Vector2( 0,-8), time = 2.0f }
    }
        };
    }

    public static EnemyPathDef figure_eight()
    {
        return new EnemyPathDef
        {
            id = "figure_eight",
            points =
    {
        new(){ pos = new Vector2( 0,10), time = 0.0f },
        new(){ pos = new Vector2(-4, 7), time = 0.5f },
        new(){ pos = new Vector2( 0, 4), time = 1.0f },
        new(){ pos = new Vector2( 4, 7), time = 1.5f },
        new(){ pos = new Vector2( 0,10), time = 2.0f },
        new(){ pos = new Vector2(-4,13), time = 2.5f },
        new(){ pos = new Vector2( 0,16), time = 3.0f },
        new(){ pos = new Vector2( 4,13), time = 3.5f },
        new(){ pos = new Vector2( 0,10), time = 4.0f }
    }
        };
    }
}




*/