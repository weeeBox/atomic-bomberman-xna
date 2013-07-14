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
import bc.assets.types.Animation;

public class AnimationInfo extends AssetInfo
{
	static
	{
		ContentInfo<Animation> info = new ContentInfo<Animation>();
		info.importer = new AnimationImporter();
		info.writer = new AnimationWriter();
		
		AssetRegistry.register(AnimationInfo.class, info);
	}
	
	private File textureFile;
	
	public AnimationInfo()
	{
	}
	
	public AnimationInfo(String name, File file)
	{
		super(name, file);
	}

	public void setTexture(File texture)
	{
		this.textureFile = texture;
	}
	
	public File getTextureFile()
	{
		return textureFile;
	}
}

class AnimationImporter extends ContentImporter<Animation>
{
	@Override
	public Animation importContent(File file, AssetContext context) throws IOException
	{
		throw new Error("Implement me"); // FIXME
	}
}

class AnimationWriter extends ContentWriter<Animation>
{
	@Override
	protected void write(BinaryWriter output, Animation t, AssetContext context)
	{
		throw new Error("Implement me"); // FIXME
	}
}

