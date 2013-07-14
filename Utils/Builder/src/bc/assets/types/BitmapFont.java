package bc.assets.types;

import java.io.File;

import bc.assets.Asset;

public class BitmapFont extends Asset 
{	
	public BitmapFont()
	{
	}
	
	public BitmapFont(String name, File file)
	{
		super(name, file);
	}
}