package bc.assets.info;

import java.io.File;

import bc.assets.AssetInfo;

public class BitmapFontInfo extends AssetInfo 
{	
	public BitmapFontInfo()
	{
		super("PixelFont");
	}
	
	public BitmapFontInfo(String name, File file)
	{
		super(name, file);
	}
}