package bc.assets;

public abstract class ContentProcessor <T extends Asset>
{
	@SuppressWarnings("unchecked")
	public void processAsset(Asset asset, AssetContext context)
	{
		process((T)asset, context);
	}
	
	protected abstract void process(T t, AssetContext context);
}
