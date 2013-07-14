package bc.assets.types;

import java.awt.image.BufferedImage;

import bc.assets.Asset;

public class Texture extends Asset
{
	private BufferedImage image;
	
	public Texture(BufferedImage image)
	{
		this.image = image;
	}
	
	public BufferedImage getImage()
	{
		return image;
	}
}
