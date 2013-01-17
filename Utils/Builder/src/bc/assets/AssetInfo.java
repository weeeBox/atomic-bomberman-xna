package bc.assets;

public class AssetInfo
{
	private String importer;
	private String processor;
	private String type;
	private String typePrefix;

	public AssetInfo(String type, String typePrefix, String importer)
	{
		this(type, typePrefix, importer, null);
	}
	
	public AssetInfo(String type, String typePrefix, String importer, String processor)
	{
		this.type = type;
		this.typePrefix = typePrefix;
		this.importer = importer;
		this.processor = processor;
	}

	public String getImporter()
	{
		return importer;
	}
	
	public String getProcessor()
	{
		return processor;
	}

	public String getType()
	{
		return type;
	}

	public String getTypePrefix()
	{
		return typePrefix;
	}
}
