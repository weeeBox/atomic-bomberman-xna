package bc.assets;

import java.io.File;

import bc.utils.filesystem.FileUtils;

public abstract class AssetInfo
{
	private String runtimeType;
	
	private String id;
	
	private File file;
	
	private String relativePath;
	
	protected AssetInfo(String runtimeType)
	{
		this.runtimeType = runtimeType;
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
	
	public void extractRelativePath(File baseDir)
	{
		relativePath = file.getPath().substring(baseDir.getPath().length() + 1);
		id = FileUtils.getFilenameNoExt(relativePath).replace(File.separator, "_");
	}
	
	public String getRelativePath()
	{
		return relativePath;
	}
	
	public String getId()
	{
		return id;
	}
	
	public String getRuntimeType()
	{
		return runtimeType;
	}
}
