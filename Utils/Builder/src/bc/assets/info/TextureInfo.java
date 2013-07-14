package bc.assets.info;

import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;

import javax.imageio.ImageIO;

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
		ContentInfo<TextureInfo, Texture> info = new ContentInfo<TextureInfo, Texture>();
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

class TextureImporter extends ContentImporter<TextureInfo, Texture>
{
	@Override
	public Texture importAsset(TextureInfo info, AssetContext context) throws IOException
	{
		BufferedImage image = ImageIO.read(info.getFile());
		return new Texture(image);
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