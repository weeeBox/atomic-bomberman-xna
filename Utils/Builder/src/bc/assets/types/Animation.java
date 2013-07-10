package bc.assets.types;

import java.io.File;

import bc.assets.Asset;
import bc.assets.AssetInfo;
import bc.utils.filesystem.FileUtils;

public class Animation extends Asset
{
	private static AssetInfo info = new AssetInfo("Animation", "ani", "AnimationImporter");
	
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
		addChildRes(new TextureAsset(textureName, textureFile));
		
		postProcess();
	}
	
	public void setTexture(File texture)
	{
		this.textureFile = texture;
	}
}
