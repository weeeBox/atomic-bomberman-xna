// This file was generated. Do not modify.
 
using BomberEngine.Core.Assets;
 
namespace Assets
{
	public class AssetPacks
	{
		public const int PACK_ALL = 0;
		public const int PACKS_COUNT = 1;
	}
	
	public class A
	{
		// PACK_ALL
		public const int TXWLKE0001 = 0;
		public const int TXWLKN0001 = 1;
		public const int TXWLKS0001 = 2;
		public const int TXWLKW0001 = 3;
		// total
		public const int RES_COUNT = 4;
	}
	
	public class Assets
	{
		public static readonly AssetLoadInfo[][] PACKS =
		{
			// PACK_ALL
			new AssetLoadInfo[]
			{
				new AssetLoadInfo("WLKE0001", A.TXWLKE0001, AssetType.TEXTURE),
				new AssetLoadInfo("WLKN0001", A.TXWLKN0001, AssetType.TEXTURE),
				new AssetLoadInfo("WLKS0001", A.TXWLKS0001, AssetType.TEXTURE),
				new AssetLoadInfo("WLKW0001", A.TXWLKW0001, AssetType.TEXTURE),
			}
		};
	}
}
