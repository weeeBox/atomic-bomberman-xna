package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;

public class VectorFontAsset extends Asset 
{	
	private static final AssetInfo info = new AssetInfo("VectorFont", "fnt", "FontDescriptionImporter", "FontDescriptionProcessor");
	
	public VectorFontAsset()
	{
		super(info);
	}
	
	public VectorFontAsset(String name, File file)
	{
		super(info, name, file);
	}
}
