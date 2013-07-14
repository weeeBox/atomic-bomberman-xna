package bc.tasks;

import java.io.File;
import java.io.IOException;
import java.util.List;

import org.apache.tools.ant.BuildException;
import org.apache.tools.ant.Task;

import bc.assets.Asset;
import bc.assets.AssetContext;
import bc.assets.AssetDir;
import bc.assets.AssetInfo;
import bc.assets.AssetRegistry;
import bc.assets.ContentInfo;

public class ContentTask extends Task
{
	private static final String ASSET_BIN_EXT = ".b";
	
	private File codeFile;
	private File outputDir;

	private AssetDir rootDir;
	
	public ContentTask()
	{
	}
	
	@Override
	public void execute() throws BuildException
	{
		checkParams();
		try
		{
			process(rootDir, outputDir);
		}
		catch (IOException e)
		{
			throw new BuildException(e);
		}
	}
	
	private void process(AssetDir assetDir, File outputDir) throws IOException
	{
		AssetContext context = new AssetContext(new File(outputDir, assetDir.getName()));
		process(assetDir, context);
	}
	
	private void process(AssetDir assetDir, AssetContext context) throws IOException
	{
		List<AssetDir> dirs = assetDir.getDirs();
		for (AssetDir dir : dirs)
		{
			process(dir, context.createChild(dir.getName()));
		}
		
		List<AssetInfo> assets = assetDir.getAssets();
		for (AssetInfo asset : assets)
		{
			process(asset, context);
		}
	}
	
	private void process(AssetInfo assetInfo, AssetContext context) throws IOException
	{
		ContentInfo<? extends AssetInfo, ? extends Asset> info = findInfo(assetInfo.getClass());
		
		Asset asset = info.importer.importContent(assetInfo, context);

		if (info.processor != null)
			info.processor.processAsset(asset, context);
		
		File outputFile = new File(context.getOutputDir(), assetInfo.getName() + ASSET_BIN_EXT);
		info.writer.write(outputFile, asset, context);
		
		System.out.println(outputFile);
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
	
	private ContentInfo<?, ?> findInfo(Class<?> cls)
	{
		ContentInfo<?, ?> info = AssetRegistry.find(cls);
		if (info == null)
		{
			throw new BuildException("Can't find asset info for: " + cls);
		}
		
		return info;
	}
}
