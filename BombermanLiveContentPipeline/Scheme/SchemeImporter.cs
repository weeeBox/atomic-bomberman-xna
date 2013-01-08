using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;
using BombermanLiveCommon.Resources.Scheme;

namespace BombermanLiveContentPipeline.Scheme
{
    [ContentImporter(".sch", DisplayName = "Scheme Importer", DefaultProcessor = "SchemeProcessor")]
    public class SchemeImporter : ContentImporter<SchemeResource>
    {
        /// <summary>
        /// Number of tiles in one line of the field.
        /// </summary>
        private const int FIELD_WIDTH = 15;

        /// <summary>
        /// Number of tiles in one column of the field.
        /// </summary>
        private const int FIELD_HEIGHT = 11;

        /// <summary>
        /// Maximum number of players playing simultanously
        /// </summary>
        private const int MAX_PLAYERS = 10;

        public override SchemeResource Import(string filename, ContentImporterContext context)
        {
            string[] lines = File.ReadAllLines(filename);
            return Read(lines);
        }

        private SchemeResource Read(string[] lines)
        {
            SchemeResource scheme = new SchemeResource();

            FieldData fieldData = new FieldData(FIELD_WIDTH, FIELD_HEIGHT);
            PlayerInfo[] playerLocations = new PlayerInfo[MAX_PLAYERS];

            int maxPowerups = (int)EnumPowerups.PU_NUMBER_OF;
            PowerupInfo[] powerupInfo = new PowerupInfo[maxPowerups];

            scheme.FieldData = fieldData;
            scheme.PlayerLocations = playerLocations;
            scheme.PowerupInfo = powerupInfo;

            for (int lineIndex = 0; lineIndex < lines.Length; ++lineIndex)
            {
                string line = lines[lineIndex].Trim();

                if (line.Length == 0)
                {
                    continue;
                }


                if (line[0] == '-')
                {
                    switch (line[1])
                    {
                        case 'V':
                            {
                                scheme.Version = ParseInt(line.Substring(3, 2), -1);
                                break;
                            }

                        case 'N':
                            {
                                scheme.Name = line.Substring(3);
                                break;
                            }

                        case 'B':
                            {
                                int brickDensity = ParseInt(line.Substring(3, 2), -1);

                                if (brickDensity < 0 || brickDensity > 100)
                                {
                                    throw new InvalidContentException("Invalid brick density value: " + brickDensity);
                                }

                                break;
                            }

                        case 'R':
                            {
                                if (line.Length < 21)
                                {
                                    throw new InvalidContentException("Invalid line: " + line);
                                }

                                int y = ParseInt(line.Substring(3, 2), -1);

                                if (y < 0 || y > 10)
                                {
                                    throw new InvalidContentException("Invalid y value: " + y);
                                }

                                for (int x = 0; x < FIELD_WIDTH; ++x)
                                {
                                    switch (line[6 + x])
                                    {
                                        case '#':
                                            fieldData.Set(x, y, EnumBlocks.BLOCK_SOLID);
                                            break;
                                        case ':':
                                            fieldData.Set(x, y, EnumBlocks.BLOCK_BREAKABLE);
                                            break;
                                        case '.':
                                            break;
                                        default:
                                            throw new InvalidContentException("Invalid block type: '" + line[6 + x] + "'");
                                    }
                                }
                                break;
                            }

                        case 'S': // PlayerLocations
                            {
                                string positionLine = line.Substring(3);
                                string[] result = positionLine.Split(',');

                                if (result.Length < 3)
                                {
                                    throw new InvalidContentException("Invalid player info: " + result);
                                }

                                int plno = ParseInt(result[0], -1);
                                int x = ParseInt(result[1], -1);
                                int y = ParseInt(result[2], -1);
                                int team = -1;

                                // if team is specified...
                                if (result.Length == 4)
                                {
                                    team = ParseInt(result[3], -1);
                                }

                                playerLocations[plno].x = x;
                                playerLocations[plno].y = y;
                                playerLocations[plno].team = team;
                                break;
                            }

                        case 'P': // Powerup Infos
                            {
                                String powerupLine = line.Substring(3);
                                string[] result = powerupLine.Split(',');

                                if (result.Length < 6)
                                {
                                    throw new InvalidContentException("Invalid powerups info: " + powerupLine);
                                }

                                int puno = ParseInt(result[0], -1);
                                int bw = ParseInt(result[1], -1);
                                int has_ov = ParseInt(result[2]);
                                int ov = ParseInt(result[3]);
                                int fb = ParseInt(result[4]);

                                powerupInfo[puno].bornWith = bw;
                                powerupInfo[puno].hasOverride = has_ov == 0 ? false : true;
                                powerupInfo[puno].overrideValue = ov;
                                powerupInfo[puno].forbidden = fb == 0 ? false : true;
                                break;
                            }

                        // version 3:

                        case 'A':      // Arrows
                            //                    -A,E,1,0
                            //                    -A,E,1,1
                            //                    -A,E,1,2
                            //                    -A,W,1,3
                            //                    -A,N,1,4
                            //                    -A,E,1,5
                            break;

                        case 'C':     // Conveyer Belt
                            //                    -C,E,1,2
                            //                    -C,S,2,2
                            //                    -C,W,3,2
                            //                    -C,S,4,2
                            //                    -C,N,5,2
                            //                    -C,W,6,2
                            //                    -C,E,7,2
                            break;

                        case 'T':      // Trampoline
                            //                   -T,2,2
                            //                   -T,-3,2
                            //                   -T,2,-3
                            //                   -T,-3,-3
                            //                   -T,H,H
                            //                   -T,H,H
                            //                   -T,H,H
                            //                   -T,H,H
                            break;

                        case 'W':     // Warp Holes
                            //-W,1,0, 0, 0,7
                            //-W,1,1, 0, 5,8
                            //-W,1,2, 0,10,4
                            //-W,1,3, 7, 0,5
                            break;

                        case 'I':     // Ice
                            //                    -I,1,2
                            //                    -I,2,2
                            //                    -I,3,2
                            //                    -I,4,2
                            //                    -I,5,2
                            //                    -I,6,2
                            //                    -I,7,2
                            break;

                    }

                }
            }

            return scheme;
        }

        private int ParseInt(String str)
        {
            return ParseInt(str, -1);
        }

        private int ParseInt(String str, int defaultValue)
        {
            int value = defaultValue;
            int.TryParse(str, out defaultValue);
            return value;
        }
    }
}
