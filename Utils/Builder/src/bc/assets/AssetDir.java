package bc.assets;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import bc.assets.types.Animation;
import bc.assets.types.BitmapFont;
import bc.assets.types.Music;
import bc.assets.types.Scheme;
import bc.assets.types.Sound;
import bc.assets.types.Texture;

public class AssetDir extends Asset
{
	private static final AssetInfo info;
	
	static
	{
		info = new AssetInfo();
		info.importer = new AssetDirImporter();
	}
	
	private List<Asset> assets = new ArrayList<Asset>();
	
	public AssetDir()
	{
		super(info);
	}
	
	protected AssetDir(String name, File file)
	{
		super(info, name, file);
	}
	
	public void addDir(AssetDir assetDir)
	{
		add(assetDir);
	}
	
	public void addTexture(Texture texture)
	{
		add(texture);
	}
	
	public void addAnimation(Animation animation)
	{
		add(animation);
	}
	
	public void addSound(Sound sound)
	{
		add(sound);
	}
	
	public void addMusic(Music music)
	{
		add(music);
	}
	
	public void addBitmapFont(BitmapFont font)
	{
		add(font);
	}
	
	public void addScheme(Scheme scheme)
	{
		add(scheme);
	}
	
	private void add(Asset asset)
	{
		assets.add(asset);
	}
	
	public List<Asset> getAssets()
	{
		return assets;
	}
}

class AssetDirImporter extends ContentImporter<AssetDir>
{
	@Override
	public AssetDir read(File file, ContentReaderContext context) throws IOException
	{
		return null;
	}
}