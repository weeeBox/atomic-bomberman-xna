package bc.tasks;

import java.io.File;
import java.io.IOException;
import java.util.List;

import org.apache.tools.ant.BuildException;
import org.apache.tools.ant.Task;

import bc.assets.Asset;
import bc.assets.AssetDir;
import bc.assets.AssetInfo;
import bc.assets.AssetRegistry;
import bc.assets.ContentImporter;
import bc.assets.ContentInfo;
import bc.assets.ContentProcessor;
import bc.assets.ContentWriter;

public class ContentTask extends Task
{
	private File codeFile;
	private File outputDir;

	private AssetDir rootDir;
	
	private List<Asset> assets;
	
	public ContentTask()
	{
	}
	
	@Override
	public void execute() throws BuildException
	{
		checkParams();
		try
		{
			process(rootDir);
		}
		catch (IOException e)
		{
			throw new BuildException(e);
		}
	}
	
	private void process(AssetDir assetDir) throws IOException
	{
		List<AssetDir> dirs = assetDir.getDirs();
		for (AssetDir dir : dirs)
		{
			process(dir);
		}
		
		List<AssetInfo> assets = assetDir.getAssets();
		for (AssetInfo asset : assets)
		{
			process(asset);
		}
	}
	
	private void process(AssetInfo assetInfo) throws IOException
	{
		ContentInfo<? extends Asset> info = findInfo(assetInfo.getClass());
		
		File source = assetInfo.getFile();
		if (!source.exists()) 
			throw new BuildException("File not exists: " + source);

		ContentImporter<? extends Asset> importer = info.importer;
		Asset asset = importer.importContent(source, null);

		ContentProcessor<? extends Asset> processor = info.processor;
		if (processor != null)
			processor.processAsset(asset, null);
		
		ContentWriter<? extends Asset> writer = info.writer;
	}

	private void checkParams()
	{
		if (codeFile == null)
			throw new BuildException("Missing 'codeFile' attribute");
		
		if (codeFile.exists() && codeFile.isDirectory())
			throw new BuildException("File is a directory: " + codeFile);
		
		if (outputDir == null)
			throw new BuildException("Missing 'outputDir' attribute");
		
		if (outputDir.exists() && !outputDir.isDirectory())
			throw new BuildException("File is not a directory: " + outputDir);

		if (rootDir == null)
			throw new BuildException("Add a root dir element");
	}

	public void addDir(AssetDir dir)
	{
		if (rootDir != null)
			throw new BuildException("Only 1 root dir allowed");
		
		rootDir = dir;
	}
	
	public void setCodeFile(File codeFile)
	{
		this.codeFile = codeFile;
	}
	
	public void setOutputDir(File outputDir)
	{
		this.outputDir = outputDir;
	}
	
	private ContentInfo<?> findInfo(Class<?> cls)
	{
		ContentInfo<?> info = AssetRegistry.find(cls);
		if (info == null)
		{
			throw new BuildException("Can't find asset info for: " + cls);
		}
		
		return info;
	}
}
