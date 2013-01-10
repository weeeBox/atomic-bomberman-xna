package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;

public class TextureAsset extends Asset 
{
	private static final AssetInfo info = new AssetInfo("Texture", "tex", "TextureImporter", "TextureProcessor");
	
	private boolean dxtCompressed;
	
	public TextureAsset()
	{
		super(info);
	}
	
	public TextureAsset(String name, File file)
	{
		super(info, name, file);
	}
	
	public boolean isDxtCompressed()
	{
		return dxtCompressed;
	}

	public void setDxtCompressed(boolean dxtCompressed)
	{
		this.dxtCompressed = dxtCompressed;
	}
}
