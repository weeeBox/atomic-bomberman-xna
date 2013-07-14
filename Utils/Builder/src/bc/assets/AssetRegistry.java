package bc.assets;

import java.util.HashMap;
import java.util.Map;

public class AssetRegistry
{
	private static Map<Class<?>, ContentInfo<?, ?>> registry = new HashMap<Class<?>, ContentInfo<?, ?>>();
	
	public static void register(Class<?> cls, ContentInfo<?, ?> info)
	{
		registry.put(cls, info);
	}
	
	public static ContentInfo<?, ?> find(Class<?> cls)
	{
		return registry.get(cls);
	}
}
