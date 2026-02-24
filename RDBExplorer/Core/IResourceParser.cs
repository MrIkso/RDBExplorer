using System;
using System.Collections.Generic;
using System.Text;

namespace RDBExplorer.Core
{
    public class EntryData
    {
        public string? Name { get; set; }

        public byte[]? Data { get; set; }
    }

    public interface IResourceParser
    {
        public string? GetJsonData();

        public List<EntryData> GetEntries();
        void Load(byte[] data);
        object RawModel { get; }

        public bool IsConvertedToText { get; }
    }

    public abstract class ResourceWrapper<TModel> : IResourceParser
    {
        public TModel Model { get; protected set; }

        public object RawModel => Model;

        public abstract bool IsConvertedToText { get; }

        public abstract void Load(byte[] data);
        public abstract string? GetJsonData();
        public abstract List<EntryData> GetEntries();
    }
}
