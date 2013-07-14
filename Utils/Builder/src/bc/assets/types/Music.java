package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;

public class Music extends Asset 
{
	private static AssetInfo info; // = new AssetInfo("Music", "music", "Mp3Importer", "SongProcessor");
	
	public Music()
	{
		super(info);
	}
	
	public Music(String name, File file)
	{
		super(info, name, file);
	}
}
