using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Consoles;

namespace Bomberman
{
    public class CVars
    {
        /* System config */
        public static readonly CVar sy_maxControllers = new CVar("sy_maxControllers", 4, CFlags.Hidden);    // max supported controllers
        public static readonly CVar sy_maxKeyboards = new CVar("sy_maxKeyboards", 6, CFlags.Hidden);        // max supported keyboard players

        /* Round */
        public static readonly CVar roundsToWin = new CVar("roundsToWin", 3);   // number of rounds to win
        public static readonly CVar roundTime = new CVar("roundTime", 10);      // round time (before sudden death)
        public static readonly CVar winDelay = new CVar("winDelay", 3);         // delay between round end (everyone is dead or single player left) and results screen

        /* Player */
        public static readonly CVar cg_playerSpeed = new CVar("cg_playerSpeed", 200, CFlags.Readonly);      // player's default speed
        public static readonly CVar cg_playerSpeedAdd = new CVar("cg_playerSpeedAdd", 30, CFlags.Readonly); // player's speed addition (when speed boost is picked)
        public static readonly CVar name = new CVar("name", "UnnamedPlayer");                               // player's default name

        /* Bomb */
        public static readonly CVar cg_bombRollSpeed = new CVar("cg_bombRollSpeed", 250, CFlags.Readonly);      // kicked bomb rolling speed
        public static readonly CVar cg_bombFlySpeed = new CVar("cg_bombFlySpeed", 300, CFlags.Readonly);        // affects bomb fly time
        public static readonly CVar cg_bombDropGravity = new CVar("cg_bombDropGravity", 4000, CFlags.Readonly); // affects bomb throw height
        public static readonly CVar cg_bombShortFlame = new CVar("cg_bombShortFlame", 1, CFlags.Readonly);      // flame cell radius on "short" bomb
        public static readonly CVar cg_bombFlyDistance = new CVar("cg_bombFlyDistance", 3, CFlags.Readonly);    // punched/thrown bomb fly distance in cells
        public static readonly CVar cg_bombJumpDistance = new CVar("cg_bombJumpDistance", 1, CFlags.Readonly);  // punched/thrown bomb jump distance (after landing)

        /* Initial powerups count */
        public static readonly CVar cg_initBomb = new CVar("cg_initBomb", 1);
        public static readonly CVar cg_initFlame = new CVar("cg_initFlame", 2);
        public static readonly CVar cg_initDisease = new CVar("cg_initDisease", 0);
        public static readonly CVar cg_initKick = new CVar("cg_initKick", 0);
        public static readonly CVar cg_initExtraSpeed = new CVar("cg_initExtraSpeed", 0);
        public static readonly CVar cg_initPunch = new CVar("cg_initPunch", 0);
        public static readonly CVar cg_initGrab = new CVar("cg_initGrab", 0);
        public static readonly CVar cg_initSpooger = new CVar("cg_initSpooger", 0);
        public static readonly CVar cg_initGoldflame = new CVar("cg_initGoldflame", 0);
        public static readonly CVar cg_initTrigger = new CVar("cg_initTrigger", 0);
        public static readonly CVar cg_initJelly = new CVar("cg_initJelly", 0);
        public static readonly CVar cg_initEbola = new CVar("cg_initEbola", 0);
        public static readonly CVar cg_initRandom = new CVar("cg_initRandom", 0);

        /* Max powerups count */
        public static readonly CVar cg_maxBomb = new CVar("cg_maxBomb", 8, CFlags.Readonly);
        public static readonly CVar cg_maxFlame = new CVar("cg_maxFlame", 8, CFlags.Readonly);
        public static readonly CVar cg_maxDisease = new CVar("cg_maxDisease", 0, CFlags.Readonly);
        public static readonly CVar cg_maxKick = new CVar("cg_maxKick", 1, CFlags.Readonly);
        public static readonly CVar cg_maxSpeed = new CVar("cg_maxExtraSpeed", 4, CFlags.Readonly);
        public static readonly CVar cg_maxPunch = new CVar("cg_maxPunch", 1, CFlags.Readonly);
        public static readonly CVar cg_maxGrab = new CVar("cg_maxGrab", 1, CFlags.Readonly);
        public static readonly CVar cg_maxSpooger = new CVar("cg_maxSpooger", 1, CFlags.Readonly);
        public static readonly CVar cg_maxGoldflame = new CVar("cg_maxGoldflame", 1, CFlags.Readonly);
        public static readonly CVar cg_maxTrigger = new CVar("cg_maxTrigger", 1, CFlags.Readonly);
        public static readonly CVar cg_maxJelly = new CVar("cg_maxJelly", 1, CFlags.Readonly);
        public static readonly CVar cg_maxEbola = new CVar("cg_maxEbola", 0, CFlags.Readonly);
        public static readonly CVar cg_maxRandom = new CVar("cg_maxRandom", 0, CFlags.Readonly);

        /* TODO: comment */
        public static readonly CVar cg_fieldBomb = new CVar("cg_fieldBomb", 10);
        public static readonly CVar cg_fieldFlame = new CVar("cg_fieldFlame", 10);
        public static readonly CVar cg_fieldDisease = new CVar("cg_fieldDisease", 10);
        public static readonly CVar cg_fieldKick = new CVar("cg_fieldKick", 4);
        public static readonly CVar cg_fieldExtraSpeed = new CVar("cg_fieldExtraSpeed", 8);
        public static readonly CVar cg_fieldPunch = new CVar("cg_fieldPunch", 2);
        public static readonly CVar cg_fieldGrab = new CVar("cg_fieldGrab", 2);
        public static readonly CVar cg_fieldSpooger = new CVar("cg_fieldSpooger", 1);
        public static readonly CVar cg_fieldGoldflame = new CVar("cg_fieldGoldflame", -2);
        public static readonly CVar cg_fieldTrigger = new CVar("cg_fieldTrigger", 5);
        public static readonly CVar cg_fieldJelly = new CVar("cg_fieldJelly", 1);
        public static readonly CVar cg_fieldEbola = new CVar("cg_fieldEbola", -4);
        public static readonly CVar cg_fieldRandom = new CVar("cg_fieldRandom", -2);

        /* Game settings */
        public static readonly CVar cg_maxPlayers = new CVar("cg_maxPlayers", 10);  // max multilayer players count (local/network)

        /* Timings */
        public static readonly CVar cg_fuzeTimeNormal = new CVar("cg_fuzeTimeNormal", 2000);    // "normal" bomb explosion time
        public static readonly CVar cg_fuzeTimeShort = new CVar("cg_fuzeTimeShort", 500);       // "short" bomb explosion time (when infected)
        public static readonly CVar cg_timeFlame = new CVar("cg_timeFlame", 500);               // explosion flame delay

        /* Debug */
        public static readonly CVar g_drawGrid = new CVar("g_drawGrid", 0, CFlags.Debug);
        public static readonly CVar g_drawSlotSize = new CVar("g_drawSlotSize", 0, CFlags.Debug);
        public static readonly CVar g_drawPlayerCell = new CVar("g_drawPlayerCell", 0, CFlags.Debug);
        public static readonly CVar g_drawPlayerMovable = new CVar("g_drawPlayerMovable", 0, CFlags.Debug);
        public static readonly CVar g_drawHiddenPowerups = new CVar("g_drawHiddenPowerups", 0, CFlags.Debug);
        public static readonly CVar g_startupMultiplayerMode = new CVar("g_startupMultiplayerMode", null, CFlags.Debug);

        /* Network */
        public static readonly CVar sv_port = new CVar("sv_port", 1334);
        public static readonly CVar sv_appId = new CVar("sv_appId", "bomberman");
        public static readonly CVar sv_hostname = new CVar("sv_hostname", "noname");

        public static readonly CVar[] powerupsInitials = 
        {
	        cg_initBomb,
	        cg_initFlame,
	        cg_initDisease,
	        cg_initKick,
	        cg_initExtraSpeed,
	        cg_initPunch,
	        cg_initGrab,
	        cg_initSpooger,
	        cg_initGoldflame,
	        cg_initTrigger,
	        cg_initJelly,
	        cg_initEbola,
	        cg_initRandom,
        };

        public static readonly CVar[] powerupsMax = 
        {
	        cg_maxBomb,
	        cg_maxFlame,
	        cg_maxDisease,
	        cg_maxKick,
	        cg_maxSpeed,
	        cg_maxPunch,
	        cg_maxGrab,
	        cg_maxSpooger,
	        cg_maxGoldflame,
	        cg_maxTrigger,
	        cg_maxJelly,
	        cg_maxEbola,
	        cg_maxRandom,
        };

        public static readonly CVar[] powerupsCount = 
        {
	        cg_fieldBomb,
	        cg_fieldFlame,
	        cg_fieldDisease,
	        cg_fieldKick,
	        cg_fieldExtraSpeed,
	        cg_fieldPunch,
	        cg_fieldGrab,
	        cg_fieldSpooger,
	        cg_fieldGoldflame,
	        cg_fieldTrigger,
	        cg_fieldJelly,
	        cg_fieldEbola,
	        cg_fieldRandom,
        };

        private static readonly CVar[] debugVars =
        {
            g_drawGrid,
            g_drawSlotSize,
            g_drawPlayerCell,
            g_drawPlayerMovable,
            g_drawHiddenPowerups,
            g_startupMultiplayerMode,
        };

        private static readonly CVar[] serverVars =
        {
            sv_appId,
            sv_port,
            sv_hostname,
        };

        public static void Register(CConsole console)
        {
            console.RegisterCvar(sy_maxControllers);
            console.RegisterCvar(sy_maxKeyboards);

            console.RegisterCvar(name);

            console.RegisterCvar(roundsToWin);
            console.RegisterCvar(roundTime);
            console.RegisterCvar(winDelay);

            console.RegisterCvars(powerupsInitials);
            console.RegisterCvars(powerupsMax);
            console.RegisterCvars(powerupsCount);
            console.RegisterCvars(debugVars);
            console.RegisterCvars(serverVars);

            console.RegisterCvar(cg_fuzeTimeNormal);
            console.RegisterCvar(cg_fuzeTimeShort);
            console.RegisterCvar(cg_timeFlame);

            console.RegisterCvar(cg_maxPlayers);

            console.RegisterCvar(cg_playerSpeed);
            console.RegisterCvar(cg_playerSpeedAdd);

            console.RegisterCvar(cg_bombRollSpeed);
            console.RegisterCvar(cg_bombFlySpeed);
            console.RegisterCvar(cg_bombDropGravity);
            console.RegisterCvar(cg_bombShortFlame);
            console.RegisterCvar(cg_bombFlyDistance);
            console.RegisterCvar(cg_bombJumpDistance);
        }
    }
}
