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
import bc.utils.filesystem.FileUtils;

public class Animation extends Asset
{
	private static AssetInfo info;// = new AssetInfo("Animation", "ani", "AnimationImporter");
	
	static
	{
		info = new AssetInfo();
		info.importer = new AnimationImporter();
		info.writer = new AnimationWriter();
	}
	
	private File textureFile;
	
	public Animation()
	{
		super(info);
	}
	
	public Animation(String name, File file)
	{
		super(info, name, file);
	}

	@Override
	public void process()
	{
		preProcess();

		if (textureFile == null)
		{
			File sourceFile = getSourceFile();
			textureFile = new File(sourceFile.getParentFile(), FileUtils.getFilenameNoExt(sourceFile) + "_tex.png");
		}
		
		String textureName = FileUtils.getFilenameNoExt(textureFile);
		addChildRes(new Texture(textureName, textureFile));
		
		postProcess();
	}
	
	public void setTexture(File texture)
	{
		this.textureFile = texture;
	}
}

class AnimationImporter extends ContentImporter<Animation>
{
	@Override
	public Animation read(File file, ContentReaderContext context) throws IOException
	{
		throw new Error("Implement me"); // FIXME
	}
}

class AnimationWriter extends ContentWriter<Animation>
{
	@Override
	protected void write(BinaryWriter output, Animation t, ContentWriterContext context)
	{
		throw new Error("Implement me"); // FIXME
	}
}

