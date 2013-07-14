package bc.assets;

import java.io.File;

public class AssetContext
{
	private File outputDir;
	
	public AssetContext(File outputFile)
	{
		this.outputDir = outputFile;
	}
	
	public File getOutputDir()
	{
		return outputDir;
	}
}
