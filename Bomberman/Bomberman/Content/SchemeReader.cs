using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BombermanCommon.Resources.Scheme;
using BomberEngine.Core.Assets;
using System.IO;

namespace Bomberman.Content
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content
    /// Pipeline to read the specified data type from binary .xnb format.
    /// 
    /// Unlike the other Content Pipeline support classes, this should
    /// be a part of your main game project, and not the Content Pipeline
    /// Extension Library project.
    /// </summary>
    public class SchemeReader : AssetReader
    {
        private const int FIELD_WIDTH = 15;
        private const int FIELD_HEIGHT = 11;
        private const int MAX_PLAYERS = 10;
        private const int MAX_POWERUPS = 13;

        public Asset Read(Stream stream)
        {
            List<String> lines = ReadLines(stream);

            NameReader nameReader = new NameReader();
            BrickDensityReader densityReader = new BrickDensityReader();
            FieldDataReader dataReader = new FieldDataReader(FIELD_WIDTH, FIELD_HEIGHT);
            PlayerLocationReader playersReader = new PlayerLocationReader(MAX_PLAYERS);
            PowerupInfoReader powerupReader = new PowerupInfoReader(MAX_POWERUPS);

            Dictionary<String, SchemeSectionReader> lookup = new Dictionary<String, SchemeSectionReader>();
            lookup["-N"] = nameReader;
            lookup["-B"] = densityReader;
            lookup["-R"] = dataReader;
            lookup["-S"] = playersReader;
            lookup["-P"] = powerupReader;

            for (int lineIndex = 0; lineIndex < lines.Count; ++lineIndex)
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
                    String[] tokens = line.Split(',');
                    String type = tokens[0].Trim();

                    if (lookup.ContainsKey(type))
                    {
                        SchemeSectionReader reader = lookup[type];

                        String[] dataTokens = new String[tokens.Length - 1];
                        for (int i = 0; i < dataTokens.Length; ++i)
                        {
                            dataTokens[i] = tokens[i + 1].Trim();
                        }

                        reader.Read(dataTokens);
                    }
                }
            }

            Scheme scheme = new Scheme();
            scheme.name = nameReader.GetName();
            scheme.brickDensity = densityReader.GetDensity();
            scheme.fieldData = dataReader.GetData();
            scheme.playerLocations = playersReader.GetData();
            scheme.powerupInfo = powerupReader.GetData();

            return scheme;
        }

        private List<String> ReadLines(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                List<String> lines = new List<String>();
                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }

                return lines;
            }
        }
    }

    abstract class SchemeSectionReader
    {
        public abstract void Read(String[] tokens);

        protected int ReadInt(String str)
        {
            return int.Parse(str);
        }

        protected bool ReadBool(String str)
        {
            return ReadInt(str) == 1;
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
