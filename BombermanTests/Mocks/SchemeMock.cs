using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BombermanCommon.Resources.Scheme;
using BombermanCommon.Resources;
using Bomberman.Content;

namespace BombermanTests.Mocks
{
    public class SchemeMock : Scheme
    {
        const int Width = 15;
        const int Height = 11;

        public SchemeMock(String name, int brickDensity)
        {
            this.name = name;
            this.brickDensity = brickDensity;

            FieldBlocks[] blocks = {
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Solid, 	FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Solid, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
	            FieldBlocks.Solid, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Solid, 	FieldBlocks.Brick, 	FieldBlocks.Solid, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Solid, //
	            FieldBlocks.Brick, 	FieldBlocks.Solid, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Brick, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Solid, 	FieldBlocks.Brick, //
	            FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Solid, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Solid, 	FieldBlocks.Brick, 	FieldBlocks.Brick, //
	            FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Solid, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Solid, 	FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Brick, //
	            FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Brick, //
	            FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Solid, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Solid, 	FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Brick, //
	            FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Solid, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Solid, 	FieldBlocks.Brick, 	FieldBlocks.Brick, //
	            FieldBlocks.Brick, 	FieldBlocks.Solid, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Brick, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Solid, 	FieldBlocks.Brick, //
	            FieldBlocks.Solid, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Solid, 	FieldBlocks.Brick, 	FieldBlocks.Solid, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Solid, //
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Solid, 	FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Brick, 	FieldBlocks.Solid, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
            };

            fieldData = new FieldData(Width, Height, blocks);
            
            playerLocations = new PlayerLocationInfo[] {
	            new PlayerLocationInfo(0, 1, 0, -1),
	            new PlayerLocationInfo(1, 4, 0, -1),
	            new PlayerLocationInfo(2, 10, 0, -1),
	            new PlayerLocationInfo(3, 13, 0, -1),
	            new PlayerLocationInfo(4, 13, 10, -1),
	            new PlayerLocationInfo(5, 10, 10, -1),
	            new PlayerLocationInfo(6, 4, 10, -1),
	            new PlayerLocationInfo(7, 1, 10, -1),
	            new PlayerLocationInfo(8, 5, 5, -1),
	            new PlayerLocationInfo(9, 9, 5, -1),
            };

            powerupInfo = new PowerupInfo[] {
	            new PowerupInfo(0, false, false, 0, false),
	            new PowerupInfo(1, false, false, 0, false),
	            new PowerupInfo(2, false, false, 0, false),
	            new PowerupInfo(3, false, false, 0, false),
	            new PowerupInfo(4, false, false, 0, false),
	            new PowerupInfo(5, false, false, 0, false),
	            new PowerupInfo(6, false, false, 0, false),
	            new PowerupInfo(7, false, false, 0, false),
	            new PowerupInfo(8, false, false, 0, false),
	            new PowerupInfo(9, false, false, 0, false),
	            new PowerupInfo(10, false, false, 0, false),
	            new PowerupInfo(11, false, false, 0, false),
	            new PowerupInfo(12, false, false, 0, false),
            };
        }
    }

    public class EmptySchemeMock : Scheme
    {
        const int Width = 15;
        const int Height = 11;

        public EmptySchemeMock(String name, int brickDensity)
        {
            this.name = name;
            this.brickDensity = brickDensity;

            FieldBlocks[] blocks = {
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
	            FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, 	FieldBlocks.Blank, //
            };

            fieldData = new FieldData(Width, Height, blocks);

            playerLocations = new PlayerLocationInfo[] {
	            new PlayerLocationInfo(0, 1, 0, -1),
	            new PlayerLocationInfo(1, 4, 0, -1),
	            new PlayerLocationInfo(2, 10, 0, -1),
	            new PlayerLocationInfo(3, 13, 0, -1),
	            new PlayerLocationInfo(4, 13, 10, -1),
	            new PlayerLocationInfo(5, 10, 10, -1),
	            new PlayerLocationInfo(6, 4, 10, -1),
	            new PlayerLocationInfo(7, 1, 10, -1),
	            new PlayerLocationInfo(8, 5, 5, -1),
	            new PlayerLocationInfo(9, 9, 5, -1),
            };

            powerupInfo = new PowerupInfo[] {
	            new PowerupInfo(0, false, false, 0, false),
	            new PowerupInfo(1, false, false, 0, false),
	            new PowerupInfo(2, false, false, 0, false),
	            new PowerupInfo(3, false, false, 0, false),
	            new PowerupInfo(4, false, false, 0, false),
	            new PowerupInfo(5, false, false, 0, false),
	            new PowerupInfo(6, false, false, 0, false),
	            new PowerupInfo(7, false, false, 0, false),
	            new PowerupInfo(8, false, false, 0, false),
	            new PowerupInfo(9, false, false, 0, false),
	            new PowerupInfo(10, false, false, 0, false),
	            new PowerupInfo(11, false, false, 0, false),
	            new PowerupInfo(12, false, false, 0, false),
            };
        }
    }
}
