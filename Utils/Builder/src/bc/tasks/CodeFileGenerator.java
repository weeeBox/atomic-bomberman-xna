package bc.tasks;

import java.io.File;
import java.io.IOException;
import java.util.List;

import bc.assets.AssetPackage;
import bc.assets.AssetInfo;
import bc.utils.filesystem.FileUtils;
import bc.utils.output.ListWriteDestination;
import bc.utils.output.WriteDestination;

public class CodeFileGenerator
{
	private static final String ASSET_NAMES_CLASS = "A";
	private static final String ASSET_LOAD_INFO_CLASS = "AssetLoadInfo";

	public void generate(File file, List<AssetPackage> packs) throws IOException
	{
		ListWriteDestination dest = new ListWriteDestination();

		writeHeader(dest);
		writeCode(dest, packs);
		writeEnding(dest);

		List<String> lines = dest.getLines();
		FileUtils.writeFile(file, lines, "utf-8");

		System.out.println("File created: " + file);
	}

	private void writeHeader(WriteDestination dest) throws IOException
	{
		dest.writeln("// This file was generated. Do not modify.");
		dest.writeln();
		dest.writeln("using BomberEngine.Core.Assets;");
		dest.writeln("using BomberEngine.Core.Assets.Types;");
		dest.writeln("using Bomberman.Content;");
		dest.writeln();
		dest.writeln("namespace Assets");
		dest.writeBlockOpen();
	}

	private void writeCode(WriteDestination dest, List<AssetPackage> packs) throws IOException
	{
		writeResIds(dest, packs);
		dest.writeln();
		writeResInfos(dest, packs);
	}

	private void writeResIds(WriteDestination out, List<AssetPackage> packs)
	{
		out.writeln("public class %s", ASSET_NAMES_CLASS);
		out.writeBlockOpen();

		int resIndex = 0;
		for (AssetPackage pack : packs)
		{
			out.writeln("// " + pack.getName().toUpperCase());

			List<AssetInfo> packResources = pack.getAssets();
			for (AssetInfo res : packResources)
			{
				out.writeln("public const int %s = %d;", res.getId(), resIndex++);
			}
		}
		out.writeln("// total resources count");
		out.writeln("public const int RES_COUNT = %d;", resIndex);

		out.writeBlockClose();
	}

	private void writeResInfos(WriteDestination out, List<AssetPackage> packs)
	{
		out.writeln("public class AssetPacks");
		out.writeBlockOpen();
		
		out.writeln("public enum Packs");
		out.writeBlockOpen();
		for (int packIndex = 0; packIndex < packs.size(); packIndex++)
		{
			String packName = packs.get(packIndex).getName();
			out.writeln("%s%s", packName, packIndex < packs.size() - 1 ? "," : "");
		}
		out.writeBlockClose();
		
		out.writeln();
		
		out.writeln("private static readonly %s[][] PACKS =", ASSET_LOAD_INFO_CLASS);
		out.writeBlockOpen();

		for (AssetPackage pack : packs)
		{
			writePackInfo(out, pack);
		}

		out.writeBlockClose(true);
		
		out.writeln();
		out.writeln("public static %s[] GetPack(Packs pack)", ASSET_LOAD_INFO_CLASS);
		out.writeBlockOpen();
		out.writeln("return PACKS[(int)pack];");
		out.writeBlockClose();
		
		out.writeBlockClose();
	}

	private void writePackInfo(WriteDestination out, AssetPackage pack)
	{
		out.writeln("// " + pack.getName().toUpperCase());
		out.writeln("new %s[]", ASSET_LOAD_INFO_CLASS);
		out.writeBlockOpen();

		List<AssetInfo> packResources = pack.getAssets();
		for (AssetInfo res : packResources)
		{
			writeResInfo(out, res);
		}
		out.writeBlockClose();
	}

	private void writeResInfo(WriteDestination out, AssetInfo res)
	{
		out.writeln("new %s(%s, typeof(%s), \"%s\"),", ASSET_LOAD_INFO_CLASS, res.getId(), res.getRuntimeType(), res.getRelativePath().replace(File.separator, "\\\\"));
	}

	private void writeEnding(WriteDestination out) throws IOException
	{
		out.writeBlockClose();
	}
}