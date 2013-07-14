package bc.assets.types;

import bc.assets.Asset;

public class Scheme extends Asset
{
	public String name;
	public FieldData fieldData;
	public PlayerLocationInfo[] playerLocations;
	public PowerupInfo[] powerupInfo;
	public int brickDensity;

	public static class FieldData
	{
		private FieldBlocks[] data;
		private int width;
		private int height;

		public FieldData(int width, int height)
		{
			this(width, height, new FieldBlocks[width * height]);
		}

		public FieldData(int width, int height, FieldBlocks[] data)
		{
			if (data.length != width * height)
			{
				throw new IllegalArgumentException("Invalid data array size");
			}

			this.width = width;
			this.height = height;

			this.data = data;
		}

		public FieldBlocks get(int x, int y)
		{
			int index = y * width + x;
			return data[index];
		}

		public void set(int x, int y, FieldBlocks block)
		{
			int index = y * width + x;
			data[index] = block;
		}

		public int getWidth()
		{
			return width;
		}

		public int getHeight()
		{
			return height;
		}

		public FieldBlocks[] getDataArray()
		{
			return data;
		}
	}

	public enum FieldBlocks
	{
		Blank, // no block
		Brick, // breakable brick
		Solid, // solid brick
		Count
	}

	public static class PlayerLocationInfo
	{
		public int index;
		public int x;
		public int y;
		public int team;
	}

	public static class PowerupInfo
	{
		public int powerupIndex;
		public boolean bornWith;
		public boolean hasOverride;
		public int overrideValue;
		public boolean forbidden;
	}
}