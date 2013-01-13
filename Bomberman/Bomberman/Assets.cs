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
		public const int tex_FIELD7 = 4;
		public const int tex_BMB1001 = 5;
		public const int tex_F0BRICK = 6;
		public const int tex_F0SOLID = 7;
		// total resources count
		public const int RES_COUNT = 8;
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
				new AssetLoadInfo("FIELD7", A.tex_FIELD7, AssetType.Texture),
				new AssetLoadInfo("BMB1001", A.tex_BMB1001, AssetType.Texture),
				new AssetLoadInfo("F0BRICK", A.tex_F0BRICK, AssetType.Texture),
				new AssetLoadInfo("F0SOLID", A.tex_F0SOLID, AssetType.Texture),
			}
		};
		
		public static AssetLoadInfo[] GetPack(Packs pack)
		{
			return PACKS[(int)pack];
		}
	}
}
