package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;

public class BitmapFont extends Asset 
{	
	private static AssetInfo info; // = new AssetInfo("PixelFont", "fnt", "PixelFontImporter", "PixelFontProcessor");
	
	public BitmapFont()
	{
		super(info);
	}
	
	public BitmapFont(String name, File file)
	{
		super(info, name, file);
	}
}