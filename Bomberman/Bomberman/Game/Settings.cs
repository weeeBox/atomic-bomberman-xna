using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace Bomberman.Game
{
    public sealed class Settings
    {
        public static readonly int VAL_RES_W = 1024;
        public static readonly int VAL_RES_H = 768;

        public static readonly int VAL_CELL_W = 64;
        public static readonly int VAL_CELL_H = 58;

        public static readonly int VAL_FIELD_OFFSET_X = 32;
        public static readonly int VAL_FIELD_OFFSET_Y = 103;

        public static readonly int VAL_SHOVE_W = 20;
        public static readonly int VAL_SHOVE_H = 18;

        public static readonly int VAL_FIELD_WIDTH = 15;
        public static readonly int VAL_FIELD_HEIGHT = 11;

        public static readonly int VAL_MAX_MAPS = 11;
        public static readonly int VAL_MAX_PLAYERS = 10;

        public static readonly int VAL_NUM_TO_WIN_MATCH = 3;
        public static readonly int VAL_WIN_ON_WINS = 1;

        public static readonly int VAL_ENCLOSEMENT_DEPTH = 3;

        public static readonly int VAL_STOMPED_BOMBS_DETONATE = 1;
        public static readonly int VAL_BOMB_THROW_DISTANCE = 3;

        public static readonly int VAL_TEAMPLAY = 0;
        public static readonly int VAL_GAME_MUSIC = 1;
        public static readonly int VAL_MENU_MUSIC = 1;
        public static readonly int VAL_MAP = 0;
        public static readonly int VAL_ROUNDTIME = 150;
        public static readonly int VAL_FULLSCREEN = 1;
        public static readonly int VAL_PLAYER_RANDOM_START = 1;
        public static readonly int VAL_GOLDBOMBERMAN = 0;
        public static readonly int VAL_CONVEYOR_SPEED = 1;
        public static readonly int VAL_LOST_NET_REVERT_AI = 0;
        public static readonly int VAL_INTRO_TIME_LOGO = 5;
        public static readonly int VAL_INTRO_TIME_TITLE = 20;

        public static readonly int VAL_PLAYER_SPEED = 290;
        public static readonly int VAL_PLAYER_SPEED_ADD = 50;
        public static readonly int VAL_PLAYER_SPEED_MOLASSES = 100;
        public static readonly int VAL_PLAYER_SPEED_CRACK = 400;
        public static readonly int VAL_PLAYER_SPEED_CRACK_ADD = 200;

        public static readonly int VAL_BOMB_ROLL_SPEED = 250;
        public static readonly int VAL_BOMB_FLY_SPEED = 300;
        public static readonly int VAL_BOMB_DROP_GRAVITY = 4000;

        public static readonly int VAL_MAINMENU_FONT_COLOR_R = 255;
        public static readonly int VAL_MAINMENU_FONT_COLOR_G = 255;
        public static readonly int VAL_MAINMENU_FONT_COLOR_B = 255;

        public static readonly int VAL_MENU_KEYREPEAT_DELAY = 250;
        public static readonly int VAL_MENU_KEYREPEAT_INTERVAL = 100;

        public static readonly int VAL_DOUBLECLICK_TIME = 100;

        public static readonly int VAL_FUZE_TIME_NORMAL = 2000;
        public static readonly int VAL_FUZE_TIME_SHORT = 1000;
        public static readonly int VAL_FUZE_TIME_DUD_MIN = 6000;
        public static readonly int VAL_FUZE_TIME_DUD_ADD = 10000;
        public static readonly int VAL_TIME_FLAME_TRIGGER = 20;
        public static readonly int VAL_TIME_FLAME = 500;
        public static readonly int VAL_TIME_BRICK_DISINTEGRATE = 500;
        public static readonly int VAL_TIME_SURVIVE_TILL_WIN = 1000;
        public static readonly int VAL_MAX_TIME_PER_FRAME = 95;
        public static readonly int VAL_TEAMPLAY_TIME_TRUECOLORS = 2000;
        public static readonly int VAL_SHOW_POWERUPS_IN_BRICKS = 1;
        public static readonly int VAL_SUICIDE_IMPOSSIBLE = 0;
        public static readonly int VAL_FIREINTHEHOLE_TIME = 1000;
        public static readonly int VAL_FIREINTHEHOLE_BOMBS = 5;
        public static readonly int VAL_PLAYER_KEEPS_ADDITIONAL_PUS = 0;
        public static readonly int VAL_TIME_POWERUP_RECYCLE = 3;
        public static readonly int VAL_POWERUPS_DESTROYABLE = 0;
        public static readonly int VAL_DISEASES_DESTROYABLE = 1;
        public static readonly int VAL_TRY_HIDING_DEAD_PLAYER_PUS = 0;
        public static readonly int VAL_TRY_HIDING_EXCLUSION_PUS = 0;
        public static readonly int VAL_TRY_HIDING_OTHER_RECYCLED_PUS = 1;

        public static readonly int VAL_PU_INIT_BOMB = 1;
        public static readonly int VAL_PU_INIT_FLAME = 2;
        public static readonly int VAL_PU_INIT_DISEASE = 0;
        public static readonly int VAL_PU_INIT_ABILITY_KICK = 0;
        public static readonly int VAL_PU_INIT_EXTRA_SPEED = 0;
        public static readonly int VAL_PU_INIT_ABLITY_PUNCH = 0;
        public static readonly int VAL_PU_INIT_ABILITY_GRAB = 0;
        public static readonly int VAL_PU_INIT_SPOOGER = 0;
        public static readonly int VAL_PU_INIT_GOLDFLAME = 0;
        public static readonly int VAL_PU_INIT_TRIGGER = 0;
        public static readonly int VAL_PU_INIT_JELLY = 0;
        public static readonly int VAL_PU_INIT_EBOLA = 0;
        public static readonly int VAL_PU_INIT_RANDOM = 0;

        public static readonly int VAL_PU_MAX_BOMB = 8;
        public static readonly int VAL_PU_MAX_FLAME = 8;
        public static readonly int VAL_PU_MAX_DISEASE = 0;
        public static readonly int VAL_PU_MAX_ABILITY_KICK = 1;
        public static readonly int VAL_PU_MAX_EXTRA_SPEED = 4;
        public static readonly int VAL_PU_MAX_ABILITY_PUNCH = 1;
        public static readonly int VAL_PU_MAX_ABILITY_GRAB = 1;
        public static readonly int VAL_PU_MAX_SPOOGER = 1;
        public static readonly int VAL_PU_MAX_GOLDFLAME = 1;
        public static readonly int VAL_PU_MAX_TRIGGER = 1;
        public static readonly int VAL_PU_MAX_JELLY = 1;
        public static readonly int VAL_PU_MAX_EBOLA = 0;
        public static readonly int VAL_PU_MAX_RANDOM = 0;
        public static readonly int VAL_PU_MAX_LAST = -2000;

        public static readonly int VAL_PU_FIELD_BOMB = 10;
        public static readonly int VAL_PU_FIELD_FLAME = 10;
        public static readonly int VAL_PU_FIELD_DISEASE = 10;
        public static readonly int VAL_PU_FIELD_ABILITY_KICK = 4;
        public static readonly int VAL_PU_FIELD_EXTRA_SPEED = 8;
        public static readonly int VAL_PU_FIELD_ABLITY_PUNCH = 2;
        public static readonly int VAL_PU_FIELD_ABILITY_GRAB = 2;
        public static readonly int VAL_PU_FIELD_SPOOGER = 1;
        public static readonly int VAL_PU_FIELD_GOLDFLAME = -2;
        public static readonly int VAL_PU_FIELD_TRIGGER = 5;
        public static readonly int VAL_PU_FIELD_JELLY = 1;
        public static readonly int VAL_PU_FIELD_EBOLA = -4;
        public static readonly int VAL_PU_FIELD_RANDOM = -2;

        public static readonly int VAL_DSE_TIME_LIMITED = 1;
        public static readonly int VAL_DSE_MULTIPLY = 1;
        public static readonly int VAL_DSE_MIN_KEEP_BEFORE_PASS = 500;
        public static readonly int VAL_DSE_ARE_CUREABLE = 1;
        public static readonly int VAL_DSE_CURE_CHANCE = 10;
        public static readonly int VAL_DSE_TIME_MOLASSES = 15;
        public static readonly int VAL_DSE_TIME_CRACK = 15;
        public static readonly int VAL_DSE_TIME_CONSTIPATION = 15;
        public static readonly int VAL_DSE_TIME_POOPS = 15;
        public static readonly int VAL_DSE_TIME_SHORTFLAME = 15;
        public static readonly int VAL_DSE_TIME_CRACKPOOPS = 15;
        public static readonly int VAL_DSE_TIME_SHORTFUZE = 15;
        public static readonly int VAL_DSE_TIME_SWAP = 0;
        public static readonly int VAL_DSE_TIME_REVERSED = 15;
        public static readonly int VAL_DSE_TIME_LEPROSY = 15;
        public static readonly int VAL_DSE_TIME_INVISIBLE = 15;
        public static readonly int VAL_DSE_TIME_DUDS = 15;
        public static readonly int VAL_DSE_TIME_HYPERSWAP = 0;
        public static readonly int VAL_DSE_PROB_MOLASSES = 10;
        public static readonly int VAL_DSE_PROB_CRACK = 10;
        public static readonly int VAL_DSE_PROB_CONSTIPATION = 10;
        public static readonly int VAL_DSE_PROB_POOPS = 10;
        public static readonly int VAL_DSE_PROB_SHORTFLAME = 10;
        public static readonly int VAL_DSE_PROB_CRACKPOOPS = 10;
        public static readonly int VAL_DSE_PROB_SHORTFUZE = 10;
        public static readonly int VAL_DSE_PROB_SWAP = 10;
        public static readonly int VAL_DSE_PROB_REVERSED = 10;
        public static readonly int VAL_DSE_PROB_LEPROSY = 10;
        public static readonly int VAL_DSE_PROB_INVISIBLE = 10;
        public static readonly int VAL_DSE_PROB_DUDS = 10;
        public static readonly int VAL_DSE_PROB_HYPERSWAP = 10;

        public static readonly int VAL_COLOR_PLAYER_0R = 255;
        public static readonly int VAL_COLOR_PLAYER_0G = 255;
        public static readonly int VAL_COLOR_PLAYER_0B = 255;
        public static readonly int VAL_COLOR_PLAYER_1R = 50;
        public static readonly int VAL_COLOR_PLAYER_1G = 50;
        public static readonly int VAL_COLOR_PLAYER_1B = 50;
        public static readonly int VAL_COLOR_PLAYER_2R = 255;
        public static readonly int VAL_COLOR_PLAYER_2G = 0;
        public static readonly int VAL_COLOR_PLAYER_2B = 0;
        public static readonly int VAL_COLOR_PLAYER_3R = 0;
        public static readonly int VAL_COLOR_PLAYER_3G = 0;
        public static readonly int VAL_COLOR_PLAYER_3B = 255;
        public static readonly int VAL_COLOR_PLAYER_4R = 0;
        public static readonly int VAL_COLOR_PLAYER_4G = 255;
        public static readonly int VAL_COLOR_PLAYER_4B = 0;
        public static readonly int VAL_COLOR_PLAYER_5R = 255;
        public static readonly int VAL_COLOR_PLAYER_5G = 255;
        public static readonly int VAL_COLOR_PLAYER_5B = 0;
        public static readonly int VAL_COLOR_PLAYER_6R = 0;
        public static readonly int VAL_COLOR_PLAYER_6G = 255;
        public static readonly int VAL_COLOR_PLAYER_6B = 255;
        public static readonly int VAL_COLOR_PLAYER_7R = 255;
        public static readonly int VAL_COLOR_PLAYER_7G = 0;
        public static readonly int VAL_COLOR_PLAYER_7B = 255;
        public static readonly int VAL_COLOR_PLAYER_8R = 255;
        public static readonly int VAL_COLOR_PLAYER_8G = 128;
        public static readonly int VAL_COLOR_PLAYER_8B = 0;
        public static readonly int VAL_COLOR_PLAYER_9R = 128;
        public static readonly int VAL_COLOR_PLAYER_9G = 0;
        public static readonly int VAL_COLOR_PLAYER_9B = 255;

        public static int Get(int id)
        {
            return id;
        }
    }
}
