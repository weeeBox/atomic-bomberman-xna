package bc.assets;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;

public abstract class ContentStreamReader <T> extends ContentImporter<T>
{
	@Override
	public T read(File file, ContentReaderContext context) throws IOException
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

	protected abstract T read(InputStream stream, ContentReaderContext context) throws IOException;

}
