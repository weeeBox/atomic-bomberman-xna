package bc.assets.types;

import java.io.File;
import java.io.IOException;

import bc.assets.Asset;
import bc.assets.AssetInfo;
import bc.assets.AssetRegistry;
import bc.assets.BinaryWriter;
import bc.assets.ContentImporter;
import bc.assets.ContentReaderContext;
import bc.assets.ContentWriter;
import bc.assets.ContentWriterContext;

public class Texture extends Asset 
{
	static
	{
		AssetInfo info = new AssetInfo();
		info.importer = new TextureImporter();
		info.writer = new TextureWriter();
		
		AssetRegistry.register(Texture.class, info);
	}
	
	public Texture()
	{
	}
	
	public Texture(String name, File file)
	{
		super(name, file);
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