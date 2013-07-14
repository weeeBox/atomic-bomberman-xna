package bc.assets;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

import bc.assets.types.Animation;
import bc.assets.types.BitmapFont;
import bc.assets.types.Music;
import bc.assets.types.Scheme;
import bc.assets.types.Sound;
import bc.assets.types.Texture;
import bc.tasks.ContentProjTask;


public class AssetPackage
{
	private List<Asset> resources = new ArrayList<Asset>();

	private String name;

	public AssetPackage()
	{
	}

	public AssetPackage(String name)
	{
		this.name = name;
	}

	public String getName()
	{
		return name;
	}

	public void setName(String name)
	{
		this.name = name;
	}

	public void process()
	{
		String productName = name;
		File productsDir = new File(ContentProjTask.resDir, productName);
		productsDir.mkdir();

		List<Asset> resourcesCopy = new ArrayList<Asset>(resources.size());
		resourcesCopy.addAll(resources);

		for (Asset resource : resourcesCopy)
		{
			System.out.println("Process: " + resource);
			resource.process();
		}
	}

	public void addImage(Texture image)
	{
		addResource(image);
	}

	public void addPixelFont(BitmapFont font)
	{
		addResource(font);
	}

	public void addSound(Sound sound)
	{
		addResource(sound);
	}

	public void addMusic(Music music)
	{
		addResource(music);
	}
	
	public void addScheme(Scheme scheme)
	{
		addResource(scheme);
	}
	
	public void addAnimation(Animation animation)
	{
		addResource(animation);
	}

	private void addResource(Asset res)
	{
		res.setPackage(this);
		resources.add(res);
	}

	public List<Asset> getResources()
	{
		return resources;
	}

	@Override
	public String toString()
	{
		StringBuilder buf = new StringBuilder();
		buf.append("Package: " + name);
		for (Asset res : resources)
		{
			buf.append("\n\t" + res);
		}
		return buf.toString();
	}
}
