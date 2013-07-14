package bc.assets;

import java.util.ArrayList;
import java.util.List;

import bc.assets.types.Animation;
import bc.assets.types.BitmapFont;
import bc.assets.types.Music;
import bc.assets.types.Scheme;
import bc.assets.types.Sound;
import bc.assets.types.Texture;

public class AssetDir
{
	private List<AssetDir> dirs = new ArrayList<AssetDir>();
	private List<Asset> assets = new ArrayList<Asset>();
	
	public void addDir(AssetDir assetDir)
	{
		dirs.add(assetDir);
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