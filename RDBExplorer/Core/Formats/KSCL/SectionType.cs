using System;
using System.Collections.Generic;
using System.Text;

namespace RDBExplorer.Core.Formats.KSCL
{
    public enum SectionType : uint
    {
        TextureStage = 0x00,
        Pane = 0x01,
        Group = 0x02,
        ViewInfo = 0x03,
        TextureStageDict = 0x04,
        Unknown5 = 0x05,
        TextData = 0x06,
        Animation = 0x07,
        FCurve = 0x08,
        AnimTag = 0x09,
        AnimTagEx = 0x0A,
        FreeShape1 = 0x0B,
        FreeShape2 = 0x0C,
        Hierarchy = 0x0D,
        PaneVtxControl = 0x11
    }
}
