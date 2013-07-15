using System;
using System.Collections.Generic;
using BomberEngine.Core.Assets.Readers;
using BomberEngine.Core.Assets.Types;
using BomberEngine.Debugging;
using BomberEngine.Game;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace BomberEngine.Core.Assets
{
    public abstract class AssetManager
    {
        private string baseDir;

        private AssetManagerListener listener;

        private Dictionary<Type, AssetReader> readers;
        private Asset[] assets;

        private List<AssetLoadInfo> loadingQueue;
        private int loadedCount;
        
        private DelayedCall loadingTimer;

        public AssetManager(String baseDir, int assetCount)
        {
            this.baseDir = baseDir;

            assets = new Asset[assetCount];
            loadingQueue = new List<AssetLoadInfo>();
            InitReaders();
        }

        private void InitReaders()
        {
            readers = new Dictionary<Type, AssetReader>();
            readers.Add(typeof(TextureImage), new TextureReader());
        }

        public void AddToQueue(AssetLoadInfo info)
        {
            Debug.Assert(!IsAssetLoaded(info.id), "Resource already loaded: " + info.id);
            Debug.Assert(!loadingQueue.Contains(info), "Resource already in the loading queue: " + info.id);

            loadingQueue.Add(info);
        }

        public void LoadImmediately()
        {   
            foreach (AssetLoadInfo info in loadingQueue)
            {
                LoadAsset(info);
            }
            loadingQueue.Clear();
        }

        public void Load()
        {
            Application.ScheduleCall(OnTimer, 0.05f);
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

        protected bool LoadAsset(AssetLoadInfo info)
        {
            Debug.Assert(!IsAssetLoaded(info.id));

            Asset asset = LoadAsset(info.path, info.type);
            assets[info.id] = asset;
            return asset != null;
        }


        public T LoadAsset<T>(String path) where T : Asset
        {   
            return (T)LoadAsset(path, typeof(T));
        }

        private Asset LoadAsset(String path, Type type)
        {
            AssetReader reader;
            if (readers.TryGetValue(type, out reader))
            {
                using (System.IO.Stream stream = System.IO.File.OpenRead(Path.Combine(baseDir, path)))
                {
                    return reader.Read(stream);
                }
            }

            throw new InvalidOperationException("Can't find reader for: " + type);
        }

        private void OnTimer(DelayedCall timer)
        {
            AssetLoadInfo info = loadingQueue[loadedCount];
            if (LoadAsset(info))
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
                throw new InvalidOperationException("Can't load asset: " + info.path);
            }
        }

        private bool IsAssetLoaded(int id)
        {
            return GetAsset(id) != null;
        }
    }
}
