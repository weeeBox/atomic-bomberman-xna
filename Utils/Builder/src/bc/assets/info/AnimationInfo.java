package bc.assets.info;

import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import java.util.List;

import javax.imageio.ImageIO;

import org.apache.tools.ant.BuildException;
import org.dom4j.Document;
import org.dom4j.Element;
import org.dom4j.io.SAXReader;

import bc.assets.AssetContext;
import bc.assets.AssetInfo;
import bc.assets.AssetRegistry;
import bc.assets.BinaryWriter;
import bc.assets.ContentImporter;
import bc.assets.ContentInfo;
import bc.assets.ContentWriter;
import bc.assets.types.Animation;
import bc.assets.types.Animation.AnimationFrame;
import bc.assets.types.Animation.AnimationGroup;
import bc.assets.types.Texture;

public class AnimationInfo extends AssetInfo
{
	static
	{
		ContentInfo<AnimationInfo, Animation> info = new ContentInfo<AnimationInfo, Animation>();
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

class AnimationImporter extends ContentImporter<AnimationInfo, Animation>
{
	@Override
	public Animation importAsset(AnimationInfo info, AssetContext context) throws IOException
	{
		Element root = readDocument(info.getFile()).getRootElement();
		
		Texture texture = readTexture(root, info.getParentFile());
		
		String name = attributeString(root, "name");
		Animation animation = new Animation(name, texture);
		
		List<Element> animationElements = root.elements("animation");
		for (Element animationElement : animationElements)
		{
			animation.addGroup(readGroup(animationElement));
		}
		
		return animation;
	}

	private Texture readTexture(Element element, File baseDir) throws IOException
	{
		String textureName = attributeString(element, "file");
		File textureFile = new File(baseDir, textureName);
		if (!textureFile.exists())
			throw new BuildException("File not exists: " + textureFile);
		
		BufferedImage textureImage = ImageIO.read(new File(baseDir, textureName));
		Texture texture = new Texture(textureImage);
		return texture;
	}

	private AnimationGroup readGroup(Element element)
	{
		String name = attributeString(element, "name");
		
		AnimationGroup group = new AnimationGroup(name);
		
		List<Element> frameElements = element.elements("frame");
		for (Element frameElement : frameElements)
		{
			group.addFrame(readFrame(frameElement));
		}
		
		return group;
	}
	
	private AnimationFrame readFrame(Element element)
	{
		AnimationFrame frame = new AnimationFrame();
		
		frame.x = attributeInt(element, "x");
		frame.y = attributeInt(element, "y");
		frame.ox = attributeInt(element, "ox");
		frame.oy = attributeInt(element, "oy");
		frame.w = attributeInt(element, "w");
		frame.h = attributeInt(element, "h");
		
		return frame;
	}

	private Document readDocument(File file)
	{
		try
		{
			return new SAXReader().read(file);
		}
		catch (Exception e)
		{
			throw new BuildException(e);
		}
	}
	
	private String attributeString(Element element, String name)
	{
		String value = element.attributeValue(name);
		if (value == null)
			throw new BuildException("Missing '" + name + "' attribute");
		
		return value;
	}
	
	private int attributeInt(Element element, String name)
	{
		String value = attributeString(element, name);
		try
		{
			return Integer.parseInt(value);
		}
		catch (NumberFormatException e)
		{
			throw new BuildException(e);
		}
	}
}

class AnimationWriter extends ContentWriter<Animation>
{
	@Override
	protected void write(BinaryWriter output, Animation t, AssetContext context)
	{
	}
}

