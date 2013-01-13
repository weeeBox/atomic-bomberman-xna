package bc.bomberman;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.List;

import bc.bomberman.utils.FormatException;
import bc.bomberman.utils.NotImplementedException;
import bc.bomberman.utils.Size;

public class AniFile
	{
		/// <summary>A list of the image frames in the animation</summary>
		public List<AnimationFrame> Frames = new ArrayList<AnimationFrame>();
		/// <summary>A list of the animation sequences in the file</summary>
		public List<Animation> Sequences = new ArrayList<Animation>();
		private boolean extraData;
		private int frame;
		private short frameX;
		private short frameY;

		/// <summary>
		/// Parses the specified AB animation file
		/// </summary>
		/// <param name="filename">Path to the .ani file</param>
		public AniFile(String filename) throws IOException, FormatException {
			this(filename, false);
		}

		/// <summary>
		/// Parses the specified AB animation file
		/// </summary>
		/// <param name="filename">Path to the .ani file</param>
		/// <param name="extraData">
		/// A boolean value specifying whether to output extra (debug)
		/// information to the console
		/// </param>
		public AniFile(String filename, boolean extraData) throws IOException, FormatException
		{
			this.extraData = extraData;
			InputStream s = null;
			try
			{
				s = new FileInputStream(new File(filename));
				create(s);
			}
			finally
			{
				if (s != null) {
					s.close();
				}
			}
		}

		/// <summary>
		/// Parses an animation file read from the specified stream
		/// </summary>
		/// <param name="s">The stream to read ani data from</param>
		public AniFile(InputStream s) throws FormatException, IOException {
				this(s, false);
			}

		/// <summary>
		/// Parses an animation file read from the specified stream
		/// </summary>
		/// <param name="s">stream to load the animation from</param>
		/// <param name="extraData">
		/// pass true to output debugging information to the console during file
		/// parsing
		/// </param>
		public AniFile(InputStream s, boolean extraData) throws FormatException, IOException
		{
			this.extraData = extraData;
			create(s);
		}

		/// <summary>
		/// Decodes an ani file into an Animation object
		/// </summary>
		/// <param name="s">source stream that delivers the animation file</param>
		private void create(InputStream s) throws FormatException, IOException
		{
			BinaryReader r = new BinaryReader(s);

			//
			// Read the ANI header
			//

			String fileSignature = r.ReadChars(10);
			if (!fileSignature.equals("CHFILEANI "))
				throw new FormatException("Invalid ANI file signature: " + fileSignature);

			int fileLength = r.readUInt32();

			int fileId = r.ReadUInt16();

			if (fileId != 0)
				OutputInfo("FileId: " + fileId);

			int nHeads = 0,
				nPals = 0,
				nTPals = 0,
				nCBoxes = 0;

			long bytesRead = 0;

			//
			// go through all items in the file
			//
			while (bytesRead < fileLength)
			{
				if (bytesRead + 10 > fileLength)
					throw new FormatException("Item header too short (" + (fileLength - bytesRead) + ")");

				String itemSignature = r.ReadChars(4);
				int itemLength = r.readUInt32();
				int itemId = r.ReadUInt16();
				long itemStart = (bytesRead += 10);
				long itemEnd = itemStart + itemLength;
				long a; // random var for temporary readings

				if (itemEnd > fileLength)
					throw new FormatException("Item " + itemSignature + " (" + itemId + ") of length " + itemLength
								+ " does not fit in file with " + (fileLength - bytesRead) + " bytes left.");

				// each item type is responsible for reading exactly itemLength
				// bytes and incrementing bytesRead by itemLength
				switch (itemSignature)
				{
					case "HEAD":
						{
							++nHeads;
							if (nHeads > 1)
								OutputInfo("Multiple heads detected o_O");

							if (itemId != 1)
								OutputInfo("Head id: " + itemId);

							if (itemLength != 48)
								OutputInfo("Head length: " + itemLength);

							if ((a = r.readUInt32()) != 0x00010000 && a != 0x00010001)
								OutputInfo("1st dword in head is {0,8:X}", a);

							if ((a = r.readUInt32()) != 0x00010064 && a != 0x00000064 && a != 0x0001001E)
								OutputInfo("2nd dword in head is {0,8:X}", a);

							if ((a = r.readUInt32()) != 0x00000001)
								OutputInfo("3nd dword in head is {0,8:X}", a);

							for (bytesRead += 12; bytesRead < itemEnd; ++bytesRead)
								if ((a = r.ReadByte()) != 0)
									OutputInfo("Extra data in head: " + a);

							break;
						}
					case "FRAM":
						bytesRead = ParseFrame(s, r, bytesRead, itemId, itemLength, itemStart, itemEnd);

						break;
					case "SEQ ":
						bytesRead = ParseSequence(s, r, bytesRead, itemId, itemLength, itemStart, itemEnd);

						break;
					case "PAL ":
						{
							++nPals;
							if (nPals > 1)
								OutputInfo("Multiple Pals detected");

							if (itemId != 1)
								OutputInfo("Pal id: " + itemId);

							if (itemLength != 8192)
								OutputInfo("Pal length: " + itemLength);

							s.skip(itemLength);
							bytesRead += itemLength;
							break;
						}
					case "TPAL":
						{
							++nTPals;
							if (nTPals > 1)
								OutputInfo("Multiple T-Pals detected");

							if (itemId != 1)
								OutputInfo("TPal id: " + itemId);

							if (itemLength != 1028)
								OutputInfo("TPal length: " + itemLength);

							s.skip(itemLength);
							bytesRead += itemLength;
							break;
						}
					case "CBOX":
						{
							++nCBoxes;
							if (nCBoxes > 1)
								OutputInfo("Multiple CBoxes detected");

							if (itemId != 1)
								OutputInfo("CBox id: " + itemId);

							if (itemLength != 4)
								OutputInfo("CBox length: " + itemLength);

							if (itemLength < 4)
								throw new FormatException("CBox item too short (" + (fileLength - bytesRead) + ")");

							if ((a = r.ReadUInt16()) != 16)
								OutputInfo("CBox X: " + a);

							if ((a = r.ReadUInt16()) != 16)
								OutputInfo("CBox Y: " + a);

							s.skip(itemLength - 4);

							bytesRead += itemLength;
							break;
						}
					default:
						OutputInfo("Unknown item " + itemSignature);
						s.skip(itemLength);
						bytesRead += itemLength;
						break;
				}

				assert bytesRead == itemEnd : "Ani File Item Handler bug ," + 
										"Wrong number of bytes read for item " + itemSignature;
			}

			if (s.available() > 0)				
				OutputInfo("File is longer than fileLength specifies. " + s.available() + " bytes of additional data found.");
			
			if (extraData)
			{
				// System.out.printf("-- Summary --");
				// System.out.printf("{0} Head(s), {1} Palettes, {2} T-Palettes, {3} CBoxes found",
				//					nHeads, nPals, nTPals, nCBoxes);
				// System.out.printf("{0} Frame(s) found", Frames.size());
				for (int i = 0; i < Frames.size(); ++i)
				{
					// System.out.printf("\tFrame{0}:", i);
					// System.out.printf("\t\tFilename: " + Frames[i].FileName);
					// System.out.printf("\t\tDimensions: {0}x{1}", Frames[i].BitmapBuilder.Width, Frames[i].BitmapBuilder.Height);
					// System.out.printf("\t\tHot spot: ({0}, {1})", Frames[i].Offset.Width, Frames[i].Offset.Height);
					// System.out.printf("\t\tKey Color: {0} (raw: 0x{1:X})", Frames[i].KeyColor, Frames[i].RawKeyColor);
				}
				// System.out.printf("{0} Sequence(s) found", Sequences.size());
				for (int i = 0; i < Sequences.size(); ++i)
				{
					Animation seq = Sequences.get(i);
					// System.out.printf("\tSequence{0}:", i);
					// System.out.printf("\t\tName: " + seq.Name);
					// System.out.printf("\t\tNumber of states: " + seq.Frames.Length);
					for (int j = 0; j < seq.Frames.length; ++j)
					{
						//AnimationState stat = seq.States[j];

						// System.out.printf("\t\tState{0}:", j);
						// System.out.printf("\t\t\tFrame: {0}", seq.Frames[j].FileName ?? "(None)");
						//// System.out.printf("\t\t\tNew Speed: {0}", (stat.NewSpeed == -1) ? "--" : stat.NewSpeed.ToString());
						//// System.out.printf("\t\t\tFrame offset: ({0}, {1})",
						//						seq.FrameOffset[j].Width,
						//						seq.FrameOffset[j].Height);
						// System.out.printf("\t\t\tRelative frame offset: ({0}, {1})", seq.FrameOffset[j].Width, seq.FrameOffset[j].Height);
					}
				}
			}
		}

		private void OutputInfo(String format, Object... args) {
		}

		/// <summary>
		/// parse an ani file FRAM item
		/// </summary>
		/// <param name="s">the stream to read from. must be skippable</param>
		/// <param name="r">a DataInputStream for s</param>
		/// <param name="bytesRead">current position in the file</param>
		/// <param name="itemId">item id read from item header</param>
		/// <param name="itemLength">item length</param>
		/// <param name="itemStart">item start position in the file</param>
		/// <param name="itemEnd">item end position</param>
		/// <returns>the new position in the file</returns>
		private long ParseFrame(InputStream s, BinaryReader r, long bytesRead, int itemId, int itemLength, long itemStart, long itemEnd) throws FormatException, IOException
		{
			assert itemStart == bytesRead;
			long a; // random var for temporary readings
			int iFrame = Frames.size();
			AnimationFrame frame;
			// HACKHACK: we should first collect all the data, then create a frame out of it
			Frames.add(frame = new AnimationFrame());

			if (itemId != 0)
				OutputInfo("Frame id: " + itemId);

			int nFrameHeads = 0,
				nFrameFNames = 0,
				nFrameCImages = 0;

			while (bytesRead < itemEnd)
			{
				if (bytesRead + 10 > itemEnd)
				{
//					OutputInfo("bytesRead: {0}, stream is at {1}", bytesRead, s.Position);
					throw new FormatException("Frame Item header too short (" + (itemEnd - bytesRead) + ")");
				}

				String frameItemSignature = r.ReadChars(4);
				int frameItemLength = r.readUInt32();
				int frameItemId = r.ReadUInt16();
				long frameItemStart = (bytesRead += 10);
				long frameItemEnd = frameItemStart + frameItemLength;

				if (bytesRead + frameItemLength > itemEnd)
					throw new FormatException("Frame Item " + frameItemSignature
								+ " (" + frameItemId + ") of length " + frameItemLength
								+ " does not fit in frame with " + (itemEnd - bytesRead) + " bytes left");

				switch (frameItemSignature)
				{
					case "HEAD":
						{
							++nFrameHeads;
							if (nFrameHeads > 1)
								OutputInfo("Multiple frame{0} heads detected", iFrame);

							if (frameItemId != 1)
								OutputInfo("Frame{0} head id: " + frameItemId, iFrame);

							if (frameItemLength != 2)
							{
								OutputInfo("Frame{0} head size: " + frameItemLength, iFrame);
								s.skip(frameItemLength);
							}
							else
							{
								if ((a = r.ReadUInt16()) != 0x6403 && a != 0x0F03 && a != 0x1E03)
									OutputInfo("Frame{0} head data: " + a, iFrame);
							}

							bytesRead += frameItemLength;
							break;
						}
					case "FNAM":
						{
							++nFrameFNames;
							if (nFrameFNames > 1)
								OutputInfo("Multiple frame{0} filenames detected, previous was " + frame.getFileName(), iFrame);

							if (frameItemId != 1)
								OutputInfo("Frame{0} filename id: " + frameItemId, iFrame);

							if (frameItemLength == 0)
								throw new FormatException("Zero length frame" + iFrame + " filename");

							frame.setFileName(new String(r.ReadChars((int)frameItemLength - 1)));

							if ((a = r.ReadByte()) != 0)
								OutputInfo("Last frame{0} filename char: " + a, iFrame);

							bytesRead += frameItemLength;
							break;
						}
					case "CIMG":
						++nFrameCImages;
						if (nFrameCImages > 1)
							OutputInfo("Multiple frame{0} images detected", iFrame);

						bytesRead = ParseFrameCImage(s, r, bytesRead, iFrame, frame, frameItemId, frameItemLength, frameItemStart, frameItemEnd);

						break;
					case "ATBL":
						// known but ignored
						s.skip(frameItemLength);
						bytesRead += frameItemLength;
						break;
					default:
						OutputInfo("Unknown frame{0} item " + frameItemSignature, iFrame);
						s.skip(frameItemLength);
						bytesRead += frameItemLength;
						break;
				}
				assert bytesRead == frameItemEnd;
			}

			if (nFrameHeads == 0)
				OutputInfo("Frame{0} has no head", iFrame);
			if (nFrameFNames == 0)
				OutputInfo("Frame{0} has no filename", iFrame);
			if (nFrameCImages == 0)
				OutputInfo("Frame{0} has no c-image", iFrame);

			return bytesRead;
		}


		/// <summary>
		/// parse an ani file "CIMG" item (inside "FRAM")
		/// </summary>
		/// <param name="s">the stream to read from. must be skippable</param>
		/// <param name="r">a DataInputStream for s</param>
		/// <param name="bytesRead">current position in the file</param>
		/// <param name="iFrame">current sequence number for debug output</param>
		/// <param name="frame">current frame</param>
		/// <param name="frameItemId">item id read from item header</param>
		/// <param name="frameItemLength">item length</param>
		/// <param name="frameItemStart">item start position in the file</param>
		/// <param name="frameItemEnd">item end position</param>
		/// <returns>the new position in the file</returns>
		private long ParseFrameCImage(InputStream s, BinaryReader r, long bytesRead, int iFrame, AnimationFrame frame, int frameItemId, int frameItemLength, long frameItemStart, long frameItemEnd) throws FormatException, IOException
		{
			assert frameItemStart == bytesRead;
			long a;
			if (frameItemId != 1)
				OutputInfo("Frame{0} CImage id: " + frameItemId, iFrame);

			if (frameItemLength < 32)
				throw new FormatException("CImage too short (" + (frameItemEnd - bytesRead) + ")");

			int imageType = r.ReadUInt16();

			int imageUnknown1 = r.ReadUInt16();

			if (imageUnknown1 != 4 && imageUnknown1 != 0)
				OutputInfo("CImage{0} header unknown1: " + imageUnknown1, iFrame);

			int imageAdditionalSize = r.readUInt32();

			bytesRead += 8;

			if (imageAdditionalSize < 24)
				throw new FormatException("CImage" + iFrame + " header of size " + imageAdditionalSize + " can't contain any valid information");

			if (imageAdditionalSize > frameItemEnd - bytesRead)
				throw new FormatException("CImage" + iFrame + " too short (" + (frameItemEnd - bytesRead) + ") for header");

			boolean imagePaletteHeader = false;
			int imagePaletteSize = 0;

			if (imageAdditionalSize >= 32)
				imagePaletteHeader = true;

			if (imageAdditionalSize > 32)
				imagePaletteSize = (int)(imageAdditionalSize - 32);

			if (imageAdditionalSize == 32)
				OutputInfo("CImage{0} has a palette header but no palette o_O", iFrame);

			//
			// "following header" (16 bytes)
			//
			int imageUnknown2 = r.readUInt32();

			if (!imagePaletteHeader && imageUnknown2 != 0 || imagePaletteHeader && imageUnknown2 != 24)
				OutputInfo("CImage{0} header unknown2: " + imageUnknown2, iFrame);

			int bitsPerPixel;
			int width = r.ReadUInt16();
			int height = r.ReadUInt16();
			int hotSpotX = r.ReadUInt16();
			int hotSpotY = r.ReadUInt16();
			frame.Offset = new Size(hotSpotX, hotSpotY);
			frame.RawKeyColor = r.ReadUInt16();

			if (frame.RawKeyColor == 0xFFFF)
			{
				frame.KeyColor = 0;
			}
			else
			{
				// no idea what bit 15 means
				if (frame.RawKeyColor >> 15 == 1)
					OutputInfo("CImage{0} key color has bit 15 set, but is not white",
								iFrame);

				int R = (frame.RawKeyColor >> 10) << 3,
					G = ((frame.RawKeyColor >> 5) & 0x1F) << 3,
					B = (frame.RawKeyColor & 0x1F) << 3;

				R += (int)Math.ceil(R * 6.0 / 239.0);
				G += (int)Math.ceil(G * 6.0 / 239.0);
				B += (int)Math.ceil(B * 6.0 / 239.0);

				frame.KeyColor = fromArgb(R, G, B);
			}

			int imageUnknown3 = r.ReadUInt16();

			if (imageUnknown3 != 0)
				OutputInfo("CImage{0} header unknown3: " + imageUnknown3, iFrame);

			bytesRead += 16;

			//
			// calculate image information from header data
			//
			switch (imageType)
			{
				case 0x04:
					bitsPerPixel = 16;

					// for some reason, type 4 images can have a palette
					/*if (imagePaletteSize > 0)
						OutputInfo("CImage{0} is type 4 but has a palette of "
													+ imagePaletteSize + " bytes", iFrame);*/

					break;
				case 0x05:
					bitsPerPixel = 24;
					if (imagePaletteSize > 0)
						throw new FormatException("CImage" + iFrame + " is type 5 but has a palette of "
														+ imagePaletteSize + " bytes");
					break;
				case 0x0A:
					bitsPerPixel = 4;

					if (imagePaletteSize != 64)
						throw new FormatException("CImage" + iFrame + " is type 10 but has a palette of "
														+ imagePaletteSize + " bytes");

					break;
				case 0x0B:
					bitsPerPixel = 8;

					if (imagePaletteSize != 1024)
						throw new FormatException("CImage" + iFrame + " is type 11 but has a palette of "
														+ imagePaletteSize + " bytes");

					break;
				default:
					throw new FormatException("CImage" + iFrame + " has unknown image type");
			}

			//
			// Construct the bitmap
			//
			BitmapBuilder b = new BitmapBuilder(bitsPerPixel, width, height);

			//
			// read optional palette header (8 bytes)
			//
			int imageUnknownP1, imageUnknownP2;

			if (imagePaletteHeader)
			{
				if ((imageUnknownP1 = r.readUInt32()) != 0x1000000 && imageUnknownP1 != 0x100000)
					OutputInfo("CImage{0} header unknownP1: " + imageUnknownP1, iFrame);

				if ((imageUnknownP2 = r.readUInt32()) != 8)
					OutputInfo("CImage{0} header unknownP2: " + imageUnknownP2, iFrame);

				bytesRead += 8;
			}

			//
			// read palette data, which is in bitmap format
			//
			if (imagePaletteSize > 0)
			{
				b.ReadPaletteFromBitmap(s);

				bytesRead += b.PaletteSize;

				if (imagePaletteSize > b.PaletteSize)
				{
					s.skip(imagePaletteSize - b.PaletteSize);
					bytesRead += imagePaletteSize - b.PaletteSize;
				}
			}

			//
			// additional header
			//
			int imageUnknownA1 = r.ReadUInt16();

			if (imageUnknownA1 != 0x10 && imageUnknownA1 != 0x11 && imageUnknownA1 != 0x12)
				OutputInfo("CImage{0} header unknownA1: " + imageUnknownA1, iFrame);

			int imageUnknownA2 = r.ReadUInt16();

			if (imageUnknownA2 != 12)
				OutputInfo("CImage{0} header unknownA2: " + imageUnknownA2, iFrame);

			long imageCompressedSize = r.readUInt32() - 12,
				imageUncompressedSize = r.readUInt32();

			// check if size matches
			int bytesRequired = (int)Math.ceil(bitsPerPixel * width / 8.0) * height;

			if (imageUncompressedSize != 0xFF000000 && imageUncompressedSize != bytesRequired)
				OutputInfo("CImage{0} uncompressed size is {1}, expected {2}", iFrame, imageUncompressedSize, bytesRequired);

			bytesRead += 12;

			//
			// Run-Length-Decode the image data and insert as bitmap data
			// this is tga-type RLE
			//
			long imageDataBytes = b.ReadDataFromAni(s, (long)(frameItemEnd - bytesRead));

			bytesRead += Math.abs(imageDataBytes);

			if (imageDataBytes < 0)
				throw new FormatException("CImage" + iFrame + " data too short. Only " + (frameItemEnd - bytesRead) + " bytes left");


			if (bytesRead + 1 > frameItemEnd)
				OutputInfo("No terminator in image{0}:", iFrame);
			else if (bytesRead + 1 < frameItemEnd)
			{
				OutputInfo("Extra data in image{0}: {1} and {2} more bytes", iFrame, r.ReadByte(), frameItemEnd - bytesRead - 1);
				s.skip(frameItemEnd - bytesRead - 1);
				bytesRead = frameItemEnd;
			}
			else if ((a = r.ReadByte()) != 0xFF)
			{
				OutputInfo("Unusual terminator in image{0}: " + r.ReadByte(), iFrame);
				bytesRead += 2;
			}
			else
				++bytesRead;

			//
			// image creation is done!
			//

			// crop and save
			BitmapBuilder.CropResult crop = b.Crop(frame.KeyColor, frame.RawKeyColor);
			frame.BitmapBuilder = crop.getBuilder();
			frame.Offset.Width -= crop.getLeft();
			frame.Offset.Height -= crop.getTop();

			// normalize size and offset to field width/height
			// HACKHACK/TRYTRY: I believe (WRONG! TODO) this is the only location where those numbers
			// (40 = field width in pixels, 36 = field height in pixels) are ever
			// needed. If not, put them somewhere sensible as constants
			frame.Size = new Size(frame.BitmapBuilder.Width / 40.0f,
															frame.BitmapBuilder.Height / 36.0f);

			frame.Offset.Width /= 40.0f;
			frame.Offset.Height /= 36.0f;

//			if (ExtraData)
//			{
//				/*InputStream f = File.OpenWrite(@"out\" + frame.FileName + ".bmp");
//				int uh;
//				while ((uh = frame.ImageStream.ReadByte()) != -1)
//					f.WriteByte((byte)uh);
//				f.Close();*/
//				new Bitmap(frame.BitmapBuilder.GetStream()).Save(@"out\" + frame.FileName + ".png", ImageFormat.Png);
//			}

			return bytesRead;
		}

		/// <summary>
		/// parse an ani file "SEQ " item
		/// </summary>
		/// <param name="s">the stream to read from. must be skippable</param>
		/// <param name="r">a DataInputStream for s</param>
		/// <param name="bytesRead">current position in the file</param>
		/// <param name="itemId">item id read from item header</param>
		/// <param name="itemLength">item length</param>
		/// <param name="itemStart">item start position in the file</param>
		/// <param name="itemEnd">item end position</param>
		/// <returns>the new position in the file</returns>
		private int fromArgb(int r, int g, int b) {
			throw new NotImplementedException();
		}

		private long ParseSequence(InputStream s, BinaryReader r, long bytesRead, int itemId, int itemLength, long itemStart, long itemEnd) throws IOException, FormatException
		{
			assert itemStart == bytesRead;
			int iSeq = Sequences.size();
			Animation seq = new Animation();
			Sequences.add(seq);
			//List<AnimationState> states = new ArrayList<AnimationState>();
			List<Integer> frames = new ArrayList<Integer>();
			List<Short> framesX = new ArrayList<Short>();
			List<Short> framesY = new ArrayList<Short>();

			if (itemId != 0)
				OutputInfo("Seq{0} id: " + itemId, iSeq);

			int nSeqHeads = 0;

			while (bytesRead < itemEnd)
			{
				if (bytesRead + 10 > itemEnd)
					throw new FormatException("Seq" + iSeq + " Item header too short (" + (itemEnd - bytesRead) + ")");

				String seqItemSignature = new String(r.ReadChars(4));
				int seqItemLength = r.readUInt32();
				int seqItemId = r.ReadUInt16();
				long seqItemStart = (bytesRead += 10);
				long seqItemEnd = seqItemStart + seqItemLength;

				if (bytesRead + seqItemLength > itemEnd)
					throw new FormatException("Seq" + iSeq + " Item " + seqItemSignature
								+ " (" + seqItemId + ") of length " + seqItemLength
								+ " does not fit in sequence with " + (itemEnd - bytesRead) + " bytes left");

				switch (seqItemSignature)
				{
					case "HEAD":
						{
							++nSeqHeads;

							if (nSeqHeads > 1)
								OutputInfo("Multiple seq{0} heads ^^", iSeq);

							if (seqItemId != 1)
								OutputInfo("Seq{0} head id: " + seqItemId, iSeq);

							if (seqItemLength != 96)
								OutputInfo("Seq{0} head size: " + seqItemLength, iSeq);


							StringBuilder seqName = new StringBuilder();
							while (bytesRead++ != seqItemEnd)
							{
								char c = r.ReadChar();

								if (c == '\0')
									break;

								seqName.append(c);
							}
							seq.setName(seqName.toString());

							if (bytesRead > seqItemEnd)
								throw new FormatException("Seq" + iSeq + " Head name is not terminated");


							s.skip(seqItemEnd - bytesRead);
							bytesRead = seqItemEnd;
							break;
						}
					case "STAT":
						{
							int iStat = frames.size();
							//AnimationState stat;
							//states.Add(stat = new AnimationState());
//							int frame;
//							short frameX, frameY;
							bytesRead = ParseSequenceState(s, r, bytesRead, iSeq, iStat, seqItemId, seqItemLength, seqItemStart, seqItemEnd);
							frames.add(frame);
							framesX.add(frameX);
							framesY.add(frameY);
							break;
						}
					default:
						OutputInfo("Unknown seq{0} item " + seqItemSignature, iSeq);
						s.skip(seqItemLength);
						bytesRead += seqItemLength;
						break;
				}
				assert bytesRead == seqItemEnd;
			}

			//seq.States = states.ToArray();
			seq.Frames = new AnimationFrame[frames.size()];
			seq.FrameOffset = new Size[frames.size()];
			for (int i = 0; i < frames.size(); ++i)
			{
				if (frames.get(i) >= Frames.size())
					throw new FormatException("Seq" + iSeq + "State" + i + "Frame" + i + " tells us to use frame " + frames.get(i) + ", which doesn't exist");
				
				seq.Frames[i] = Frames.get(frames.get(i));
				// HACKHACK: field width/height in pixels. See other comment
				seq.FrameOffset[i] = new Size(framesX.get(i) / 40.0f, framesY.get(i) / 36.0f);
			}

			return bytesRead;
		}


		private long ParseSequenceState(InputStream s, BinaryReader r, long bytesRead, int iSeq, int iStat, /*AnimationState stat,*/int seqItemId, int seqItemLength, long seqItemStart, long seqItemEnd) throws IOException, FormatException
		{
			assert seqItemStart == bytesRead;
			long a;

			if (seqItemId != 0)
				OutputInfo("Seq{0}State{1} id: " + seqItemId, iSeq, iStat);

			int nStatHeads = 0,
				nStatFrames = 0;

			// shut up warnings. This is actually unnecessary
			frame = 0; frameX = frameY = 0;

			while (bytesRead < seqItemEnd)
			{
				if (bytesRead + 10 > seqItemEnd)
					throw new FormatException("Seq" + iSeq + "State" + iStat + " Item header too short (" + (seqItemEnd - bytesRead) + ")");

				String statItemSignature = r.ReadChars(4);
				int statItemLength = r.readUInt32();
				int statItemId = r.ReadUInt16();
				long statItemStart = (bytesRead += 10);
				long statItemEnd = statItemStart + statItemLength;

				if (bytesRead + statItemLength > seqItemEnd)
					throw new FormatException("Seq" + iSeq + "State" + iStat + " Item " + statItemSignature
								+ " (" + statItemId + ") of length " + statItemLength
								+ " does not fit in state with " + (seqItemEnd - bytesRead) + " bytes left");

				switch (statItemSignature)
				{
					case "HEAD":
						{
							++nStatHeads;

							if (nStatHeads > 1)
								OutputInfo("Multiple seq{0}state{1} heads", iSeq, iStat);

							if (statItemId != 1)
								OutputInfo("Seq{0}State{1} head id: " + statItemId, iSeq, iStat);

							if (statItemLength != 46)
							{
								OutputInfo("Seq{0}State{1} head size: " + statItemLength, iSeq, iStat);
								s.skip(statItemLength);
							}
							else
							{
								// this is some additional info stored with each state
								/*stat.NewSpeed = readInt16(r);
								stat.MoveX = readInt16(r);
								stat.MoveY = readInt16(r);
								for (int i = 0; i < 16; ++i)
									stat.StateValues[i] = readInt16(r);
						
								// now the bitmask
								a = readUInt16(r);
						
								for (int i = 0; i < 16; ++i)
									if ((a & (2 << i)) == 0)
										stat.StateValues[i] = int.MaxValue;
								*/
								// we don't need the above info, so just skip it
								s.skip(40);

								// now there should be 3 zero-words
								if ((a = r.ReadUInt16()) != 0)
									OutputInfo("Seq{0}State{1} head word 1: " + a, iSeq, iStat);
								if ((a = r.ReadUInt16()) != 0)
									OutputInfo("Seq{0}State{1} head word 2: " + a, iSeq, iStat);
								if ((a = r.ReadUInt16()) != 0)
									OutputInfo("Seq{0}State{1} head word 3: " + a, iSeq, iStat);
							}

							bytesRead += statItemLength;
							break;
						}
					case "FRAM":
						{
							++nStatFrames;

							if (nStatFrames > 1)
								OutputInfo("Multiple seq{0}state{1} frames o_O", iSeq, iStat);

							if (statItemId != 1)
								OutputInfo("Seq{0}State{1} frame id: " + statItemId, iSeq, iStat);

							if (statItemLength < 2)
								throw new FormatException("Seq" + iSeq + "State" + iStat + " frame too short");

							// TRYTRY: why the heck can there be multiple frames in a state?!
							int nFramesThisItem = r.ReadUInt16();
							bytesRead += 2;

							if (nFramesThisItem > 1)
							{
								// System.out.printf("Multiple frames ({0}) in Seq" + iSeq + "State" + iStat, nFramesThisItem);
							}

							if (statItemEnd - bytesRead < nFramesThisItem * 10)
								throw new FormatException("Seq" + iSeq + "State" + iStat + " frame too short for " + nFramesThisItem + " frames");

							for (int i = 0; i < nFramesThisItem; ++i)
							{
								// frame number
								frame = r.ReadUInt16();

								//stat.FrameX = readInt16(r);
								//stat.FrameY = readInt16(r);
								frameX = (short) r.ReadInt16();
								frameY = (short) r.ReadInt16();

								if ((a = r.readUInt32()) != 0)
									OutputInfo("Seq{0}State{1}Frame{2} end dword: " + a, iSeq, iStat, i);
								bytesRead += 10;
							}

							if (bytesRead < statItemEnd)
							{
								OutputInfo("Seq{0}State{1} has {2} bytes of extra data", iSeq, iStat,
													statItemEnd - bytesRead);
								s.skip(statItemEnd - bytesRead);
								bytesRead = statItemEnd;
							}

							break;
						}
					case "RECT":
					case "POIN":
						// known but ignored
						s.skip(statItemLength);
						bytesRead += statItemLength;
						break;
					default:
						OutputInfo("Unknown seq{0}stat{1} item " + statItemSignature, iSeq, iStat);
						s.skip(statItemLength);
						bytesRead += statItemLength;
						break;
				}
			}

			if (nStatFrames == 0)
				throw new FormatException("Seq" + iSeq + "State" + iStat + " has no frames");

			return bytesRead;
		}
	}