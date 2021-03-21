using System;

namespace MsdfGen
{
    public unsafe class Msdf : IDisposable
    {
        private readonly MsdfNative.Msdf* _nativeHandle;

        public Msdf(MsdfMode mode, int width, int height)
        {
            _nativeHandle = MsdfNative.MSDF_Create(mode, width, height);
            MsdfNative.CheckAndThrow();

            Shape = new Shape(MsdfNative.MSDF_GetShape(_nativeHandle));
        }

        public static  delegate*<MsdfNative.Shape*, double, ulong, void> GetColoringStrategy(MsdfColoringStrategy coloringStrategy)
        {
            return MsdfNative.MSDF_GetColoringStrategy(coloringStrategy);
        }
        
        public bool TestRenderMulti(MsdfNative.BitmapBase* renderDestination)
        {
            if (MsdfNative.MSDF_TestRenderMulti(_nativeHandle, renderDestination))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }
        public bool ApplyOrientation()
        {
            if (MsdfNative.MSDF_ApplyOrientation(_nativeHandle))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }

        public bool ApplyOutputDistanceShift()
        {
            if (MsdfNative.MSDF_ApplyOutputDistanceShift(_nativeHandle))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }
        public bool ApplyScanlinePass()
        {
            if (MsdfNative.MSDF_ApplyScanlinePass(_nativeHandle))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }

        public bool Save(string filename, Format format)
        {
            if (MsdfNative.MSDF_Save(_nativeHandle, filename, format))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }

        public bool ColorEdges(delegate*<MsdfNative.Shape*, double, ulong, void> edgeColoring, ulong coloringSeed, bool skipColoring, string edgeAssignment, double angleThreshold)
        {
            if (MsdfNative.MSDF_ColorEdges(_nativeHandle, edgeColoring, coloringSeed, skipColoring, edgeAssignment, angleThreshold))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }
        public bool Generate()
        {
            if (MsdfNative.MSDF_Generate(_nativeHandle))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }

        public Shape Shape { get; }

        private BitmapBase _bitmap;

        public BitmapBase Bitmap
        {
            get
            {
                var bmpHandle = MsdfNative.MSDF_GetData(_nativeHandle);

                if (_bitmap != null)
                {
                    if (_bitmap.NativeHandle == bmpHandle)
                        return _bitmap;
                    
                    _bitmap.Dispose();
                }

                _bitmap = new BitmapBase(bmpHandle);
                return _bitmap;
            }
        }

        public bool ConvertTo8Bit()
        {
            if (MsdfNative.MSDF_ConvertTo8Bit(_nativeHandle))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }

        public void PrintMetrics(bool autoFrame)
        {
            MsdfNative.MSDF_PrintMetrics(_nativeHandle, autoFrame);
            MsdfNative.CheckAndThrow();
        }

        public void AutoFrame()
        {
            MsdfNative.MSDF_AutoFrame(_nativeHandle);
            MsdfNative.CheckAndThrow();
        }
        public void SetTranslation(double x, double y)
        {
            MsdfNative.MSDF_SetTranslation(_nativeHandle, x, y);
            MsdfNative.CheckAndThrow();
        }
        public void SetScale(double x, double y)
        {
            MsdfNative.MSDF_SetScale(_nativeHandle, x, y);
            MsdfNative.CheckAndThrow();
        }
        public void GetTranslation(out double x, out double y)
        {
            MsdfNative.MSDF_GetTranslation(_nativeHandle, out x, out y);
            MsdfNative.CheckAndThrow();
        }
        public void GetScale(out double x, out double y)
        {
            MsdfNative.MSDF_GetScale(_nativeHandle, out x, out y);
            MsdfNative.CheckAndThrow();
        }
        
        public double Range
        {
            get
            {
                var ret = MsdfNative.MSDF_GetRange(_nativeHandle);
                MsdfNative.CheckAndThrow();
                return ret;
            }
            set
            {
                MsdfNative.MSDF_SetRange(_nativeHandle, value);
                MsdfNative.CheckAndThrow();
            }
        }
        
        public MsdfNative.Msdf* NativeHandle => _nativeHandle;

        public void Dispose()
        {
            MsdfNative.MSDF_Delete(_nativeHandle);
            MsdfNative.CheckAndThrow();
        }
    }
}