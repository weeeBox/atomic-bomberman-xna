package bc.assets;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;

public abstract class ContentStreamReader extends ContentImporter
{
	@Override
	public Asset importContent(File file, AssetContext context) throws IOException
	{
		FileInputStream fis = null;
		try
		{
			return read(fis, context);
		}
		finally
		{
			if (fis != null)
				fis.close();
		}
	}

	protected abstract Asset read(InputStream stream, AssetContext context) throws IOException;

}
