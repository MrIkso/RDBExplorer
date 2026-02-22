namespace RDBExplorer.Core.Models
{
    public enum KoeiPlatform : uint
    {
        PS2 = 0x00, // <- didn't find a game with g1t files
        PS3 = 0x01, // Special Tiled Z Morton Swizzling
        X360 = 0x02, // Extra Special Tiling Swizzling
        NWii = 0x03, // 
        NDS = 0x04, // <- didn't find a game with g1t files
        N3DS = 0x05, // 
        PSVita = 0x06, // 
        Android = 0x07, // 
        iOS = 0x08, // 
        NWiiU = 0x09, // Big Endian
        WinMac = 0x0A, // They share the same enum
        PS4 = 0x0B, // Special Z Morton Swizzling
                    //XOne     = 0x0C, // <- Need rom to confrim
                    //???      = 0x0D, //
        WinDX12 = 0x0E, // 
                        //???      = 0x0F, //
        NSwitch = 0x10, // 
                        //???      = 0x11, //
                        //???      = 0x12, //
        PS5 = 0x13, //
    }
}
