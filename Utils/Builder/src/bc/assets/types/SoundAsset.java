package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;

public class SoundAsset extends Asset
{
	private static final AssetInfo info = new AssetInfo("Sound", "snd", "WavImporter", "SoundEffectProcessor");

	public SoundAsset()
	{
		super(info);
	}

	public SoundAsset(String name, File file)
	{
		super(info, name, file);
	}
}
