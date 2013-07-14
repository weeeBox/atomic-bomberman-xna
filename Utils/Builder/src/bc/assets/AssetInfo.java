package bc.assets;

import java.io.File;

public abstract class AssetInfo
{
	private String name;
	private File file;
	
	protected AssetInfo()
	{
	}
	
	protected AssetInfo(String name, File file)
	{
		setName(name);
		setFile(file);
	}
	
	public String getName()
	{
		return name;
	}
	
	public void setName(String name) 
	{
		this.name = name;
	}
	
	public File getFile() 
	{
		return file;
	}
	
	public File getParentFile()
	{
		return file.getParentFile();
	}
	
	public void setFile(File file) 
	{
		this.file = file;
	}

	@Override
	public String toString() 
	{
		return String.format("name='%s' path='%s'", name, file);
	}	
}
