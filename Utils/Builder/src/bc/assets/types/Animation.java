package bc.assets.types;

import java.util.ArrayList;
import java.util.List;

import bc.assets.Asset;

public class Animation extends Asset
{
	private String name;
	private Texture texture;
	private List<AnimationFrame> frames;

	public Animation(String name, Texture texture)
	{
		this.name = name;
		this.texture = texture;
		frames = new ArrayList<AnimationFrame>();
	}
	
	public String getName()
	{
		return name;
	}
	
	public Texture getTexture()
	{
		return texture;
	}
	
	public void addFrame(AnimationFrame frame)
	{
		frames.add(frame);
	}
	
	public List<AnimationFrame> getFrames()
	{
		return frames;
	}
	
	public static class AnimationFrame
	{
		public int x;
		public int y;
		public int ox;
		public int oy;
		public int w;
		public int h;
	}
}
