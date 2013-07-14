package bc.assets.info;

import java.io.File;

import bc.assets.AssetInfo;

public class MusicInfo extends AssetInfo 
{
	public MusicInfo()
	{
		super("Music");
	}
	
	public MusicInfo(String name, File file)
	{
		super(name, file);
	}
}
