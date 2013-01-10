package bc.tasks;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

import org.apache.tools.ant.BuildException;

import bc.utils.filesystem.FileUtils;

public class ProjectFileSync 
{
	private List<File> files;
	
	public ProjectFileSync()
	{
		files = new ArrayList<File>();
	}	
	
	public void addFile(File file)
	{
		if (file == null)
			throw new BuildException("Can't add null file");
		
		files.add(file);
	}
	
	public void sync(File contentDir)
	{		
		for (File file : files) 
		{
			FileUtils.copy(file, contentDir);
		}
	}
}
