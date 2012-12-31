package bc.bomberman;

import bc.bomberman.utils.Size;


public class Animation {

	private String Name;
	AnimationFrame[] Frames;
	Size[] FrameOffset;

	public String getName() {
		return Name;
	}

	public void setName(String name) {
		Name = name;
	}

	public AnimationFrame[] getFrames() {
		return Frames;
	}

	public void setFrames(AnimationFrame[] frames) {
		Frames = frames;
	}

	public Size[] getFrameOffset() {
		return FrameOffset;
	}

	public void setFrameOffset(Size[] frameOffset) {
		FrameOffset = frameOffset;
	}

}
