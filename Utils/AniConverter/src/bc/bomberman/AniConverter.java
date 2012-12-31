package bc.bomberman;
import java.io.IOException;

import bc.bomberman.utils.FormatException;


public class AniConverter 
{
	public static void main(String[] args) throws IOException, FormatException 
	{
		AniFile file = new AniFile(args[0]);
	}
}
