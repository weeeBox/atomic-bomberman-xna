using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using BomberEngine.Debugging;
using BomberEngine.Game;
using BomberEngine.Core.Assets.Loaders;
using BomberEngine.Core.Assets.Types;
using Microsoft.Xna.Framework.Graphics;
using BomberEngine.Core.Visual;
using BomberEngine.Core.IO;
using BomberEngine.Core.Assets.Readers;

namespace BomberEngine.Core.Assets
{
    public abstract class AssetManager
    {
        private ContentManager contentManager;

        private AssetManagerListener listener;

        private Dictionary<int, AssetLoader> loaders;
        private Dictionary<Type, AssetReader> readers;

        private Asset[] assets;

        private List<AssetLoadInfo> loadingQueue;

        private int loadedCount;
        
        private DelayedCall loadingTimer;

        private VectorFont systemFont;

        public AssetManager(ContentManager contentManager, int resourcesCount)
        {
            this.contentManager = contentManager;

            assets = new Asset[resourcesCount];
            loadingQueue = new List<AssetLoadInfo>();
            InitLoaders();
            InitReaders();
        }

        private void InitLoaders()
        {
            loaders = new Dictionary<int, AssetLoader>();
            loaders.Add(AssetTypeBase.Texture, new TextureLoader());
            loaders.Add(AssetTypeBase.VectorFont, new VectorFontLoader());
        }

        private void InitReaders()
        {
            readers = new Dictionary<Type, AssetReader>();
            readers.Add(typeof(TextureReader), new TextureReader());
        }

        public void AddToQueue(AssetLoadInfo info)
        {
            Debug.Assert(!IsAssetLoaded(info.index), "Resource already loaded: " + info.path);
            Debug.Assert(!loadingQueue.Contains(info), "Resource already in the loading queue: " + info.path);

            loadingQueue.Add(info);
        }

        public void LoadImmediately()
        {   
            foreach (AssetLoadInfo info in loadingQueue)
            {
                LoadResource(info);
            }
            loadingQueue.Clear();
        }

        public void Load()
        {
            Application.ScheduleTimer(OnTimer, 0.05f);
        }

        protected void RegisterLoader(int type, AssetLoader loader)
        {
            loaders.Add(type, loader);
        }

        protected void RegisterReader(Type type, AssetReader reader)
        {
            readers.Add(type, reader);
        }

        public TextureImage GetTexture(int id)
        {
            return (TextureImage)GetAsset(id);
        }

        public Font GetFont(int id)
        {
            return (Font)GetAsset(id);
        }

        public Asset GetAsset(int id)
        {
            return assets[id];
        }

        public T LoadAsset<T>(String path) where T : Asset
        {
            Type type = typeof(T);
            AssetReader reader;
            if (readers.TryGetValue(type, out reader))
            {
                using (System.IO.Stream stream = System.IO.File.OpenRead(path))
                {
                    return (T) reader.Read(stream);
                }
            }

            return default(T);
        }

        private void OnTimer(DelayedCall timer)
        {
            AssetLoadInfo info = loadingQueue[loadedCount];
            if (LoadResource(info))
            {
                ++loadedCount;

                if (listener != null)
                {
                    listener.OnResourceLoaded(this, info);
                }

                if (loadingQueue.Count == loadedCount)
                {
                    if (listener != null)
                    {
                        GC.Collect();
                        listener.OnResourcesLoaded(this);
                    }
                    timer.Cancel();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected bool LoadResource(AssetLoadInfo info)
        {
            AssetLoader loader = loaders.ContainsKey(info.type) ? loaders[info.type] : null;

            if (loader == null)
            {
                throw new InvalidOperationException("Loader not found for resource type: " + info.type);
            }

            Asset asset = loader.LoadAsset(contentManager, info);
            Debug.Assert(asset != null, "Loader returned null: " + info.path);

            assets[info.index] = asset;

            return asset != null;
        }

        private bool IsAssetLoaded(int id)
        {
            return assets[id] != null;
        }

        public virtual VectorFont SystemFont
        {
            get
            {
                if (systemFont == null)
                {
                    systemFont = new VectorFont(contentManager.Load<SpriteFont>("SystemFont"));
                }
                return systemFont;
            }
        }
    }
}
