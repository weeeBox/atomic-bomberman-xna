package bc.assets;

public class ContentInfo <I extends AssetInfo, T extends Asset>
{
	public ContentImporter<I, T> importer;
	public ContentProcessor<T> processor;
	public ContentWriter<T> writer;
}