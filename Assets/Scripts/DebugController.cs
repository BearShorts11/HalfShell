using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{

    bool showConsole; //key = / 
    string input;
    private bool showHelp;
    Vector2 scroll;

    PlayerBehavior player;
    PlayerShooting shooting;

    public List<object> commandList;
    public static DebugCommand MAX_HEALTH;
    public static DebugCommand MAX_SLUGS;
    public static DebugCommand DAMAGE;
    public static DebugCommand<float> DAMAGE_AMT;
    public static DebugCommand<bool> INVINCIBILITY;
    public static DebugCommand<bool> USE_SHELLS;
    public static DebugCommand<string> SET_SHELLTYPE_MAX;
    public static DebugCommand HELP;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerBehavior>();
        shooting = FindFirstObjectByType<PlayerShooting>();

        #region declaring commands
        MAX_HEALTH = new DebugCommand("max_health", "sets player to max health", "max_health", () =>
        { 
            player.SetHealth(player.maxHealth);
        });

        MAX_SLUGS = new DebugCommand("max_slugs", "sets slug shell count to max", "max_slugs", () =>
        {
            Slug s = new Slug();
            shooting.AddAmmo(s.MaxHolding, s);
        });

        DAMAGE = new DebugCommand("damage", "damage the player by 10HP", "damage", () =>
        {
            player.Damage(10);
        });

        DAMAGE_AMT = new DebugCommand<float>("dmg_amount", "damage the player by amount specified", "dmg_amount <float>", (x) =>
        {
            player.Damage(x);
        });

        INVINCIBILITY = new DebugCommand<bool>("invincibility_set", "sets the player invincible (no damage) according to bool", "invincibility_set <bool>", (x) =>
        {
            if (x) player.Invincible();
            else player.Mortal();
        });

        USE_SHELLS = new DebugCommand<bool>("use_shells", "toggles if player uses shells when adding ammo to mag", "use_shells <bool>", (x) =>
        {
            if (x) shooting.UseShells();
            else shooting.InfiniteShells();
        });

        SET_SHELLTYPE_MAX = new DebugCommand<string>("shelltype_max", "sets specified shell type to max", "shelltype_max <string>", (x) =>
        {
            ShellBase shell;
            switch (x)
            {
                case "slug":
                    shell = new Slug();
                    break;
                default:
                    shell = new HalfShell();
                    break;
            }
            shooting.AddAmmo(shell.MaxHolding, shell);

        });

        HELP = new DebugCommand("help", "shows list of commands", "help", () =>
        {
            showHelp = true;
        });
        #endregion


        commandList = new List<object>
        {
            MAX_HEALTH, MAX_SLUGS, DAMAGE, DAMAGE_AMT, INVINCIBILITY, USE_SHELLS, SET_SHELLTYPE_MAX, HELP
        };
    }

    public void OnToggleDebug(InputValue val)
    { 
        showConsole = !showConsole;   
    }
    public void OnReturn(InputValue val)
    {
        if (showConsole)
        {
            HandleInput();
            input = "";
        }
    }


    private void OnGUI()
    {
        if (!showConsole)
        { 
            //player.YesMove();
            //if (Input.GetKeyDown(KeyCode.Tab)) PlayerBehavior.UnlockCursor();
            //else PlayerBehavior.LockCursor();
            return;
        }

        //player.NoMove();
        //PlayerBehavior.UnlockCursor();

        float y = 0f;

        if (showHelp)
        {
            GUI.Box(new Rect(0, y, Screen.width, 150), "");
            Rect viewport = new Rect(0, 0, Screen.width - 30f, 20 * commandList.Count);
            scroll = GUI.BeginScrollView(new Rect(0, y+5f, Screen.width, 140f), scroll, viewport);

            for (int i = 0; i < commandList.Count; i++)
            { 
                DebugCommandBase command = commandList[i] as DebugCommandBase;

                string label = $"{command.CommandFormat} : {command.CommandDescription}";
                Rect labelRect = new Rect(5, 20*i, viewport.width-100, 20);

                GUI.Label(labelRect, label);
            }
            GUI.EndScrollView();
            y += 150;
        }

        GUI.Box(new Rect(0, y, Screen.width, 30), "");
        GUI.backgroundColor = new Color(0,0,0,0);

        input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
    }

    private void HandleInput()
    {
        string[] properties = input.Split(' ');

        for (int i = 0; i < commandList.Count; i++)
        { 
        
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

            if(input.Contains(commandBase.CommandId) )
            {
                if (commandList[i] as DebugCommand != null) (commandList[i] as DebugCommand).Invoke();
                else if (commandList[i] as DebugCommand<float> != null) (commandList[i] as DebugCommand<float>).Invoke(int.Parse(properties[1]));
                else if (commandList[i] as DebugCommand<bool> != null) (commandList[i] as DebugCommand<bool>).Invoke(bool.Parse(properties[1]));
                else if (commandList[i] as DebugCommand<string> != null) (commandList[i] as DebugCommand<string>).Invoke(properties[1]);
            }

        }
    }

}
