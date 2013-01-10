package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;

public class MusicAsset extends Asset 
{
	private static final AssetInfo info = new AssetInfo("MUSIC", "MUSIC", "Mp3Importer", "SongProcessor");
	
	public MusicAsset()
	{
		super(info);
	}
	
	public MusicAsset(String name, File file)
	{
		super(info, name, file);
	}
}
