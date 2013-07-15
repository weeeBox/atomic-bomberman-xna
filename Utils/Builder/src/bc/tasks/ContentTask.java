package bc.tasks;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.apache.tools.ant.BuildException;
import org.apache.tools.ant.Task;

import bc.assets.Asset;
import bc.assets.AssetContext;
import bc.assets.AssetInfo;
import bc.assets.AssetPackage;
import bc.assets.AssetRegistry;
import bc.assets.ContentInfo;

public class ContentTask extends Task
{
	private File codeFile;
	private File outputDir;

	private List<AssetPackage> packs;
	
	public ContentTask()
	{
		packs = new ArrayList<AssetPackage>();
	}
	
	@Override
	public void execute() throws BuildException
	{
		checkParams();
		try
		{
			process(outputDir, packs);
			generateCode(codeFile, packs);
		}
		catch (IOException e)
		{
			throw new BuildException(e);
		}
	}

	private void process(File outputDir, List<AssetPackage> packs) throws IOException
	{
		AssetContext context = new AssetContext(outputDir);
		for (AssetPackage pack : packs)
		{
			process(pack, context);
		}
	}
	
	private void process(AssetPackage assetDir, AssetContext context) throws IOException
	{
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
		
		assetInfo.extractRelativePath(getProject().getBaseDir());
		
		File outputFile = new File(context.getOutputDir(), assetInfo.getRelativePath());
		info.writer.write(outputFile, asset, context);
		
		System.out.println(outputFile);
	}
	
	private void generateCode(File file, List<AssetPackage> packs) throws IOException
	{
		new CodeFileGenerator().generate(file, packs);
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
	}

	public void addPack(AssetPackage pack)
	{
		packs.add(pack);
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
