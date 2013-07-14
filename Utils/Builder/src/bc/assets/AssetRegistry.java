package bc.assets;

import java.util.HashMap;
import java.util.Map;

public class AssetRegistry
{
	private static Map<Class<?>, AssetInfo> registry = new HashMap<Class<?>, AssetInfo>();
	
	public static void register(Class<?> cls, AssetInfo info)
	{
		registry.put(cls, info);
	}
	
	public static AssetInfo find(Class<?> cls)
	{
		return registry.get(cls);
	}
}
