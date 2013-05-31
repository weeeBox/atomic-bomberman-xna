// This file was generated. Do not modify.
 
using BomberEngine.Core.Assets;
 
namespace Assets
{
	public class A
	{
		// ALL
		public const int tex_WLKE0001 = 0;
		public const int tex_WLKN0001 = 1;
		public const int tex_WLKS0001 = 2;
		public const int tex_WLKW0001 = 3;
		public const int tex_PUD = 4;
		public const int tex_PUL = 5;
		public const int tex_PUR = 6;
		public const int tex_PUP = 7;
		public const int tex_FIELD7 = 8;
		public const int tex_BMB1001 = 9;
		public const int tex_BMBC1 = 10;
		public const int tex_DIGIB001 = 11;
		public const int tex_F0BRICK = 12;
		public const int tex_F0SOLID = 13;
		public const int fnt_system = 14;
		public const int fnt_console = 15;
		public const int fnt_button = 16;
		public const int tex_pow_bomb = 17;
		public const int tex_pow_disea = 18;
		public const int tex_pow_ebola = 19;
		public const int tex_pow_flame = 20;
		public const int tex_pow_gold = 21;
		public const int tex_pow_grab = 22;
		public const int tex_pow_jelly = 23;
		public const int tex_pow_kick = 24;
		public const int tex_pow_punch = 25;
		public const int tex_pow_skate = 26;
		public const int tex_pow_trig = 27;
		public const int tex_pow_pooge = 28;
		public const int tex_pow_rand = 29;
		// total resources count
		public const int RES_COUNT = 30;
	}
	
	public class AssetPacks
	{
		public enum Packs
		{
			ALL
		}
		
		private static readonly AssetLoadInfo[][] PACKS =
		{
			// ALL
			new AssetLoadInfo[]
			{
				new AssetLoadInfo("WLKE0001", A.tex_WLKE0001, AssetType.Texture),
				new AssetLoadInfo("WLKN0001", A.tex_WLKN0001, AssetType.Texture),
				new AssetLoadInfo("WLKS0001", A.tex_WLKS0001, AssetType.Texture),
				new AssetLoadInfo("WLKW0001", A.tex_WLKW0001, AssetType.Texture),
				new AssetLoadInfo("BMF0010", A.tex_PUD, AssetType.Texture),
				new AssetLoadInfo("PUL0010", A.tex_PUL, AssetType.Texture),
				new AssetLoadInfo("PUR0010", A.tex_PUR, AssetType.Texture),
				new AssetLoadInfo("PUBN010", A.tex_PUP, AssetType.Texture),
				new AssetLoadInfo("FIELD7", A.tex_FIELD7, AssetType.Texture),
				new AssetLoadInfo("BMB1001", A.tex_BMB1001, AssetType.Texture),
				new AssetLoadInfo("BMBC1", A.tex_BMBC1, AssetType.Texture),
				new AssetLoadInfo("DIGIB001", A.tex_DIGIB001, AssetType.Texture),
				new AssetLoadInfo("F0BRICK", A.tex_F0BRICK, AssetType.Texture),
				new AssetLoadInfo("F0SOLID", A.tex_F0SOLID, AssetType.Texture),
				new AssetLoadInfo("SystemFont", A.fnt_system, AssetType.VectorFont),
				new AssetLoadInfo("ConsoleFont", A.fnt_console, AssetType.VectorFont),
				new AssetLoadInfo("ButtonFont", A.fnt_button, AssetType.VectorFont),
				new AssetLoadInfo("POWBOMB", A.tex_pow_bomb, AssetType.Texture),
				new AssetLoadInfo("POWDISEA", A.tex_pow_disea, AssetType.Texture),
				new AssetLoadInfo("POWEBOLA", A.tex_pow_ebola, AssetType.Texture),
				new AssetLoadInfo("POWFLAME", A.tex_pow_flame, AssetType.Texture),
				new AssetLoadInfo("POWGOLD", A.tex_pow_gold, AssetType.Texture),
				new AssetLoadInfo("POWGRAB", A.tex_pow_grab, AssetType.Texture),
				new AssetLoadInfo("POWJELLY", A.tex_pow_jelly, AssetType.Texture),
				new AssetLoadInfo("POWKICK", A.tex_pow_kick, AssetType.Texture),
				new AssetLoadInfo("POWPUNCH", A.tex_pow_punch, AssetType.Texture),
				new AssetLoadInfo("POWSKATE", A.tex_pow_skate, AssetType.Texture),
				new AssetLoadInfo("POWTRIG", A.tex_pow_trig, AssetType.Texture),
				new AssetLoadInfo("PWSPOOGE", A.tex_pow_pooge, AssetType.Texture),
				new AssetLoadInfo("PWRANDOM", A.tex_pow_rand, AssetType.Texture),
			}
		};
		
		public static AssetLoadInfo[] GetPack(Packs pack)
		{
			return PACKS[(int)pack];
		}
	}
}
