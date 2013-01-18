using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;
using BombermanCommon.Resources.Scheme;

namespace BombermanContentPipeline.Scheme
{
    [ContentImporter(".sch", DisplayName = "Scheme Importer", DefaultProcessor = "SchemeProcessor")]
    public class SchemeImporter : ContentImporter<SchemeInfo>
    {   
        private const int FIELD_WIDTH = 15;
        private const int FIELD_HEIGHT = 11;
        private const int MAX_PLAYERS = 10;
        private const int MAX_POWERUPS = 13;

        public override SchemeInfo Import(string filename, ContentImporterContext context)
        {
            String[] lines = File.ReadAllLines(filename);
            return Read(lines);
        }

        private SchemeInfo Read(String[] lines)
        {
            NameReader nameReader = new NameReader();
            BrickDensityReader densityReader = new BrickDensityReader();
            FieldDataReader dataReader = new FieldDataReader(FIELD_WIDTH, FIELD_HEIGHT);
            PlayerLocationReader playersReader = new PlayerLocationReader(MAX_PLAYERS);
            PowerupInfoReader powerupReader = new PowerupInfoReader(MAX_POWERUPS);

            Dictionary<char, SchemeSectionReader> lookup = new Dictionary<char, SchemeSectionReader>();
            lookup['N'] = nameReader;
            lookup['B'] = densityReader;
            lookup['R'] = dataReader;
            lookup['S'] = playersReader;
            lookup['P'] = powerupReader;

            for (int lineIndex = 0; lineIndex < lines.Length; ++lineIndex)
            {
                string line = lines[lineIndex].Trim();

                if (line.Length == 0)
                {
                    continue;
                }

                if (line.StartsWith(";"))
                {
                    continue;
                }

                if (line[0] == '-')
                {
                    char type = line[1];
                    SchemeSectionReader reader = null;
                    lookup.TryGetValue(type, out reader);

                    if (reader != null)
                    {
                        String[] tokens = line.Substring(3).Split(',');
                        reader.Read(tokens);
                    }
                }
            }

            SchemeInfo scheme = new SchemeInfo();
            scheme.name = nameReader.GetName();
            scheme.brickDensity = densityReader.GetDensity();
            scheme.fieldData = dataReader.GetData();
            scheme.playerLocations = playersReader.GetData();
            scheme.powerupInfo = powerupReader.GetData();

            return scheme;
        }
    }

    abstract class SchemeSectionReader
    {
        public abstract void Read(String[] tokens);

        protected int ReadInt(String str)
        {
            return int.Parse(str);
        }

        protected int ReadInt(String str, int defaultValue)
        {
            int value = defaultValue;
            int.TryParse(str.Trim(), out defaultValue);
            return value;
        }

        protected bool ReadBool(String str)
        {
            return ReadBool(str, false);
        }

        protected bool ReadBool(String str, bool defaultValue)
        {
            return ReadInt(str, 0) == 1;
        }
    }

    class NameReader : SchemeSectionReader
    {
        private String name;

        public override void Read(String[] tokens)
        {
            name = tokens[0];
        }

        public String GetName()
        {
            return name;
        }
    }

    class BrickDensityReader : SchemeSectionReader
    {
        private int density;

        public override void Read(String[] tokens)
        {
            density = ReadInt(tokens[0]);
        }

        public int GetDensity()
        {
            return density;
        }
    }

    class FieldDataReader : SchemeSectionReader
    {
        private const char BLOCK_SOLID = '#';
        private const char BLOCK_BRICK = ':';
        private const char BLOCK_BLANK = '.';

        private FieldData data;
        private int rowIndex;

        public FieldDataReader(int width, int height)
        {
            data = new FieldData(width, height);
        }

        public override void Read(String[] tokens)
        {
            String dataString = tokens[1];

            for (int colIndex = 0; colIndex < dataString.Length; ++colIndex)
            {
                char chr = dataString[colIndex];
                switch (chr)
                {
                    case BLOCK_SOLID:
                    {
                        data.Set(colIndex, rowIndex, FieldBlocks.Solid);
                        break;
                    }
                    case BLOCK_BRICK:
                    {
                        data.Set(colIndex, rowIndex, FieldBlocks.Brick);
                        break;
                    }
                    case BLOCK_BLANK:
                    {
                        data.Set(colIndex, rowIndex, FieldBlocks.Blank);
                        break;
                    }
                }
            }

            ++rowIndex;
        }

        public FieldData GetData()
        {
            return data;
        }
    }

    class PlayerLocationReader : SchemeSectionReader
    {
        private PlayerLocationInfo[] data;
        
        private int playerIndex;

        public PlayerLocationReader(int playersCount)
        {
            data = new PlayerLocationInfo[playersCount];
        }

        public override void Read(String[] tokens)
        {
            PlayerLocationInfo info = new PlayerLocationInfo();

            info.index = ReadInt(tokens[0]);
            info.x = ReadInt(tokens[1]);
            info.y = ReadInt(tokens[2]);
            info.team = tokens.Length > 3 ? ReadInt(tokens[3]) : -1;

            data[playerIndex] = info;
            ++playerIndex;
        }

        public PlayerLocationInfo[] GetData()
        {
            return data;
        }
    }

    class PowerupInfoReader : SchemeSectionReader
    {
        private PowerupInfo[] data;
        private int powerupIndex;

        public PowerupInfoReader(int dataSize)
        {
            data = new PowerupInfo[dataSize];
        }

        public override void Read(String[] tokens)
        {
            PowerupInfo info = new PowerupInfo();
            info.powerupIndex = ReadInt(tokens[0]);
            info.bornWith = ReadBool(tokens[1]);
            info.hasOverride = ReadBool(tokens[2]);
            info.overrideValue = ReadInt(tokens[3]);
            info.forbidden = ReadBool(tokens[4]);

            data[powerupIndex] = info;

            ++powerupIndex;
        }

        public PowerupInfo[] GetData()
        {
            return data;
        }
    }
}
