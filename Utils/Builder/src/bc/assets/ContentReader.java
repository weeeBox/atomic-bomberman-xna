package bc.assets;

import java.io.File;
import java.io.IOException;

public abstract class ContentReader <T>
{
	public abstract T read(File file, ContentReaderContext context) throws IOException;
}
