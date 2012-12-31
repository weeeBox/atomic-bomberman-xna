package bc.bomberman;

import bc.bomberman.utils.Size;

public class AnimationFrame {

	public String FileName;
	public Size Offset;
	public int RawKeyColor;
	public int KeyColor;
	public BitmapBuilder BitmapBuilder;
	public Size Size;

	public String getFileName() {
		return FileName;
	}

	public void setFileName(String fileName) {
		FileName = fileName;
	}

}
