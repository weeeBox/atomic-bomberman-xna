package bc.assets;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;

public abstract class ContentWriter<T>
{
	public void write(File file, T t, ContentWriterContext context) throws IOException
	{
		BinaryWriter output = null;
		try
		{
			output = new BinaryWriter(new FileOutputStream(file));
			write(output, t, context);
		}
		finally
		{
			if (output != null) output.close();
		}
	}

	protected abstract void write(BinaryWriter output, T t, ContentWriterContext context);
}
