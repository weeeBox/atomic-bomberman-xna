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
		public const int tex_SKULLZ = 4;
		public const int tex_FIELD7 = 5;
		// total resources count
		public const int RES_COUNT = 6;
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
				new AssetLoadInfo("SKULLZ", A.tex_SKULLZ, AssetType.Texture),
				new AssetLoadInfo("FIELD7", A.tex_FIELD7, AssetType.Texture),
			}
		};
		
		public static AssetLoadInfo[] GetPack(Packs pack)
		{
			return PACKS[(int)pack];
		}
	}
}
