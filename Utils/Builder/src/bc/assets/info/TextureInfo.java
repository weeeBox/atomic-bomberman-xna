package bc.assets.info;

import java.io.File;
import java.io.IOException;

import bc.assets.AssetContext;
import bc.assets.AssetInfo;
import bc.assets.AssetRegistry;
import bc.assets.BinaryWriter;
import bc.assets.ContentImporter;
import bc.assets.ContentInfo;
import bc.assets.ContentWriter;
import bc.assets.types.Texture;

public class TextureInfo extends AssetInfo 
{
	static
	{
		ContentInfo<Texture> info = new ContentInfo<Texture>();
		info.importer = new TextureImporter();
		info.writer = new TextureWriter();
		
		AssetRegistry.register(TextureInfo.class, info);
	}
	
	public TextureInfo()
	{
	}
	
	public TextureInfo(String name, File file)
	{
		super(name, file);
	}
}

class TextureImporter extends ContentImporter<Texture>
{
	@Override
	public Texture importAsset(File file, AssetContext context) throws IOException
	{
		throw new Error("Implement me"); // FIXME
	}
}

class TextureWriter extends ContentWriter<Texture>
{
	@Override
	protected void write(BinaryWriter output, Texture t, AssetContext context)
	{
		throw new Error("Implement me"); // FIXME
	}
}