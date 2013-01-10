package bc.tasks;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import org.apache.tools.ant.BuildException;
import org.dom4j.Document;
import org.dom4j.DocumentException;
import org.dom4j.Element;
import org.dom4j.io.OutputFormat;
import org.dom4j.io.SAXReader;
import org.dom4j.io.XMLWriter;

import bc.assets.Asset;

public class ContentProjSync
{
	private static final String ELEMENT_ITEM_GROUP = "ItemGroup";
	private static final String ELEMENT_COMPILE = "Compile";
	private static final String ELEMENT_GENERATED_MARKER = "GeneratedByBuilder";
	private static final String ELEMENT_IMPORTER = "Importer";
	private static final String ELEMENT_PROCESSOR = "Processor";
	private static final String ELEMENT_ASSET_NAME = "Name";
	private static final String ELEMENT_INCLUDE = "Include";

	private List<Asset> resources;

	public ContentProjSync()
	{
		resources = new ArrayList<Asset>();
	}

	public void addResource(Asset res)
	{
		resources.add(res);
	}

	public void sync(File projFile)
	{
		try
		{
			Document doc = new SAXReader().read(projFile);
			processContentProj(doc, projFile);
		}
		catch (DocumentException e)
		{
			e.printStackTrace();
			throw new BuildException(e.getMessage());
		}
	}

	private void processContentProj(Document doc, File projFile)
	{
		if (resources.size() > 0)
		{
			clearOldItems(doc);
			addNewItems(doc);
			writeDocument(doc, projFile);
		}
	}

	private void clearOldItems(Document doc)
	{
		List<Element> itemGroups = doc.getRootElement().elements(ELEMENT_ITEM_GROUP);
		for (Element e : itemGroups)
		{
			clearItemsGroupElement(e);

			if (e.elements().isEmpty())
			{
				e.getParent().remove(e);
			}
		}
	}

	private void clearItemsGroupElement(Element element)
	{
		List<Element> children = element.elements();
		for (Element child : children)
		{
			if (child.getName().equals(ELEMENT_COMPILE))
			{
				Element generatedElement = child.element(ELEMENT_GENERATED_MARKER);
				if (generatedElement != null)
				{
					element.remove(child);
				}
			}
		}
	}

	private void addNewItems(Document doc)
	{
		Element parent = doc.getRootElement().addElement(ELEMENT_ITEM_GROUP);
		for (Asset res : resources)
		{
			addResource(res, parent);
		}
	}

	private void addResource(Asset res, Element parent)
	{
		Element element = parent.addElement(ELEMENT_COMPILE);
		element.addAttribute(ELEMENT_INCLUDE, res.getDestFile().getName());
		element.addElement(ELEMENT_ASSET_NAME).addText(res.getShortName());
		element.addElement(ELEMENT_IMPORTER).addText(res.getImporter());
		String processor = res.getProcessor();
		if (processor != null)
		{
			element.addElement(ELEMENT_PROCESSOR).addText(processor);
		}
		element.addElement(ELEMENT_GENERATED_MARKER).addText("true");
	}

	private void writeDocument(Document doc, File file)
	{
		try
		{
			FileOutputStream stream = new FileOutputStream(file);

			OutputFormat format = OutputFormat.createPrettyPrint();
			XMLWriter writer = new XMLWriter(stream, format);
			writer.write(doc);
			writer.flush();

			stream.close();
			writer.close();
		}
		catch (IOException e)
		{
			e.printStackTrace();
		}
	}
}
