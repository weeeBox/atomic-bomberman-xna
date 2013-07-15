package bc.assets.info;

import java.awt.image.BufferedImage;
import java.io.ByteArrayOutputStream;
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
		super("Animation");
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
	protected void write(BinaryWriter output, Animation animation, AssetContext context) throws IOException
	{
		output.write(animation.getName());

		List<AnimationGroup> groups = animation.getGroups();
		write(output, groups);
		
		Texture texture = animation.getTexture();
		write(output, texture);
	}

	private void write(BinaryWriter output, List<AnimationGroup> groups) throws IOException
	{
		output.write(groups.size());
		for (AnimationGroup group : groups)
		{
			write(output, group);
		}
	}

	private void write(BinaryWriter output, AnimationGroup group) throws IOException
	{
		output.write(group.getName());
		List<AnimationFrame> frames = group.getFrames();
		output.write(frames.size());
		
		for (AnimationFrame frame : frames)
		{
			write(output, frame);
		}
	}

	private void write(BinaryWriter output, AnimationFrame frame) throws IOException
	{
		output.write(frame.x);
		output.write(frame.y);
		output.write(frame.ox);
		output.write(frame.oy);
		output.write(frame.w);
		output.write(frame.h);
	}

	private void write(BinaryWriter output, Texture texture) throws IOException
	{
		ByteArrayOutputStream bos = new ByteArrayOutputStream();
		ImageIO.write(texture.getImage(), "png", bos);
		
		byte[] data = bos.toByteArray();
		output.write(data.length);
		output.write(data);
	}
}

