package bc.assets;

import java.util.ArrayList;
import java.util.List;

import bc.assets.info.AnimationInfo;
import bc.assets.info.BitmapFontInfo;
import bc.assets.info.MusicInfo;
import bc.assets.info.SchemeInfo;
import bc.assets.info.SoundInfo;
import bc.assets.info.TextureInfo;

public class AssetPackage
{
	private String name;
	
	private List<AssetInfo> assets = new ArrayList<AssetInfo>();
	
	public void setName(String name)
	{
		this.name = name;
	}
	
	public void addTexture(TextureInfo texture)
	{
		add(texture);
	}
	
	public void addAnimation(AnimationInfo animation)
	{
		add(animation);
	}
	
	public void addSound(SoundInfo sound)
	{
		add(sound);
	}
	
	public void addMusic(MusicInfo music)
	{
		add(music);
	}
	
	public void addBitmapFont(BitmapFontInfo font)
	{
		add(font);
	}
	
	public void addScheme(SchemeInfo scheme)
	{
		add(scheme);
	}
	
	private void add(AssetInfo asset)
	{
		assets.add(asset);
	}
	
	public String getName()
	{
		return name;
	}

	public List<AssetInfo> getAssets()
	{
		return assets;
	}
}