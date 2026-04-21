namespace RDBExplorer.Core.Models
{
    public struct RdxEntry
    {
        public ushort Index;
        public byte SubdirIndex; // if not 0xFF, path is {subdirs[subdirIndex]}/ instead of ./ (this is game dependent)
        public byte LocaleIndex; // if not 0xFF, path is {selectedLanguage[localeIndex]}/0x{%08x}.fdata
        public uint FileId;
    }
}
