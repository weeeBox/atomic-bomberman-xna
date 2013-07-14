package bc.assets.types;

import java.util.ArrayList;
import java.util.List;

import bc.assets.Asset;

public class Animation extends Asset
{
	private String name;
	private Texture texture;
	private List<AnimationGroup> groups;

	public Animation(String name, Texture texture)
	{
		this.name = name;
		this.texture = texture;
		groups = new ArrayList<AnimationGroup>();
	}
	
	public String getName()
	{
		return name;
	}
	
	public Texture getTexture()
	{
		return texture;
	}
	
	public void addGroup(AnimationGroup group)
	{
		groups.add(group);
	}
	
	public List<AnimationGroup> getGroups()
	{
		return groups;
	}
	
	public static class AnimationGroup
	{
		private String name;
		private List<AnimationFrame> frames;
		
		public AnimationGroup(String name)
		{
			this.name = name;
			frames = new ArrayList<AnimationFrame>();
		}
		
		public void addFrame(AnimationFrame frame)
		{
			frames.add(frame);
		}
		
		public List<AnimationFrame> getFrames()
		{
			return frames;
		}
		
		public String getName()
		{
			return name;
		}
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
