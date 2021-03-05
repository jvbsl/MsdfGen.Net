using System;

namespace MsdfGen
{
    public unsafe class BitmapBase : IDisposable
    {
        private readonly bool _owner;

        public BitmapBase(MsdfNative.BitmapBase* bitmapBase, bool owner = false)
        {
            NativeHandle = bitmapBase;
            _owner = false;

            PixelData = MsdfNative.MSDF_Bitmap_GetPixelData(NativeHandle, out var width, out var height, out var channelCount);

            Width = width;
            Height = height;
            ChannelCount = channelCount;
            MsdfNative.CheckAndThrow();
        }
        public BitmapBase(int channels, int width, int height)
            : this(MsdfNative.MSDF_Bitmap_Create(channels, width, height), true)
        {
        }
        
        public void* PixelData { get; }
        
        public int ChannelCount { get; }
        
        public int Width { get; }
        public int Height { get; }

        public MsdfNative.BitmapBase* NativeHandle { get; }

        public void Dispose()
        {
            if (_owner)
                MsdfNative.MSDF_Bitmap_Delete(NativeHandle);
        }
    }
}