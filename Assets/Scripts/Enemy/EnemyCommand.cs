#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public enum CommandCondictionType
{
    NONE,Distance, Time
}
public enum EnemyCommandMode
{
   SPEED, GOTO, SPIRAL
}

public class EnemyCommand
{
    public EnemyCommandMode mode;

    public GameObject target;

    public float arg_speed;
    public float arg_distance;
    public float radial_speed;

    public CommandCondictionType condType;
    public float condValue;

    public EnemyCommand next = null;

}
/*
public class CommandCondiction
{
    public float value;
    public CommandCondictionType type = CommandCondictionType.NONE;

    public static CommandCondiction Time(float seconds) => new CommandCondiction() { type = CommandCondictionType.Time, value = seconds };

}

*/
[CreateAssetMenu(menuName = "AI/Enemy Command Stack")]
public class EnemyCommandStack :ScriptableObject
{
    [SerializeField, TextArea(5, 20)]
    private string script;
    public string Script => script;

    EnemyGroup group;

    List<EnemyCommand> commands = new List<EnemyCommand>();
    EnemyCommand cmd;

    public EnemyCommand? FirstCommand { get { return commands.FirstOrDefault(); } }

    public EnemyCommand? NextCommand(EnemyCommand cmd)
    {
        return cmd.next; 
    }

    
    string[] GetArgs(string txt, int minArgs)
    {
        var a  = txt.Split(' ');
        a = a.Where(X => X != "").Skip(1).ToArray();
        if (a.Length < minArgs)
            throw new Exception("bar argument: " + txt);
        return a;
    }
    T GetArg<T>(string txt, int index)
    {
        var args = GetArgs(txt, index);
        var val = args[index];

        // già del tipo giusto
        if (val is T casted)
            return casted;

        // prova conversione (string → int, float, ecc.)
        try
        {
            return (T)System.Convert.ChangeType(val, typeof(T));
        }
        catch
        {
            throw new System.InvalidCastException(
                $"Cannot convert argument '{val}' to {typeof(T)}"
            );
        }
    }

    void begin(EnemyCommandMode mode)
    {
        var old_cmd = cmd;
        cmd = new EnemyCommand() { mode = mode };
        if (old_cmd!=null)
            old_cmd.next = cmd;
    }
    void end()
    {
        commands.Add(cmd);
    }

    void cmd_arg_target(string target)
    {
        Debug.Log(">> cmd_arg_target " + target);
        if (target == "player")
            cmd.target = group.target;

    }
    void cmd_arg_distance(float dist)
    {
        Debug.Log(">> cmd_arg_distance " + dist);
        cmd.arg_distance = dist;

    }
    void cmd_arg_radialspeed(float speed)
    {
        Debug.Log(">> cmd_arg_radialspeed " + speed);
        cmd.radial_speed = speed;

    }
    /*
    void cmd_speed(float speed)
    {
        Debug.Log(">> speed "+ speed);
        cmd.speed = speed;  
    }
    */
    void cmd_cond_distance(float distance)
    {
        Debug.Log(">> cmd_cond_distance " + distance);
        cmd.condType = CommandCondictionType.Distance;
        cmd.condValue = distance;    
    }
    void cmd_cond_time(float time)
    {
        Debug.Log(">> cmd_cond_time " + time);
        cmd.condType = CommandCondictionType.Time;
        cmd.condValue = time;
    }

    void Parse(string[] lines ,string command, ref int idx)
    {
        if (command == "goto")
            begin(EnemyCommandMode.GOTO);
        if (command == "spiral")
            begin(EnemyCommandMode.SPIRAL);

        idx++;
        while (idx < lines.Length && lines[idx].StartsWith('\t'))
        {
           // Debug.Log(".."+ lines[idx]);

            int i = lines[idx].LastIndexOf("\t");
            var cmd = lines[idx].Replace("\t", "");
        //    Debug.Log("cmd|" + cmd+"|");
           /* if (cmd.StartsWith("speed"))
            {
                var speed = GetArg<float>(lines[idx], 0);
                cmd_speed(speed);
            }
           */
            if (cmd.StartsWith("target"))
            {
                var who = GetArg<string>(lines[idx], 0);
                cmd_arg_target(who);
            }
            if (cmd.StartsWith("distance"))
            {
                cmd_arg_distance(GetArg<float>(cmd, 0));
            }
            if (cmd.StartsWith("radial_speed"))
            {
                cmd_arg_radialspeed(GetArg<float>(cmd, 0));
            }
            if (cmd.StartsWith("["))
            {
                cmd = cmd.Substring(1, cmd.Length-2);
                
                if (cmd.StartsWith("distance"))
                {
                    cmd_cond_distance(GetArg<float>(cmd, 0));
                }
                if (cmd.StartsWith("time"))
                {
                    cmd_cond_time(GetArg<float>(cmd, 0));
                }
            }
            //cmd_speed(GetArgs(1)[0])

            idx++;
        }
        end();
        idx--;
    }

    public void Parse(EnemyGroup group)
    {
        commands.Clear();   
        this.group = group;
        string[] lines = script.ToLower().Split("\n");
        lines = lines.Select( X => X.TrimEnd() ).ToArray();
        for (int i=0;i< lines.Length;i++)
        {
            Debug.Log(lines[i]);
            if (lines[i].StartsWith("speed"))
            {
                begin(EnemyCommandMode.SPEED);
                cmd.arg_speed = GetArg<float>(lines[i], 0);
                end();
            }
            if (lines[i].StartsWith("goto"))
            {
                Parse(lines, "goto", ref i);
            }
            if (lines[i].StartsWith("spiral"))
            {
                Parse(lines, "spiral",ref i);
            }

        }

        cmd = commands.FirstOrDefault();

    }


  
}
