package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;

public class CorelFontAsset extends Asset 
{	
	private static final AssetInfo info = new AssetInfo("COREL_FONT", "FNT", "FontDescriptionImporter", "FontDescriptionProcessor");
	
	public CorelFontAsset()
	{
		super(info);
	}
	
	public CorelFontAsset(String name, File file)
	{
		super(info, name, file);
	}
}
