using JetImageLoader;
using JetImageLoader.Cache;
using JetImageLoader.Cache.Memory;
using JetImageLoader.Cache.Memory.CacheImpl;
using JetImageLoader.Cache.Storage;
using JetImageLoader.Cache.Storage.CacheFileNameGenerators;
using JetImageLoader.Cache.Storage.CacheImpl;
using JetImageLoader.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Split_It_.Converter
{
    public class MyAppJetImageLoaderConverter : BaseJetImageLoaderConverter
    {
        public static readonly BaseMemoryCache<string, Stream> MemoryCacheImpl = new WeakMemoryCache<string, Stream>();
        public static readonly BaseStorageCache StorageCacheImpl = 
            new LimitedStorageCache(IsolatedStorageFile.GetUserStoreForApplication(),
                "\\TempImages", new SHA1CacheFileNameGenerator(), 1024 * 1024 * 10);

        protected override JetImageLoaderConfig GetJetImageLoaderConfig()
        {
            return new JetImageLoaderConfig.Builder
            {
                IsLogEnabled = true,
                CacheMode = CacheMode.MemoryAndStorageCache,
                DownloaderImpl = new HttpWebRequestDownloader(),
                MemoryCacheImpl = MemoryCacheImpl,
                StorageCacheImpl = StorageCacheImpl
            }.Build();
        }
    }
}
