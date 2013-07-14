package bc.tasks;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

import org.apache.tools.ant.BuildException;
import org.apache.tools.ant.Task;

import bc.assets.AssetDir;

public class ContentTask extends Task
{
	private File codeFile;
	private File outputDir;
	
	private List<AssetDir> dirList;
	
	public ContentTask()
	{
		dirList = new ArrayList<AssetDir>();
	}
	
	@Override
	public void execute() throws BuildException
	{
		checkParams();
		
	}
	
	private void checkParams()
	{
		if (codeFile == null)
			throw new BuildException("Missing 'codeFile' attribute");
		
		if (codeFile.exists() && codeFile.isDirectory())
			throw new BuildException("File is a directory: " + codeFile);
		
		if (outputDir == null)
			throw new BuildException("Missing 'outputDir' attribute");
		
		if (outputDir.exists() && !outputDir.isDirectory())
			throw new BuildException("File is not a directory: " + outputDir);
	}

	public void addDir(AssetDir dir)
	{
		dirList.add(dir);
	}
	
	public void setCodeFile(File codeFile)
	{
		this.codeFile = codeFile;
	}
	
	public void setOutputDir(File outputDir)
	{
		this.outputDir = outputDir;
	}
}
