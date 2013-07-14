package bc.assets.info;

import java.io.File;

import bc.assets.AssetInfo;

public class SoundInfo extends AssetInfo
{
	public SoundInfo()
	{
		super("Sound");
	}

	public SoundInfo(String name, File file)
	{
		super(name, file);
	}
}
