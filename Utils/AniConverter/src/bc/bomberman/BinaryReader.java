package bc.bomberman;

import java.io.EOFException;
import java.io.IOException;
import java.io.InputStream;
import java.io.UnsupportedEncodingException;

public class BinaryReader
{
	private InputStream stream;
	
	private byte[] buffer;
	
	public BinaryReader(InputStream stream)
	{
		this.stream = stream;
	}

	public int ReadByte() throws IOException
	{
		return stream.read();
	}

	public char ReadChar() throws IOException
	{
		return (char) ReadByte();
	}

	public int ReadUInt16() throws IOException
	{	
		return ReadInt16() & 0xffff;
	}

	public int readUInt32() throws IOException
	{	
		int ch1 = stream.read();
        int ch2 = stream.read();
        int ch3 = stream.read();
        int ch4 = stream.read();
        
        if ((ch1 | ch2 | ch3 | ch4) < 0)
            throw new EOFException();
        return ((ch4 << 24) + (ch3 << 16) + (ch2 << 8) + (ch1 << 0));
	}

	public String ReadChars(int count) throws IOException
	{
		byte[] buffer = readBuffer(count);
		try
		{
			return new String(buffer, "ascii");
		}
		catch (UnsupportedEncodingException e)
		{
			throw new IOException(e);
		}
	}

	public int ReadInt16() throws IOException
	{
		int ch1 = stream.read();
        int ch2 = stream.read();
        
        if ((ch1 | ch2) < 0)
            throw new EOFException();
        
        return (ch2 << 8) + (ch1 << 0);
	}
	
	private byte[] readBuffer(int size) throws IOException
	{
		byte[] buffer = getBuffer(size);
		stream.read(buffer, 0, size);
		return buffer;
	}
	
	private byte[] getBuffer(int size)
	{
		if (buffer == null || buffer.length < size)
		{
			buffer = new byte[size];
		}
		
		return buffer;
	}
}
