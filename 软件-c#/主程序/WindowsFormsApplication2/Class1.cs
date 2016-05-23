using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication2
{
    public enum RETURN_CODE
    {
        ApiSuccess = 0x200,
        ApiFailed,
        ApiAccessDenied,
        ApiDmaChannelUnavailable,
        ApiDmaChannelInvalid,
        ApiDmaChannelTypeError,
        ApiDmaInProgress,
        ApiDmaDone,
        ApiDmaPaused,
        ApiDmaNotPaused,
        ApiDmaCommandInvalid,
        ApiDmaManReady,
        ApiDmaManNotReady,
        ApiDmaInvalidChannelPriority,
        ApiDmaManCorrupted,
        ApiDmaInvalidElementIndex,
        ApiDmaNoMoreElements,
        ApiDmaSglInvalid,
        ApiDmaSglQueueFull,
        ApiNullParam,
        ApiInvalidBusIndex,
        ApiUnsupportedFunction,
        ApiInvalidPciSpace,
        ApiInvalidIopSpace,
        ApiInvalidSize,
        ApiInvalidAddress,
        ApiInvalidAccessType,
        ApiInvalidIndex,
        ApiMuNotReady,
        ApiMuFifoEmpty,
        ApiMuFifoFull,
        ApiInvalidRegister,
        ApiDoorbellClearFailed,
        ApiInvalidUserPin,
        ApiInvalidUserState,
        ApiEepromNotPresent,
        ApiEepromTypeNotSupported,
        ApiEepromBlank,
        ApiConfigAccessFailed,
        ApiInvalidDeviceInfo,
        ApiNoActiveDriver,
        ApiInsufficientResources,
        ApiObjectAlreadyAllocated,
        ApiAlreadyInitialized,
        ApiNotInitialized,
        ApiBadConfigRegEndianMode,
        ApiInvalidPowerState,
        ApiPowerDown,
        ApiFlybyNotSupported,
        ApiNotSupportThisChannel,
        ApiNoAction,
        ApiHSNotSupported,
        ApiVPDNotSupported,
        ApiVpdNotEnabled,
        ApiNoMoreCap,
        ApiInvalidOffset,
        ApiBadPinDirection,
        ApiPciTimeout,
        ApiDmaChannelClosed,
        ApiDmaChannelError,
        ApiInvalidHandle,
        ApiBufferNotReady,
        ApiInvalidData,
        ApiDoNothing,
        ApiDmaSglBuildFailed,
        ApiPMNotSupported,
        ApiInvalidDriverVersion,
        ApiWaitTimeout,
        ApiWaitCanceled,
        ApiLastError
    };

    public enum IOP_SPACE
    {
        IopSpace0,
        IopSpace1,
        IopSpace2,
        IopSpace3,
        MsLcs0,
        MsLcs1,
        MsLcs2,
        MsLcs3,
        MsDram,
        MsDefault,
        ExpansionRom
    };

    public enum ACCESS_TYPE
    {
        BitSize8,
        BitSize16,
        BitSize32,
        BitSize64
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DEVICE_LOCATION
    {
        public byte BusNumber;
        public byte SlotNumber;
        public UInt16 DeviceId;
        public UInt16 VendorId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] SerialNumber;
    }

    public enum DMA_CHANNEL
    {
        IopChannel0,
        IopChannel1,
        IopChannel2,
        PrimaryPciChannel0,
        PrimaryPciChannel1,
        PrimaryPciChannel2,
        PrimaryPciChannel3
    }

    public enum DMA_CHANNEL_PRIORITY
    {
        Channel0Highest,
        Channel1Highest,
        Channel2Highest,
        Channel3Highest,
        Rotational
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DMA_CHANNEL_DESC
    {
        private ulong data;
        public ulong EnableReadyInput//0
        {
            get{return data & 0x1;}
            set{data |= value & 0x1;}
        }
        public ulong EnableBTERMInput//1
        {
            get{return (data>>1) & 0x1;}
            set{data |= (value & 0x1)<<1;}
        }
        public ulong EnableIopBurst//2
        {
            get{return (data>>2) & 0x1;}
            set{data |= (value & 0x1)<<2;}
        }
        public ulong EnableWriteInvalidMode//3
        {
            get{return (data>>3) & 0x1;}
            set{data |= (value & 0x1)<<3;}
        }
        public ulong EnableDmaEOTPin//4
        {
            get{return (data>>4) & 0x1;}
            set{data |= (value & 0x1)<<4;}
        }
        public ulong DmaStopTransferMode//5
        {
            get{return (data>>5) & 0x1;}
            set{data |= (value & 0x1)<<5;}
        }
        public ulong HoldIopAddrConst//6
        {
            get{return (data>>6) & 0x1;}
            set{data |= (value & 0x1)<<6;}
        }
        public ulong HoldIopSourceAddrConst//7
        {
            get{return (data>>7) & 0x1;}
            set{data |= (value & 0x1)<<7;}
        }
        public ulong HoldIopDestAddrConst//8
        {
            get{return (data>>8) & 0x1;}
            set{data |= (value & 0x1)<<8;}
        }
        public ulong DemandMode//9
        {
            get{return (data>>9) & 0x1;}
            set{data |= (value & 0x1)<<9;}
        }
        public ulong SrcDemandMode//10
        {
            get{return (data>>10) & 0x1;}
            set{data |= (value & 0x1)<<10;}
        }
        public ulong DestDemandMode//11
        {
            get{return (data>>11) & 0x1;}
            set{data |= (value & 0x1)<<11;}
        }
        public ulong EnableTransferCountClear//12
        {
            get{return (data>>12) & 0x1;}
            set{data |= (value & 0x1)<<12;}
        }
        public ulong WaitStates//13~16
        {
            get{return (data>>13) & 0xf;}
            set{data |= (value & 0xf)<<13;}
        }
        public ulong IopBusWidth//17~18
        {
            get{return (data>>17) & 0x3;}
            set{data |= (value & 0x3)<<17;}
        }
        public ulong EOTEndLink//19
        {
            get{return (data>>19) & 0x1;}
            set{data |= (value & 0x1)<<19;}
        }
        public ulong ValidStopControl//20
        {
            get{return (data>>20) & 0x1;}
            set{data |= (value & 0x1)<<20;}
        }
        public ulong ValidModeEnable//21
        {
            get{return (data>>21) & 0x1;}
            set{data |= (value & 0x1)<<21;}
        }
        public ulong EnableDualAddressCycles//22
        {
            get{return (data>>22) & 0x1;}
            set{data |= (value & 0x1)<<22;}
        }
        public ulong Reserved1//23~31
        {
            get{return (data>>23) & 0x1ff;}
            set{data |= (value & 0x1ff)<<23;}
        }
        public ulong TholdForIopWrites//32~35
        {
            get{return (data>>32) & 0xf;}
            set{data |= (value & 0xf)<<32;}
        }
        public ulong TholdForIopReads//36~39
        {
            get{return (data>>36) & 0xf;}
            set{data |= (value & 0xf)<<36;}
        }
        public ulong TholdForPciWrites//40~43
        {
            get{return (data>>40) & 0xf;}
            set{data |= (value & 0xf)<<40;}
        }
        public ulong TholdForPciReads//44~47
        {
            get{return (data>>44) & 0xf;}
            set{data |= (value & 0xf)<<44;}
        }
        public ulong EnableFlybyMode//48
        {
            get{return (data>>48) & 0x1;}
            set{data |= (value & 0x1)<<48;}
        }
        public ulong FlybyDirection//49
        {
            get{return (data>>48) & 0x1;}
            set{data |= (value & 0x1)<<48;}
        }
        public ulong EnableDoneInt//50
        {
            get{return (data>>50) & 0x1;}
            set{data |= (value & 0x1)<<50;}
        }
        public ulong Reserved2//51~63
        {
            get { return (data >> 51) & 0x1fff; }
            set { data |= (value & 0x1fff) << 51; }
        }
        public DMA_CHANNEL_PRIORITY DmaChannelPriority;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PLX_INTR
    {
        public ulong data;
        public ulong InboundPost      //0
        {
            get{return data & 0x1;}
            set{data |= value & 0x1;}
        }
        public ulong OutboundPost     //1
        {
            get{return (data>>1) & 0x1;}
            set{data |= (value & 0x1)<<1;}
        }
        public ulong OutboundOverflow //2
        {
            get{return (data>>2) & 0x1;}
            set{data |= (value & 0x1)<<2;}
        }
        public ulong OutboundOption   //3
        {
            get{return (data>>3) & 0x1;}
            set{data |= (value & 0x3)<<1;}
        }
        public ulong IopDmaChannel0   //4
        {
            get{return (data>>4) & 0x1;}
            set{data |= (value & 0x1)<<4;}
        }
        public ulong PciDmaChannel0   //5
        {
            get{return (data>>5) & 0x1;}
            set{data |= (value & 0x1)<<5;}
        }
        public ulong IopDmaChannel1   //6
        {
            get{return (data>>6) & 0x1;}
            set{data |= (value & 0x1)<<6;}
        }
        public ulong PciDmaChannel1   //7
        {
            get{return (data>>7) & 0x1;}
            set{data |= (value & 0x1)<<7;}
        }
        public ulong IopDmaChannel2   //8
        {
            get{return (data>>8) & 0x1;}
            set{data |= (value & 0x1)<<8;}
        }
        public ulong PciDmaChannel2   //9
        {
            get{return (data>>9) & 0x1;}
            set{data |= (value & 0x1)<<9;}
        }
        public ulong Mailbox0         //10
        {
            get{return (data>>10) & 0x1;}
            set{data |= (value & 0x1)<<10;}
        }
        public ulong Mailbox1         //11
        {
            get{return (data>>11) & 0x1;}
            set{data |= (value & 0x1)<<11;}
        }
        public ulong Mailbox2         //12
        {
            get{return (data>>12) & 0x1;}
            set{data |= (value & 0x1)<<12;}
        }
        public ulong Mailbox3         //13
        {
            get{return (data>>13) & 0x1;}
            set{data |= (value & 0x1)<<13;}
        }
        public ulong Mailbox4         //14
        {
            get{return (data>>14) & 0x1;}
            set{data |= (value & 0x1)<<14;}
        }
        public ulong Mailbox5         //15
        {
            get{return (data>>15) & 0x1;}
            set{data |= (value & 0x1)<<15;}
        }
        public ulong Mailbox6         //16
        {
            get{return (data>>16) & 0x1;}
            set{data |= (value & 0x1)<<16;}
        }
        public ulong Mailbox7         //17
        {
            get{return (data>>17) & 0x1;}
            set{data |= (value & 0x1)<<17;}
        }
        public ulong IopDoorbell      //18
        {
            get{return (data>>18) & 0x1;}
            set{data |= (value & 0x1)<<18;}
        }
        public ulong PciDoorbell      //19
        {
            get{return (data>>19) & 0x1;}
            set{data |= (value & 0x1)<<19;}
        }
        public ulong SerialPort1      //20
        {
            get{return (data>>20) & 0x1;}
            set{data |= (value & 0x1)<<20;}
        }
        public ulong SerialPort2      //21
        {
            get{return (data>>21) & 0x1;}
            set{data |= (value & 0x1)<<21;}
        }
        public ulong BIST             //22
        {
            get{return (data>>22) & 0x1;}
            set{data |= (value & 0x1)<<22;}
        }
        public ulong PowerManagement  //23
        {
            get{return (data>>23) & 0x1;}
            set{data |= (value & 0x1)<<23;}
        }
        public ulong PciMainInt       //24
        {
            get{return (data>>24) & 0x1;}
            set{data |= (value & 0x1)<<24;}
        }
        public ulong IopToPciInt      //25
        {
            get{return (data>>25) & 0x1;}
            set{data |= (value & 0x1)<<25;}
        }
        public ulong IopMainInt       //26
        {
            get{return (data>>26) & 0x1;}
            set{data |= (value & 0x1)<<26;}
        }
        public ulong PciAbort         //27
        {
            get{return (data>>27) & 0x1;}
            set{data |= (value & 0x1)<<27;}
        }
        public ulong PciReset         //28
        {
            get{return (data>>28) & 0x1;}
            set{data |= (value & 0x1)<<28;}
        }
        public ulong PciPME           //29
        {
            get{return (data>>29) & 0x1;}
            set{data |= (value & 0x1)<<29;}
        }
        public ulong Enum             //30
        {
            get{return (data>>30) & 0x1;}
            set{data |= (value & 0x1)<<30;}
        }
        public ulong PciENUM          //31
        {
            get{return (data>>31) & 0x1;}
            set{data |= (value & 0x31)<<1;}
        }
        public ulong IopBusTimeout    //32
        {
            get{return (data>>32) & 0x1;}
            set{data |= (value & 0x32)<<1;}
        }
        public ulong AbortLSERR       //33
        {
            get{return (data>>33) & 0x1;}
            set{data |= (value & 0x1)<<33;}
        }
        public ulong ParityLSERR      //34
        {
            get{return (data>>34) & 0x1;}
            set{data |= (value & 0x1)<<34;}
        }
        public ulong RetryAbort       //35
        {
            get{return (data>>35) & 0x1;}
            set{data |= (value & 0x1)<<35;}
        }
        public ulong LocalParityLSERR //36
        {
            get{return (data>>36) & 0x1;}
            set{data |= (value & 0x1)<<36;}
        }
        public ulong PciSERR          //37
        {
            get{return (data>>37) & 0x1;}
            set{data |= (value & 0x1)<<37;}
        }
        public ulong IopRefresh       //38
        {
            get{return (data>>38) & 0x1;}
            set{data |= (value & 0x1)<<38;}
        }
        public ulong PciINTApin       //39
        {
            get{return (data>>39) & 0x1;}
            set{data |= (value & 0x1)<<39;}
        }
        public ulong IopINTIpin       //40
        {
            get{return (data>>40) & 0x1;}
            set{data |= (value & 0x1)<<40;}
        }
        public ulong TargetAbort      //41
        {
            get{return (data>>41) & 0x1;}
            set{data |= (value & 0x1)<<41;}
        }
        public ulong Ch1Abort         //42
        {
            get{return (data>>42) & 0x1;}
            set{data |= (value & 0x1)<<42;}
        }
        public ulong Ch0Abort         //43
        {
            get{return (data>>43) & 0x1;}
            set{data |= (value & 0x1)<<43;}
        }
        public ulong DMAbort          //44
        {
            get{return (data>>44) & 0x1;}
            set{data |= (value & 0x1)<<44;}
        }
        public ulong IopToPciInt_2    //45
        {
            get{return (data>>45) & 0x1;}
            set{data |= (value & 0x1)<<45;}
        }
        public ulong SwInterrupt      //46
        {
            get{return (data>>46) & 0x1;}
            set{data |= (value & 0x1)<<46;}
        }
        public ulong DmaChannel3      //47
        {
            get{return (data>>47) & 0x1;}
            set{data |= (value & 0x1)<<47;}
        }
        public ulong Reserved         //48~63
        {
            get { return (data >> 48) & 0xffff; }
            set { data |= (value & 0xffff) << 48; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DMA_TRANSFER_ELEMENT
    {
        public UInt32 UserVa; // User space virtual address
        public UInt32 PciAddrHigh; // Upper 32-bits of PCI address
        public UInt32 LocalAddr; // Local bus address
        public UInt32 DestAddr; // ** no longer supported **
        public UInt32 TransferCount; // Number of bytes to transfer
        public UInt32 NextSglPtr; // ** no longer supported **
        private UInt32 data;
        public UInt32 PciSglLoc // ** no longer supported **
        {
            get{return data & 0x1;}
            set{data |= value & 0x1;}
        }
        public UInt32 LastSglElement // ** no longer supported **
        {
            get{return (data>>1) & 0x1;}
            set{data |= (value & 0x1)<<1;}
        }
        public UInt32 TerminalCountIntr
        {
            get{return (data>>2) & 0x1;}
            set{data |= (value & 0x1)<<2;}
        }
        public UInt32 LocalToPciDma
        {
            get { return (data >> 3) & 0x1; }
            set { data |= (value & 0x1) << 3; }
        }
    }
}
