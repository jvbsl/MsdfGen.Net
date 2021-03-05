using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using edgeColoringFunc_t = System.IntPtr;
namespace MsdfGen
{
    public static unsafe class MsdfNative
    {
        public static void CheckAndThrow()
        {
            var exceptions = new List<MsdfException>();
            do
            {
                var msg = MSDF_GetErrorMessage();
                if (msg == IntPtr.Zero)
                    break;
                exceptions.Add(new MsdfException(Marshal.PtrToStringAnsi(msg)));
            } while (true);

            if (exceptions.Count == 1)
                throw exceptions.First();
            if (exceptions.Count == 0)
                return;

            throw new AggregateException(exceptions);
        }
        
        public struct Msdf { }
        public struct Shape { }

        public readonly struct FontHandle
        {
            public FontHandle(IntPtr faceHandle, bool ownership = false)
            {
                FaceHandle = faceHandle;
                Ownership = ownership;
            }
            public IntPtr FaceHandle { get; }
            public bool Ownership { get; }
        }

        public struct BitmapBase { }
        
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_Shape_LoadFromFreetypeFont(Shape* shape, FontHandle* fontHandle, uint glyphIndex, uint unicode);
        
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_Shape_LoadFromFontFile(Shape* shape, string fontFileName, uint glyphIndex, uint unicode);
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_Shape_LoadFromSVGFile(Shape* shape, string svgFileName, int svgPathIndex);
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_Shape_LoadFromDescriptionFile(Shape* shape, string descriptionFile, ref bool skipColoring);
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_Shape_PreprocessGeometry(Shape* shape);
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_Shape_Validate(Shape* shape);
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_Shape_Export(Shape* shape, string fileName);
        [DllImport("msdfgen_shared")]
        public static extern void MSDF_Shape_Normalize(Shape* shape);
        [DllImport("msdfgen_shared")]
        public static extern void MSDF_Shape_FlipY(Shape* shape, bool flipped);
    
        [DllImport("msdfgen_shared")]
        public static extern void MSDF_AutoFrame(Msdf* msdf);

        [DllImport("msdfgen_shared")]
        public static extern BitmapBase* MSDF_Bitmap_Create(int channels, int width, int height);
        [DllImport("msdfgen_shared")]
        public static extern void MSDF_Bitmap_Delete(BitmapBase* bitmapBase);
    
        [DllImport("msdfgen_shared")]
        public static extern void* MSDF_Bitmap_GetPixelData(BitmapBase* bitmap, out int width, out int height, out int channelCount);


        //public static extern bool MSDF_TestRender(MSDF* msdf, msdfgen::Bitmap<float, 1>& output);
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_TestRenderMulti(Msdf* msdf, BitmapBase* renderDestination);
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_ApplyOrientation(Msdf* msdf);

        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_ApplyOutputDistanceShift(Msdf* msdf);
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_ApplyScanlinePass(Msdf* msdf);

        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_Save(Msdf* msdf, string filename, Format format);
    
        [DllImport("msdfgen_shared")]
        public static extern void MSDF_Initialize(Msdf* msdf, MsdfMode mode, int width, int height);
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_ColorEdges(Msdf* msdf,  delegate*<Shape*, double, ulong, void> edgeColoring, ulong coloringSeed, bool skipColoring, string edgeAssignment, double angleThreshold);
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_Generate(Msdf* msdf);

        [DllImport("msdfgen_shared")]
        public static extern Shape* MSDF_GetShape(Msdf* msdf);

        [DllImport("msdfgen_shared")]
        public static extern BitmapBase* MSDF_GetData(Msdf* msdf);
        
        [DllImport("msdfgen_shared")]
        public static extern bool MSDF_ConvertTo8Bit(Msdf* msdf);
    
        [DllImport("msdfgen_shared")]
        public static extern void MSDF_PrintMetrics(Msdf* msdf, bool autoFrame);
    
        [DllImport("msdfgen_shared")]
        public static extern Msdf* MSDF_Create(MsdfMode mode, int width, int height);
        [DllImport("msdfgen_shared")]
        public static extern void MSDF_Delete(Msdf* msdf);
    
        [DllImport("msdfgen_shared")]
        public static extern IntPtr MSDF_GetErrorMessage();
        
        [DllImport("msdfgen_shared")]
        public static extern delegate*<Shape*, double, ulong, void> MSDF_GetColoringStrategy(MsdfColoringStrategy coloringStrategy);
    }
}