package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;

public class SchemeAsset extends Asset
{
	private static final AssetInfo info = new AssetInfo("Scheme", "sch", "SchemeImporter");
	
	public SchemeAsset()
	{
		super(info);
	}
	
	public SchemeAsset(String name, File file)
	{
		super(info, name, file);
	}

}
