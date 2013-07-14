package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;

public class Sound extends Asset
{
	private static AssetInfo info; // = new AssetInfo("Sound", "snd", "WavImporter", "SoundEffectProcessor");

	public Sound()
	{
		super(info);
	}

	public Sound(String name, File file)
	{
		super(info, name, file);
	}
}
