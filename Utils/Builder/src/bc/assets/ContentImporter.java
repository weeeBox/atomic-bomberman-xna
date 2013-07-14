package bc.assets;

import java.io.IOException;

public abstract class ContentImporter <I extends AssetInfo, T extends Asset>
{
	@SuppressWarnings("unchecked")
	public Asset importContent(AssetInfo info, AssetContext context) throws IOException
	{
		return importAsset((I)info, context);
	}
	
	public abstract T importAsset(I info, AssetContext context) throws IOException;
}
