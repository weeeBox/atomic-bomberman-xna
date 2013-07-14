package bc.tasks;

import java.io.File;

import org.apache.tools.ant.BuildException;
import org.apache.tools.ant.Task;

import bc.assets.AssetDir;

public class ContentTask extends Task
{
	private File codeFile;
	private File outputDir;

	private AssetDir rootDir;
	
	public ContentTask()
	{
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

		if (rootDir == null)
			throw new BuildException("Add a root dir element");
	}

	public void addDir(AssetDir dir)
	{
		if (rootDir != null)
			throw new BuildException("Only 1 root dir allowed");
		
		rootDir = dir;
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
