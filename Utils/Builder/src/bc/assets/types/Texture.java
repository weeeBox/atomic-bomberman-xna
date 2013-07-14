package bc.assets.types;

import java.io.File;
import java.io.IOException;

import bc.assets.Asset;
import bc.assets.AssetInfo;
import bc.assets.BinaryWriter;
import bc.assets.ContentImporter;
import bc.assets.ContentReaderContext;
import bc.assets.ContentWriter;
import bc.assets.ContentWriterContext;

public class Texture extends Asset 
{
	private static AssetInfo info;
	
	static
	{
		info = new AssetInfo();
		info.importer = new TextureImporter();
		info.writer = new TextureWriter();
	}
	
	public Texture()
	{
		super(info);
	}
	
	public Texture(String name, File file)
	{
		super(info, name, file);
	}
}

class TextureImporter extends ContentImporter<Texture>
{
	@Override
	public Texture read(File file, ContentReaderContext context) throws IOException
	{
		throw new Error("Implement me"); // FIXME
	}
}

class TextureWriter extends ContentWriter<Texture>
{
	@Override
	protected void write(BinaryWriter output, Texture t, ContentWriterContext context)
	{
		throw new Error("Implement me"); // FIXME
	}
}