package bc.assets;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

import bc.assets.types.Animation;
import bc.assets.types.PixelFontRes;
import bc.assets.types.SchemeAsset;
import bc.assets.types.TextureAsset;
import bc.assets.types.MusicAsset;
import bc.assets.types.SoundAsset;
import bc.assets.types.VectorFontAsset;
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

	public void addImage(TextureAsset image)
	{
		addResource(image);
	}

	public void addPixelFont(PixelFontRes font)
	{
		addResource(font);
	}

	public void addVectorFont(VectorFontAsset font)
	{
		addResource(font);
	}

	public void addSound(SoundAsset sound)
	{
		addResource(sound);
	}

	public void addMusic(MusicAsset music)
	{
		addResource(music);
	}
	
	public void addScheme(SchemeAsset scheme)
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
