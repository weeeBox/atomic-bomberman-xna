package bc.assets;

import java.io.File;

import bc.tasks.ContentProjTask;
import bc.utils.filesystem.FileUtils;

public abstract class Asset
{
	private AssetPackage pack;
	
	private String name;
	
	private File sourceFile;
	private File destFile;
	
	protected Asset()
	{
	}
	
	protected Asset(String name, File file)
	{
		setName(name);
		setFile(file);
	}
	
	public void process()
	{
		preProcess();
		postProcess();
	}

	protected void preProcess()
	{
		destFile = sourceFile;
	}
	
	protected void postProcess()
	{
		addToSync();
	}
	
	protected void addChildRes(Asset res)
	{
		res.destFile = res.sourceFile;
		res.addToSync();
	}
	
	protected void addToSync()
	{
		ContentProjTask.fileSync.addFile(getDestFile());
	}

	public AssetPackage getPackage() 
	{
		return pack;
	}

	public void setPackage(AssetPackage parent) 
	{
		this.pack = parent;
	}

	public String getName()
	{
		return name;
	}
	
	public void setName(String name) 
	{
		this.name = name;
	}
	
	public String getShortName()
	{
		return FileUtils.getFilenameNoExt(destFile);
	}
	
	public File getSourceFile() 
	{
		return sourceFile;
	}
	
	public File getDestFile()
	{
		return destFile;
	}

	public void setDestFile(File destFile)
	{
		this.destFile = destFile;
	}

	public void setFile(File file) 
	{
		this.sourceFile = file;
	}

	@Override
	public String toString() 
	{
		return String.format("name='%s' path='%s'", name, sourceFile);
	}	
}
