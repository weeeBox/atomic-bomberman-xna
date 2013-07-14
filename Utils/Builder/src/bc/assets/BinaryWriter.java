package bc.assets;

import java.io.IOException;
import java.io.OutputStream;
import java.io.UTFDataFormatException;

public class BinaryWriter
{
	private OutputStream out;
	private byte[] writeBuffer;

	public BinaryWriter(OutputStream out)
	{
		this.out = out;
		writeBuffer = new byte[8];
	}

	/**
	 * Writes a one-byte Boolean value to the current stream, with 0
	 * representing false and 1 representing true.
	 */
	public void write(boolean v) throws IOException
	{
		out.write(v ? 1 : 0);
	}

	/**
	 * Writes an unsigned byte to the current stream and advances the stream
	 * position by one byte.
	 */
	public void write(byte b) throws IOException
	{
		out.write(b);
	}

	/** Writes a byte array to the underlying stream. */
	public void write(byte[] b) throws IOException
	{
		out.write(b);
	}

	/**
	 * Writes a Unicode character to the current stream and advances the current
	 * position of the stream in accordance with the Encoding used and the
	 * specific characters being written to the stream.
	 */
	public void write(char c) throws IOException
	{
		if ((c >= 0x0001) && (c <= 0x007F))
		{
			out.write(c);
		}
		else if (c > 0x07FF)
		{
			writeBuffer[0] = (byte) (0xE0 | ((c >> 12) & 0x0F));
			writeBuffer[1] = (byte) (0x80 | ((c >> 6) & 0x3F));
			writeBuffer[2] = (byte) (0x80 | ((c >> 0) & 0x3F));
			out.write(writeBuffer, 0, 3);
		}
		else
		{
			writeBuffer[0] = (byte) (0xC0 | ((c >> 6) & 0x1F));
			writeBuffer[1] = (byte) (0x80 | ((c >> 0) & 0x3F));
			out.write(writeBuffer, 0, 2);
		}
	}

	/**
	 * Writes a character array to the current stream and advances the current
	 * position of the stream in accordance with the Encoding used and the
	 * specific characters being written to the stream.
	 */
	public void write(char[] buffer) throws IOException
	{
		write(buffer, 0, buffer.length);
	}

	/**
	 * Writes an eight-byte floating-point value to the current stream and
	 * advances the stream position by eight bytes.
	 */
	public void write(double v) throws IOException
	{
		write(Double.doubleToLongBits(v));
	}

	/**
	 * Writes a two-byte signed integer to the current stream and advances the
	 * stream position by two bytes.
	 */
	public void write(short v) throws IOException
	{
		writeBuffer[1] = (byte) (v >>> 8);
		writeBuffer[0] = (byte) (v >>> 0);
		out.write(writeBuffer, 0, 2);
	}

	/**
	 * Writes a four-byte signed integer to the current stream and advances the
	 * stream position by four bytes.
	 */
	public void write(int v) throws IOException
	{
		writeBuffer[3] = (byte) (v >>> 24);
		writeBuffer[2] = (byte) (v >>> 16);
		writeBuffer[1] = (byte) (v >>> 8);
		writeBuffer[0] = (byte) (v >>> 0);
		out.write(writeBuffer, 0, 4);
	}

	/**
	 * Writes an eight-byte signed integer to the current stream and advances
	 * the stream position by eight bytes.
	 */
	public void write(long v) throws IOException
	{
		writeBuffer[7] = (byte) (v >>> 56);
		writeBuffer[6] = (byte) (v >>> 48);
		writeBuffer[5] = (byte) (v >>> 40);
		writeBuffer[4] = (byte) (v >>> 32);
		writeBuffer[3] = (byte) (v >>> 24);
		writeBuffer[2] = (byte) (v >>> 16);
		writeBuffer[1] = (byte) (v >>> 8);
		writeBuffer[0] = (byte) (v >>> 0);
		out.write(writeBuffer, 0, 8);
	}

	/**
	 * Writes a four-byte floating-point value to the current stream and
	 * advances the stream position by four bytes.
	 */
	public void write(float v) throws IOException
	{
		write(Float.floatToIntBits(v));
	}

	/**
	 * Writes a length-prefixed string to this stream in the UTF-8 encoding, and
	 * advances the current position of the stream in accordance with the
	 * encoding used and the specific characters being written to the stream.
	 */
	public void write(String str) throws IOException
	{
		int strlen = str.length();
		int utflen = 0;
		int c, count = 0;

		/* use charAt instead of copying String to char array */
		for (int i = 0; i < strlen; i++)
		{
			c = str.charAt(i);
			if ((c >= 0x0001) && (c <= 0x007F))
			{
				utflen++;
			}
			else if (c > 0x07FF)
			{
				utflen += 3;
			}
			else
			{
				utflen += 2;
			}
		}

		if (utflen > 65535) throw new UTFDataFormatException("encoded string too long: " + utflen + " bytes");

		write7BitEncodedInt(utflen);

		byte[] bytearr = null;
		bytearr = new byte[utflen];

		int i = 0;
		for (i = 0; i < strlen; i++)
		{
			c = str.charAt(i);
			if (!((c >= 0x0001) && (c <= 0x007F))) break;
			bytearr[count++] = (byte) c;
		}

		for (; i < strlen; i++)
		{
			c = str.charAt(i);
			if ((c >= 0x0001) && (c <= 0x007F))
			{
				bytearr[count++] = (byte) c;

			}
			else if (c > 0x07FF)
			{
				bytearr[count++] = (byte) (0xE0 | ((c >> 12) & 0x0F));
				bytearr[count++] = (byte) (0x80 | ((c >> 6) & 0x3F));
				bytearr[count++] = (byte) (0x80 | ((c >> 0) & 0x3F));
			}
			else
			{
				bytearr[count++] = (byte) (0xC0 | ((c >> 6) & 0x1F));
				bytearr[count++] = (byte) (0x80 | ((c >> 0) & 0x3F));
			}
		}
		out.write(bytearr, 0, utflen);
	}

	/** Writes a region of a byte array to the current stream. */
	public void write(byte[] b, int off, int len) throws IOException
	{
		out.write(b, off, len);
	}

	/**
	 * Writes a section of a character array to the current stream, and advances
	 * the current position of the stream in accordance with the Encoding used
	 * and perhaps the specific characters being written to the stream.
	 */
	public void write(char[] b, int off, int len) throws IOException
	{
		int strlen = off + len;
		int utflen = 0;
		int c, count = 0;

		/* use charAt instead of copying String to char array */
		for (int i = off; i < strlen; i++)
		{
			c = b[i];
			if ((c >= 0x0001) && (c <= 0x007F))
			{
				utflen++;
			}
			else if (c > 0x07FF)
			{
				utflen += 3;
			}
			else
			{
				utflen += 2;
			}
		}

		if (utflen > 65535) throw new UTFDataFormatException("encoded string too long: " + utflen + " bytes");

		byte[] bytearr = null;
		bytearr = new byte[utflen];

		int i = off;
		for (; i < strlen; i++)
		{
			c = b[i];
			if (!((c >= 0x0001) && (c <= 0x007F))) break;
			bytearr[count++] = (byte) c;
		}

		for (; i < strlen; i++)
		{
			c = b[i];
			if ((c >= 0x0001) && (c <= 0x007F))
			{
				bytearr[count++] = (byte) c;

			}
			else if (c > 0x07FF)
			{
				bytearr[count++] = (byte) (0xE0 | ((c >> 12) & 0x0F));
				bytearr[count++] = (byte) (0x80 | ((c >> 6) & 0x3F));
				bytearr[count++] = (byte) (0x80 | ((c >> 0) & 0x3F));
			}
			else
			{
				bytearr[count++] = (byte) (0xC0 | ((c >> 6) & 0x1F));
				bytearr[count++] = (byte) (0x80 | ((c >> 0) & 0x3F));
			}
		}
		out.write(bytearr, 0, utflen);
	}

	/**
	 * Writes a 32-bit integer in a compressed format. The integer of the value
	 * parameter is written out seven bits at a time, starting with the seven
	 * least-significant bits. The high bit of a byte indicates whether there
	 * are more bytes to be written after this one. If value will fit in seven
	 * bits, it takes only one byte of space. If value will not fit in seven
	 * bits, the high bit is set on the first byte and written out. value is
	 * then shifted by seven bits and the next byte is written. This process is
	 * repeated until the entire integer has been written.
	 */
	public void write7BitEncodedInt(int v) throws IOException
	{
		if (v <= 0x007f)
		{
			out.write(v);
		}
		else if (v <= 0x3fff)
		{
			writeBuffer[0] = (byte) (0x80 | (v & 0x7f));
			writeBuffer[1] = (byte) ((v >> 7) & 0x7f);
			out.write(writeBuffer, 0, 2);
		}
		else
		{
			writeBuffer[0] = (byte) (0x80 | (v & 0x7f));
			writeBuffer[1] = (byte) (0x80 | ((v >> 7) & 0x7f));
			writeBuffer[2] = (byte) ((v >> 14) & 0x7f);
			out.write(writeBuffer, 0, 3);
		}
	}
	
	public OutputStream getStream()
	{
		return out;
	}

	public void close() throws IOException
	{
		out.close();
	}
}