package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;

public class Scheme extends Asset
{
	private static AssetInfo info; // = new AssetInfo("Scheme", "sch", "SchemeImporter");
	
	public Scheme()
	{
		super(info);
	}
	
	public Scheme(String name, File file)
	{
		super(info, name, file);
	}

}
