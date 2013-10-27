
namespace BomberEngine
{
    public interface AssetManagerListener
    {
        void OnResourceLoaded(AssetManager manager, AssetLoadInfo info);
        void OnResourcesLoaded(AssetManager manager);
    }
}
