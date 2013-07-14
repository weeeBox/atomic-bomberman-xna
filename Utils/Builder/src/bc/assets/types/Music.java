package bc.assets.types;

import java.io.File;

import bc.assets.Asset;

public class Music extends Asset 
{
	public Music()
	{
	}
	
	public Music(String name, File file)
	{
		super(name, file);
	}
}
