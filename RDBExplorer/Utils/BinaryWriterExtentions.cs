using System;
using System.Collections.Generic;
using System.Text;

namespace RDBExplorer.Utils
{
    public static class BinaryWriterExtentions
    {
        public static void PadToAlignment(this BinaryWriter writer, int alignment)
        {
            long padding = (alignment - (writer.BaseStream.Position % alignment)) % alignment;
            for (int i = 0; i < padding; i++)
            {
                writer.Write((byte)0);
            }
        }

        public static void WritePadding(this BinaryWriter writer, int count)
        {
            if (count <= 0) 
                return;

            writer.Write(new byte[count]); 
            
        }
    }
}
