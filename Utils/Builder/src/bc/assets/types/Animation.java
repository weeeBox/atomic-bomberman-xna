package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;

public class Animation extends Asset
{
	private static AssetInfo info = new AssetInfo("Animation", "ani", "AnimationImporter");
	
	public Animation()
	{
		super(info);
	}
	
	public Animation(String name, File file)
	{
		super(info, name, file);
	}
}
