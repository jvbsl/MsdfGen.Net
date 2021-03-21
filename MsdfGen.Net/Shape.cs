namespace MsdfGen
{
    public unsafe class Shape
    {
        private readonly MsdfNative.Shape* _nativeHandle;
        internal Shape(MsdfNative.Shape* nativeHandle)
        {
            _nativeHandle = nativeHandle;
        }

        public bool LoadFromFreetypeFont(MsdfNative.FontHandle* fontHandle, uint glyphIndex, uint unicode)
        {
            if (MsdfNative.MSDF_Shape_LoadFromFreetypeFont(_nativeHandle, fontHandle, glyphIndex, unicode))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }

        public bool LoadFromFontFile(string fontFileName, uint glyphIndex, uint unicode)
        {
            if (MsdfNative.MSDF_Shape_LoadFromFontFile(_nativeHandle, fontFileName, glyphIndex, unicode))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }
        public bool LoadFromSvgFile(string svgFileName, int svgPathIndex)
        {
            if (MsdfNative.MSDF_Shape_LoadFromSVGFile(_nativeHandle, svgFileName, svgPathIndex))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }
        public bool LoadFromDescriptionFile(string descriptionFile, ref bool skipColoring)
        {
            if (MsdfNative.MSDF_Shape_LoadFromDescriptionFile(_nativeHandle, descriptionFile, ref skipColoring))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }
        public bool PreprocessGeometry()
        {
            if (MsdfNative.MSDF_Shape_PreprocessGeometry(_nativeHandle))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }
        public bool Validate()
        {
            if (MsdfNative.MSDF_Shape_Validate(_nativeHandle))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }
        public bool Export(string fileName)
        {
            if (MsdfNative.MSDF_Shape_Export(_nativeHandle, fileName))
                return true;
            MsdfNative.CheckAndThrow();
            return false;
        }
        public void Normalize()
        {
            MsdfNative.MSDF_Shape_Normalize(_nativeHandle);
            MsdfNative.CheckAndThrow();
        }
        public void FlipY(bool flipped)
        {
            MsdfNative.MSDF_Shape_FlipY(_nativeHandle, flipped);
            MsdfNative.CheckAndThrow();
        }

        public void GetBounds(out double left, out double right, out double top, out double bottom)
        {
            MsdfNative.MSDF_Shape_GetBounds(_nativeHandle, out left, out right, out top, out bottom);
            MsdfNative.CheckAndThrow();
        }
    }
}