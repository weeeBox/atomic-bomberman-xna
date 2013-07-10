package bc.assets;

import java.io.File;

import bc.tasks.ContentProjTask;
import bc.utils.filesystem.FileUtils;

public abstract class Asset
{
	public enum BuildAction
	{
		None,
		Compile
	}
	
	public enum CopyToOutputDirectory
	{
		DontCopy,
		IfNewer,
		Always
	}
	
	private AssetPackage pack;
	
	private String name;
	
	private File sourceFile;
	private File destFile;
	
	private AssetInfo assetInfo;
	
	private BuildAction buildAction = BuildAction.Compile;
	private CopyToOutputDirectory copyToOutputDirectory = CopyToOutputDirectory.DontCopy;
	
	protected Asset(AssetInfo assetInfo)
	{
		if (assetInfo == null)
		{
			throw new IllegalArgumentException("Asset info is null");
		}
		this.assetInfo = assetInfo;
	}
	
	protected Asset(AssetInfo assetInfo, String name, File file)
	{
		this(assetInfo);
		
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
		ContentProjTask.projSync.addResource(this);
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
	
	public String getLongName() 
	{
		return getResourceTypePrefix() + "_" + name;
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
	
	public String getImporter()
	{
		return assetInfo.getImporter();
	}
	
	public String getProcessor()
	{
		return assetInfo.getProcessor();
	}
	
	public boolean hasProcessor()
	{
		return getProcessor() != null;
	}
	
	public String getResourceType()
	{
		return assetInfo.getType();
	}
	
	public String getResourceTypePrefix()
	{
		return assetInfo.getTypePrefix();
	}

	public BuildAction getBuildAction()
	{
		return buildAction;
	}
	
	public void setBuildAction(BuildAction buildAction)
	{
		this.buildAction = buildAction;
	}
	
	public CopyToOutputDirectory getCopyToOutputDirectory()
	{
		return copyToOutputDirectory;
	}
	
	public void setCopyToOutputDirectory(CopyToOutputDirectory copyToOutputDirectory)
	{
		this.copyToOutputDirectory = copyToOutputDirectory;
	}
	
	@Override
	public String toString() 
	{
		return String.format("name='%s' path='%s'", name, sourceFile);
	}	
}
