package bc.assets.info;

import java.io.IOException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import bc.assets.AssetContext;
import bc.assets.AssetInfo;
import bc.assets.AssetRegistry;
import bc.assets.BinaryWriter;
import bc.assets.ContentImporter;
import bc.assets.ContentInfo;
import bc.assets.ContentWriter;
import bc.assets.types.Scheme;
import bc.assets.types.Scheme.FieldBlocks;
import bc.assets.types.Scheme.FieldData;
import bc.assets.types.Scheme.PlayerLocationInfo;
import bc.assets.types.Scheme.PowerupInfo;
import bc.utils.filesystem.FileUtils;

public class SchemeInfo extends AssetInfo
{
	static
	{
		ContentInfo<SchemeInfo, Scheme> info = new ContentInfo<SchemeInfo, Scheme>();
		info.importer = new SchemeImporter();
		info.writer = new SchemeWriter();
		AssetRegistry.register(SchemeInfo.class, info);
	}

	public SchemeInfo()
	{
		super("Scheme");
	}
}

class SchemeImporter extends ContentImporter<SchemeInfo, Scheme>
{
	private static final int FIELD_WIDTH = 15;
	private static final int FIELD_HEIGHT = 11;
	private static final int MAX_PLAYERS = 10;
	private static final int MAX_POWERUPS = 13;

	@Override
	public Scheme importAsset(SchemeInfo info, AssetContext context) throws IOException
	{
		List<String> lines = FileUtils.readFile(info.getFile());
		return importAsset(lines);
	}

	private Scheme importAsset(List<String> lines)
	{
		NameReader nameReader = new NameReader();
		BrickDensityReader densityReader = new BrickDensityReader();
		FieldDataReader dataReader = new FieldDataReader(FIELD_WIDTH, FIELD_HEIGHT);
		PlayerLocationReader playersReader = new PlayerLocationReader(MAX_PLAYERS);
		PowerupInfoReader powerupReader = new PowerupInfoReader(MAX_POWERUPS);

		Map<String, SchemeSectionReader> lookup = new HashMap<String, SchemeSectionReader>();
		lookup.put("-N", nameReader);
		lookup.put("-B", densityReader);
		lookup.put("-R", dataReader);
		lookup.put("-S", playersReader);
		lookup.put("-P", powerupReader);

		for (int lineIndex = 0; lineIndex < lines.size(); ++lineIndex)
		{
			String line = lines.get(lineIndex).trim();

			if (line.length() == 0)
			{
				continue;
			}

			if (line.startsWith(";"))
			{
				continue;
			}

			if (line.charAt(0) == '-')
			{
				String[] tokens = line.split(",");
				String type = tokens[0].trim();

				if (lookup.containsKey(type))
				{
					SchemeSectionReader reader = lookup.get(type);

					String[] dataTokens = new String[tokens.length - 1];
					for (int i = 0; i < dataTokens.length; ++i)
					{
						dataTokens[i] = tokens[i + 1].trim();
					}

					reader.read(dataTokens);
				}
			}
		}

		Scheme scheme = new Scheme();
		scheme.name = nameReader.getName();
		scheme.brickDensity = densityReader.getDensity();
		scheme.fieldData = dataReader.getData();
		scheme.playerLocations = playersReader.getData();
		scheme.powerupInfo = powerupReader.getData();

		return scheme;
	}

	private static abstract class SchemeSectionReader
	{
		public abstract void read(String[] tokens);

		protected int readInt(String str)
		{
			return Integer.parseInt(str);
		}

		protected boolean readBool(String str)
		{
			return readInt(str) == 1;
		}
	}

	private static class NameReader extends SchemeSectionReader
	{
		private String name;

		@Override
		public void read(String[] tokens)
		{
			name = tokens[0];
		}

		public String getName()
		{
			return name;
		}
	}

	private static class BrickDensityReader extends SchemeSectionReader
	{
		private int density;

		@Override
		public void read(String[] tokens)
		{
			density = readInt(tokens[0]);
		}

		public int getDensity()
		{
			return density;
		}
	}

	private static class FieldDataReader extends SchemeSectionReader
	{
		private static final char BLOCK_SOLID = '#';
		private static final char BLOCK_BRICK = ':';
		private static final char BLOCK_BLANK = '.';

		private FieldData data;
		private int rowIndex;

		public FieldDataReader(int width, int height)
		{
			data = new FieldData(width, height);
		}

		@Override
		public void read(String[] tokens)
		{
			String dataString = tokens[1];

			for (int colIndex = 0; colIndex < dataString.length(); ++colIndex)
			{
				char chr = dataString.charAt(colIndex);
				switch (chr)
				{
				case BLOCK_SOLID:
				{
					data.set(colIndex, rowIndex, FieldBlocks.Solid);
					break;
				}
				case BLOCK_BRICK:
				{
					data.set(colIndex, rowIndex, FieldBlocks.Brick);
					break;
				}
				case BLOCK_BLANK:
				{
					data.set(colIndex, rowIndex, FieldBlocks.Blank);
					break;
				}
				}
			}

			++rowIndex;
		}

		public FieldData getData()
		{
			return data;
		}
	}

	private static class PlayerLocationReader extends SchemeSectionReader
	{
		private PlayerLocationInfo[] data;

		private int playerIndex;

		public PlayerLocationReader(int playersCount)
		{
			data = new PlayerLocationInfo[playersCount];
		}

		@Override
		public void read(String[] tokens)
		{
			PlayerLocationInfo info = new PlayerLocationInfo();

			info.index = readInt(tokens[0]);
			info.x = readInt(tokens[1]);
			info.y = readInt(tokens[2]);
			info.team = tokens.length > 3 ? readInt(tokens[3]) : -1;

			data[playerIndex] = info;
			++playerIndex;
		}

		public PlayerLocationInfo[] getData()
		{
			return data;
		}
	}

	private class PowerupInfoReader extends SchemeSectionReader
	{
		private PowerupInfo[] data;
		private int powerupIndex;

		public PowerupInfoReader(int dataSize)
		{
			data = new PowerupInfo[dataSize];
		}

		@Override
		public void read(String[] tokens)
		{
			PowerupInfo info = new PowerupInfo();
			info.powerupIndex = readInt(tokens[0]);
			info.bornWith = readBool(tokens[1]);
			info.hasOverride = readBool(tokens[2]);
			info.overrideValue = readInt(tokens[3]);
			info.forbidden = readBool(tokens[4]);

			data[powerupIndex] = info;

			++powerupIndex;
		}

		public PowerupInfo[] getData()
		{
			return data;
		}
	}
}

class SchemeWriter extends ContentWriter<Scheme>
{
	@Override
	protected void write(BinaryWriter output, Scheme scheme, AssetContext context) throws IOException
	{
		output.write(scheme.name);
		output.write(scheme.brickDensity);

		FieldData fieldData = scheme.fieldData;
		output.write(fieldData.getWidth());
		output.write(fieldData.getHeight());

		FieldBlocks[] blocks = fieldData.getDataArray();
		for (int i = 0; i < blocks.length; ++i)
		{
			output.write(blocks[i].ordinal());
		}

		PlayerLocationInfo[] playerInfo = scheme.playerLocations;
		output.write(playerInfo.length);
		for (int i = 0; i < playerInfo.length; ++i)
		{
			output.write(playerInfo[i].x);
			output.write(playerInfo[i].y);
			output.write(playerInfo[i].team);
		}

		PowerupInfo[] powerupInfo = scheme.powerupInfo;
		output.write(powerupInfo.length);
		for (int i = 0; i < powerupInfo.length; ++i)
		{
			output.write(powerupInfo[i].powerupIndex);
			output.write(powerupInfo[i].bornWith);
			output.write(powerupInfo[i].hasOverride);
			output.write(powerupInfo[i].overrideValue);
			output.write(powerupInfo[i].forbidden);
		}
	}
}