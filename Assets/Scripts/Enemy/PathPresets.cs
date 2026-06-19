using System.Collections.Generic;
using UnityEngine;
using static GalagaPaths;

public enum FormationPath
{
    BeeLeft1,
    BeeLeft2,
    BeeRight1,
    BeeRight2,
    ButterflyLeft1,
    ButterflyRight1,
    BossLeft,
    BossRight,
    AttackDiveLeft,
    AttackDiveRight,
    FigureEight

}


public static class PathPresets
{
    public static List<Vector2> Get(
        FormationPath path)
    {
        switch (path)
        {
            case FormationPath.BeeLeft1:
                return BeeLeft1();

            case FormationPath.BeeLeft2:
                return BeeLeft2();

            case FormationPath.BeeRight1:
                return Mirror(BeeLeft1());

            case FormationPath.BeeRight2:
                return Mirror(BeeLeft2());

            case FormationPath.ButterflyLeft1:
                return ButterflyLeft1();

            case FormationPath.ButterflyRight1:
                return Mirror(ButterflyLeft1());

            case FormationPath.BossLeft:
                return BossLeft();

            case FormationPath.BossRight:
                return Mirror(BossLeft());

            case FormationPath.AttackDiveLeft:
                return AttackDiveLeft();

            case FormationPath.AttackDiveRight:
                return Mirror(AttackDiveLeft());

            case FormationPath.FigureEight:
                return FigureEight();

            default:
                return new List<Vector2>();
        }
    }

    static List<Vector2> Mirror(List<Vector2> src)
    {
        List<Vector2> result = new();

        foreach (var p in src)
            result.Add(new Vector2(-p.x, p.y));

        return result;
    }

    static List<Vector2> BeeLeft1()
    {
        return new()
        {
            new(-300,300),
            new(-260,200),
            new(-180,50),
            new(-80,-80),
            new(50,-120),
            new(120,-50),
            new(60,50),
            new(0,100)
        };
    }

    static List<Vector2> BeeLeft2()
    {
        return new()
        {
            new(-350,250),
            new(-300,100),
            new(-150,-100),
            new(50,-150),
            new(180,-50),
            new(140,100),
            new(0,120)
        };
    }

    static List<Vector2> ButterflyLeft1()
    {
        return new()
        {
            new(-400,350),
            new(-250,150),
            new(-50,-50),
            new(150,-100),
            new(250,0),
            new(180,120),
            new(0,180)
        };
    }

    static List<Vector2> BossLeft()
    {
        return new()
        {
            new(-450,400),
            new(-300,200),
            new(-100,-100),
            new(200,-150),
            new(350,50),
            new(250,250),
            new(0,250)
        };
    }

    static List<Vector2> AttackDiveLeft()
    {
        return new()
        {
            new(0,200),
            new(-80,120),
            new(-150,-20),
            new(-100,-250),
            new(50,-450),
            new(120,-700)
        };
    }

    static List<Vector2> FigureEight()
    {
        return new()
        {
            new(0,0),
            new(100,100),
            new(200,0),
            new(100,-100),
            new(0,0),
            new(-100,100),
            new(-200,0),
            new(-100,-100),
            new(0,0)
        };
    }
}