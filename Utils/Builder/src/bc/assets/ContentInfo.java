package bc.assets;

public class ContentInfo <T extends Asset>
{
	public ContentImporter<T> importer;
	public ContentProcessor<T> processor;
	public ContentWriter<T> writer;
}