package bc.bomberman;
import java.io.IOException;
import java.io.InputStream;

import bc.bomberman.utils.FormatException;


public class BitmapBuilder
{
	/// <summary>The size of a bitmap file header</summary>
	public static final int BitmapHeaderSize = 54;
	/// <summary>The size of the bitmap's palette</summary>
	public int PaletteSize;
	/// <summary>
	/// The index into the BitmapData array at which the palette starts,
	/// if present
	/// </summary>
	public int PaletteLocation;
	/// <summary>
	/// The index into the BitmapData array at which the image data starts
	/// </summary>
	public int DataLocation;

	int Width;
	int Height;
	/// <summary>Width of the bitmap</summary>
//	public int Width { get { return m_Width; } }
//	/// <summary>Height of the bitmap</summary>
//	public int Height { get { return m_Height; } }
	/// <summary></summary>
	public int BitsPerPixel;
	private int BytesPerLine;
	/// <summary></summary>
//	public int BytesPerLine { get { return m_BytesPerLine; } }
	private int PaddingPerLine;
	/// <summary></summary>
//	public int PaddingPerLine { get { return m_PaddingPerLine; } }

	private byte[] BitmapData;
	/// <summary>The raw bitmap data</summary>
//	public byte[] BitmapData { get { return m_BitmapData; } }

	/// <summary>
	/// Copy constructor. Performs a shallow copy, and creates a new,
	/// blank array of previous size in BitmapData
	/// </summary>
	/// <param name="old"></param>
	public BitmapBuilder(BitmapBuilder old)
	{
		PaletteSize = old.PaletteSize;
		PaletteLocation = old.PaletteLocation;
		DataLocation = old.DataLocation;
		Width = old.Width;
		Height = old.Height;
		BitsPerPixel = old.BitsPerPixel;
		BytesPerLine = old.BytesPerLine;
		PaddingPerLine = old.PaddingPerLine;
		BitmapData = new byte[old.BitmapData.length];
	}
	
	/// <summary>
	/// Constructor that creates the byte array required to hold a bitmap file
	/// with the specified properties and initializes the bitmap header
	/// information
	/// </summary>
	/// <param name="bitsPerPixel">
	/// Number of bits per pixel in the bitmap.
	/// </param>
	/// <param name="width">The width of the bitmap image</param>
	/// <param name="height">The height of the bitmap image</param>
	/// <remarks>
	/// Bitmap images support <paramref name="bitsPerPixel" /> values of
	/// 1, 4, 8, 16, 24 or 32.
	/// Images with less than 16 bits per pixel will need a palette of 2^bpp
	/// 32-bit entries, which is allocated by the constructor.
	/// </remarks>
	public BitmapBuilder(int bitsPerPixel, int width, int height)
	{
		// calculate some values
		BitsPerPixel = bitsPerPixel;
		Width = width;
		Height = height;
		
		int bmpBytesPerLine = (int)Math.ceil(Width * BitsPerPixel / 8.0);
		
		// bitmap data lines are always a multiple of four bytes
		PaddingPerLine = (4 - bmpBytesPerLine % 4) % 4;

		BytesPerLine = bmpBytesPerLine + PaddingPerLine;
		
		
		if (BitsPerPixel >= 16)
			PaletteSize = 0;
		else
			PaletteSize = 4 * (1 << BitsPerPixel); // palette is 4 * 2^bpp bytes long
		
		// Create the buffer for bitmap data
		int bmpSize = (int)(BitmapHeaderSize + PaletteSize + BytesPerLine * Height);
		byte[] bmpData = new byte[bmpSize];
		
		// Fill bitmap header
		int i = 0;
		bmpData[i++] = (byte)'B';
		bmpData[i++] = (byte)'M';
		bmpData[i++] = (byte)(bmpSize & 0xFF);
		bmpData[i++] = (byte)((bmpSize >> 8) & 0xFF);
		bmpData[i++] = (byte)((bmpSize >> 16) & 0xFF);
		bmpData[i++] = (byte)((bmpSize >> 24) & 0xFF);
		i = 10;
		bmpData[i++] = (byte)((BitmapHeaderSize + PaletteSize) & 0xFF);
		bmpData[i++] = (byte)(((BitmapHeaderSize + PaletteSize) >> 8) & 0xFF);
		bmpData[i++] = (byte)(((BitmapHeaderSize + PaletteSize) >> 16) & 0xFF);
		bmpData[i++] = (byte)(((BitmapHeaderSize + PaletteSize) >> 24) & 0xFF);
		bmpData[i++] = (byte)40;
		i = 18;
		bmpData[i++] = (byte)(Width & 0xFF);
		bmpData[i++] = (byte)((Width >> 8) & 0xFF);
		bmpData[i++] = (byte)((Width >> 16) & 0xFF);
		bmpData[i++] = (byte)((Width >> 24) & 0xFF);
		bmpData[i++] = (byte)(Height & 0xFF);
		bmpData[i++] = (byte)((Height >> 8) & 0xFF);
		bmpData[i++] = (byte)((Height >> 16) & 0xFF);
		bmpData[i++] = (byte)((Height >> 24) & 0xFF);
		bmpData[i++] = (byte)1; ++i; // always one plane in a bitmap
		bmpData[i++] = (byte)(BitsPerPixel & 0xFF);
		bmpData[i++] = (byte)((BitsPerPixel >> 8) & 0xFF);

		BitmapData = bmpData;
		
		PaletteLocation = BitmapHeaderSize;
		DataLocation = PaletteLocation + PaletteSize;
	}

	/// <summary>
	/// Creates a new <see cref="Stream" /> backed by the BuildmapBuilder's
	/// bitmap data
	/// </summary>
	/// <returns>a Stream that will provide bitmap data</returns>
	/// <remarks>
	/// This method's return value is primarily intended for use
	/// with <see cref="System.Drawing.Bitmap(System.IO.Stream)" /> or similar (for
	/// example Texture creation) functions
	/// </remarks>
//	public Stream GetStream()
//	{
//		if (m_Stream == null)
//			m_Stream = new MemoryStream(BitmapData);
//
//		return m_Stream;
//	}
	
	/// <summary>
	/// Reads a bitmap-format palette from the specified <see cref="Stream" />
	/// and inserts it into the bitmap data
	/// </summary>
	/// <param name="s">the Stream to read the palette from</param>
	/// <remarks>
	/// This will read a constant amount of <see cref="PaletteSize" /> bytes.
	/// The caller needs to make sure that this amount of data is available.
	/// </remarks>
	public void ReadPaletteFromBitmap(InputStream s) throws IOException
	{
		// a bitmap palette can simply be copied
		s.read(BitmapData, (int)PaletteLocation, (int)PaletteSize);
	}
	
	/// <summary>
	/// Reads a PCX-format palette from a Stream with the help of the
	/// specified <see cref="BinaryReader" /> and inserts it into the bitmap
	/// data
	/// </summary>
	/// <param name="r">the BinaryReader to read the palette with</param>
	/// <remarks>
	/// TRYTRY: this should read PaletteSize bytes
	/// </remarks>
	public void ReadPaletteFromPCX(BinaryReader r)
	{
		for (int i = PaletteLocation; i < DataLocation; ++i)
		{
			int R = r.ReadByte(),
				G = r.ReadByte(),
				B = r.ReadByte();

			BitmapData[i++] = (byte)B;
			BitmapData[i++] = (byte)G;
			BitmapData[i++] = (byte)R;
			// skip one more byte, as bmp uses 32 bit palette entries
		}
	}
	
	/// <summary>
	/// Reads the actual image data from a Stream with the help of the
	/// specified <see cref="BinaryReader" /> and insert it into the bitmap
	/// data
	/// </summary>
	/// <param name="s">the Stream to read the data from</param>
	/// <param name="maxLength">
	/// the maximum number of bytes to be read from the stream. Pass
	/// <see cref="UInt64.MaxValue" /> if there is no limit
	/// </param>
	/// <returns>
	/// A positive number of bytes read from the stream if successful, the
	/// negative of the number of bytes read on failure, zero if no bytes have
	/// been read.
	/// </returns>
	/// <remarks>
	/// The data will be read using the Run-length-encoding format used in
	/// Atomic bomberman ANI files, which is (exactly?) the same as that used
	/// in TGA files.
	/// </remarks>
	public long ReadDataFromAni(InputStream s, long maxLength) throws IOException
	{
		long bytesRead = 0;
		
		// this is one RLE-unit (usually a pixel, two for 4 bpp)
		int bytesPerUnit = (int)Math.ceil(BitsPerPixel / 8.0);
		
		// is RLE active (or are we in a raw block)?
		boolean rle = false;
		// the data unit that is repeated in an RLE block
		byte[] data = new byte[bytesPerUnit];
		// and the number of times it is to be repeated
		// (or the number of raw bytes)
		int count = 0;
		
		// our array index. Will jump around a bit as bitmaps are bottom-up
		int i;
		
		// we will find the pixels ordered correctly, so we loop through
		// them, and x and y provide the current pixel's location
		for (int y = 0; y < Height; ++y)
		{
			// calculate the position of the current line
			// invert the y coordinate, then find the start of that line
			i = (int)(DataLocation + (Height - y - 1) * BytesPerLine);
			//Console.WriteLine("Reading line {0}, read {1} bytes so far", y, bytesRead);
			for (int x = 0; x < Width; ++x)
			{
				// if count is zero, a new block starts, either raw or RLE
				if (count == 0)
				{
					// check whether we can still read a byte
					if ((long)bytesRead + 1 > maxLength)
						return -bytesRead;
					
					// this is the status/count byte
					count = s.read();
					++bytesRead;
					
					// if bit 7 is set, this denotes the start of an RLE block
					if ((count & 0x80) != 0)
					{
						// unset bit 7 to get the repeat count minus one
						// the one unit will be added right below in this loop
						count &= ~0x80;
						rle = true;
						
						// check whether we can still read a unit
						if ((long)bytesRead + bytesPerUnit > maxLength)
							return -bytesRead;
						
						s.read(data, 0, (int)bytesPerUnit);
						
						bytesRead += bytesPerUnit;
					}
					// if bit 7 is unset, count is the number of raw units to
					// copy minus one. The one unit will be copied right below
					// in this loop
					else
					{
						rle = false;
						
						// return failure if we cannot read so many units
						if ((long)bytesRead + count * bytesPerUnit > maxLength)
							return -bytesRead;
					}
				}
				// we are in the middle of a block. We decrease the count and
				// then copy the apropriate item below
				else
					--count;
				
				// in RLE mode we just add the saved data unit once
				if (rle)
				{
					System.arraycopy(data, 0, BitmapData, (int)i, (int)bytesPerUnit);
					i += bytesPerUnit;
				}
				// in raw mode, we copy one unit
				else
				{
					s.read(BitmapData, (int)i, (int)bytesPerUnit);
					i += bytesPerUnit;
					bytesRead += bytesPerUnit;
				}
			}
		}
		
		// HACKHACK: debug stuff. This should probably be returned somehow
		if (count > 0)
		{
//			Console.WriteLine("{0} bytes of extra data encoded in image", count);
		}
		
		// success. we return the new position
		return bytesRead;
	}

	private static long texturesCropped = 0;
	private static long m_TexturesCropped = 0;
	private static long m_RTexturesCropped = 0;
	/// <summary>Debug/Optimization/Test Value</summary>
	/// <summary>Debug/Optimization/Test Value</summary>
	private static long columnsCropped = 0;
	private static long m_ColumnsCropped = 0;
	private static long m_RColumnsCropped = 0;
	/// <summary>Debug/Optimization/Test Value</summary>
	/// <summary>Debug/Optimization/Test Value</summary>
	private static long rowsCropped = 0;
	private static long m_RowsCropped = 0;
	private static long m_RRowsCropped = 0;
	/// <summary>Debug/Optimization/Test Value</summary>
	/// <summary>Debug/Optimization/Test Value</summary>
	private static long bytesSavedByCropping = 0;
	private static long m_BytesSavedByCropping = 0;
	private static long m_RBytesSavedByCropping = 0;
	/// <summary>Debug/Optimization/Test Value</summary>
	/// <summary>Debug/Optimization/Test Value</summary>

	/// <summary>
	/// 
	/// </summary>
	/// <param name="remapped"></param>
	public static void SeqCropDone(boolean remapped)
	{
		if (remapped)
		{
			m_RTexturesCropped += texturesCropped; texturesCropped = 0;
			m_RRowsCropped += rowsCropped; rowsCropped = 0;
			m_RColumnsCropped += columnsCropped; columnsCropped = 0;
			m_RBytesSavedByCropping += bytesSavedByCropping; bytesSavedByCropping = 0;
		}
		else
		{
			m_TexturesCropped += texturesCropped; texturesCropped = 0;
			m_RowsCropped += rowsCropped; rowsCropped = 0;
			m_ColumnsCropped += columnsCropped; columnsCropped = 0;
			m_BytesSavedByCropping += bytesSavedByCropping; bytesSavedByCropping = 0;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public static long MissingTextures()
	{
		return texturesCropped;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="keyColor"></param>
	/// <param name="rawKeyColor"></param>
	/// <param name="dLeft"></param>
	/// <param name="dTop"></param>
	/// <returns></returns>
	public CropResult Crop(int keyColor, int rawKeyColor) throws FormatException
	{
		BitmapBuilder newBB;
		int dLeft, dTop;
		
		switch (BitsPerPixel)
		{
			case 16:
				//
				// bottom cropping
				//
				int cropBottom = 0;
				boolean done = false;
				
				for (long y = 0; y < Height; ++y)
				{
					int i = (int) (DataLocation + BytesPerLine * y);
					for (long x = 0; x < Width; ++x)
					{
						short color = (short)(BitmapData[i] + (BitmapData[i + 1] << 8));

						if (color != rawKeyColor)
						{
							done = true;
							break;
						}
						i += 2;
					}
					if (done)
						break;
					++cropBottom;
				}
				
				// is the image all transparent?
				if (cropBottom == Height)
				{
					// leave a 1x1 transparent image
					newBB = new BitmapBuilder(BitsPerPixel, 1, 1);
					
					newBB.BitmapData[BitmapHeaderSize] = (byte)(rawKeyColor & 0xFF);
					newBB.BitmapData[BitmapHeaderSize] = (byte)((rawKeyColor >> 8) & 0xFF);
					rowsCropped += Height - 1;
					columnsCropped += Width - 1;
					dLeft = 0;
					dTop = 0;
					// done. exit
					break;
				}

				//
				// top cropping
				//
				int cropTop = 0;
				done = false;
				
				for (long y = Height - 1; y >= 0; --y)
				{
					int i = (int) (DataLocation + BytesPerLine * y);
					for (long x = 0; x < Width; ++x)
					{
						short color = (short)(BitmapData[i] + (BitmapData[i + 1] << 8));
						
						if (color != rawKeyColor)
						{
							done = true;
							break;
						}
						i += 2;
					}
					if (done)
						break;
					++cropTop;
				}
				
				//
				// left cropping
				//
				int cropLeft = 0;
				done = false;

				for (long x = 0; x < Width; ++x)
				{
					for (long y = cropBottom; y < Height - cropTop; ++y)
					{
						int i = (int) (DataLocation + BytesPerLine * y + 2 * x);
						short color = (short)(BitmapData[i] + (BitmapData[i + 1] << 8));

						if (color != rawKeyColor)
						{
							done = true;
							break;
						}
					}
					if (done)
						break;
					++cropLeft;
				}
				
				//
				// right cropping
				//
				int cropRight = 0;
				done = false;
				
				for (long x = Width - 1; x >= 0; --x)
				{
					for (long y = cropBottom; y < Height - cropTop; ++y)
					{
						int i = (int) (DataLocation + BytesPerLine * y + 2 * x);
						short color = (short)(BitmapData[i] + (BitmapData[i + 1] << 8));
						
						if (color != rawKeyColor)
						{
							done = true;
							break;
						}
					}
					if (done)
						break;
					++cropRight;
				}
				
				//
				// do cropping
				//
				
				// do we have anything to crop?
				if (cropTop + cropBottom + cropLeft + cropRight > 0)
				{
					int newWidth = (int)(Width - cropLeft - cropRight);
					int newHeight = (int)(Height - cropTop - cropBottom);
					
					newBB = new BitmapBuilder(BitsPerPixel, newWidth, newHeight);
					
					// bytes per line (to copy). without padding.
					int newBytesPerLine = newBB.BytesPerLine - newBB.PaddingPerLine;
					int newBytesPerLineWithPadding = newBB.BytesPerLine;
					
					for (int y = 0; y < newHeight; ++y)
						System.arraycopy(BitmapData, (int)(BitmapHeaderSize + (y + cropBottom) * BytesPerLine + 2 * cropLeft),
										newBB.BitmapData, (int)(BitmapHeaderSize + y * newBytesPerLineWithPadding),
										(int)newBytesPerLine);
				}
				else
					newBB = this;
				
				rowsCropped += cropTop + cropBottom;
				columnsCropped += cropLeft + cropRight;
				
				dLeft = cropLeft;
				dTop = cropTop;
				
				break;
			// TODO: image formats
			// we can and need currently only crop 16 bpp images
			// This might change when using custom animations
			case 4:
			case 8:
			case 24:
			default:
				throw new FormatException("Cannot remap unexpected"
											+ " bitmap format "
											+ BitsPerPixel + " bpp");
		}

		bytesSavedByCropping += BitmapData.length - newBB.BitmapData.length;
		++texturesCropped;
		
		return new CropResult(newBB, dLeft, dTop);
	}
	
	public static class CropResult
	{
		private BitmapBuilder builder;
		private int left;
		private int top;

		public CropResult(BitmapBuilder builder, int left, int top) {
			this.builder = builder;
			this.left = left;
			this.top = top;
		}
		
		public BitmapBuilder getBuilder() {
			return builder;
		}
		
		public int getTop() {
			return top;
		}
		
		public int getLeft() {
			return left;
		}
	}
}

