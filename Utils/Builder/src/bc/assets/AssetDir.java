package bc.assets;

import java.io.File;
import java.io.IOException;

public class AssetDir extends Asset
{
	private static final AssetInfo info;
	
	static
	{
		info = new AssetInfo();
		info.importer = new AssetDirImporter();
	}
	
	protected AssetDir(String name, File file)
	{
		super(info, name, file);
	}
}

class AssetDirImporter extends ContentImporter<AssetDir>
{
	@Override
	public AssetDir read(File file, ContentReaderContext context) throws IOException
	{
		return null;
	}
}