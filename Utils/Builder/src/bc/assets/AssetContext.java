package bc.assets;

import java.io.File;

public class AssetContext
{
	private File outputDir;
	
	public AssetContext(File outputFile)
	{
		this.outputDir = outputFile;
	}
	
	public AssetContext createChild(String name)
	{
		return new AssetContext(new File(outputDir, name));
	}
	
	public File getOutputDir()
	{
		return outputDir;
	}
}
