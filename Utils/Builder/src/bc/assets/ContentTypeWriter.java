package bc.assets;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;

public abstract class ContentTypeWriter<T>
{
	public void write(File file, T t) throws IOException
	{
		ContentWriter output = null;
		try
		{
			output = new ContentWriter(new FileOutputStream(file));
			write(output, t);
		}
		finally
		{
			if (output != null) output.close();
		}
	}

	protected abstract void write(ContentWriter output, T t);
}
