package bc.assets;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;

public abstract class ContentWriter<T extends Asset>
{
	public void write(File file, Asset asset, AssetContext context) throws IOException
	{
		BinaryWriter output = null;
		try
		{
			output = new BinaryWriter(new FileOutputStream(file));
			writeAsset(output, asset, context);
		}
		finally
		{
			if (output != null) output.close();
		}
	}

	@SuppressWarnings("unchecked")
	private void writeAsset(BinaryWriter output, Asset asset, AssetContext context)
	{
		write(output, (T)asset, context);
	}
	
	protected abstract void write(BinaryWriter output, T asset, AssetContext context);
}
