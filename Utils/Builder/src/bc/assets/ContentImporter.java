package bc.assets;

import java.io.File;
import java.io.IOException;

public abstract class ContentImporter <T extends Asset>
{
	public abstract T importAsset(File file, AssetContext context) throws IOException;
}
