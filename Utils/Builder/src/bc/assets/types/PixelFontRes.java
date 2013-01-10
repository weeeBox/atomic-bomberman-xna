package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;

public class PixelFontRes extends Asset 
{	
	private static final AssetInfo info = new AssetInfo("PIXEL_FONT", "FNT", "PixelFontImporter", "PixelFontProcessor");
	
	public PixelFontRes()
	{
		super(info);
	}
	
	public PixelFontRes(String name, File file)
	{
		super(info, name, file);
	}
	
}