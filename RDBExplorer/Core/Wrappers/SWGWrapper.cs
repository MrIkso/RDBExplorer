using RDBExplorer.Core.Formats.KTID;
using RDBExplorer.Core.Formats.SWG;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RDBExplorer.Core.Wrappers
{
    internal class SWGWrapper : ResourceWrapper<SWGFile>
    {
        private readonly SWGParser _parser = new SWGParser();

        public override void Load(byte[] data)
        {
            _parser.Load(data);
            Model = _parser.File;
        }

        public override async Task SerializeJsonToStreamAsync(Stream stream)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            };

            await JsonSerializer.SerializeAsync(stream, Model, options);
        }

        public override List<EntryData> GetEntries() => new();

        public override bool IsConvertedToText => true;

    }
}
