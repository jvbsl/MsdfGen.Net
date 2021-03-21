using System;
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MsdfGen.Tests
{
    public class Tests
    {
        
        static PixelFormat PixelFormatFromChannelCount(int channelCount)
        {
            switch (channelCount)
            {
                case 1:
                    return PixelFormat.Format8bppIndexed;
                case 3:
                    return PixelFormat.Format24bppRgb;
                case 4:
                    return PixelFormat.Format32bppArgb;
                default:
                    throw new ArgumentOutOfRangeException(nameof(channelCount));
            }
        }
        [SetUp]
        public void Setup()
        {
        }

        private unsafe bool CompareBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1.Width != bmp2.Width || bmp1.Height != bmp2.Height || bmp1.PixelFormat != bmp2.PixelFormat)
                return false;
            var rect = new Rectangle(new Point(), bmp1.Size);
            var bmp1Data = bmp1.LockBits(rect, ImageLockMode.ReadOnly, bmp1.PixelFormat);
            var bmp2Data = bmp2.LockBits(rect, ImageLockMode.ReadOnly, bmp2.PixelFormat);
            var count = bmp1Data.Stride * bmp1.Height;
            if (count != bmp2Data.Stride * bmp2.Height)
                return false;
            var ptr1 = (int*)bmp1Data.Scan0;
            var ptr2 = (int*)bmp2Data.Scan0;
            for (int i = 0; i < count/4; i++)
            {
                if (ptr1[i] != ptr2[i])
                    return false;
            }

            return true;
        }
        static unsafe void SingleSetValue(byte* dest, float* source)
        {
            *dest = (byte) Math.Clamp((int) (*source * 256f), 0, 255);
        }
        
        static unsafe void RGBSetValue(byte* dest, float* source)
        {
            SingleSetValue(dest, source + 2);
            SingleSetValue(dest + 1, source + 1);
            SingleSetValue(dest + 2, source + 0);
        }
        static unsafe void ARGBSetValue(byte* dest, float* source)
        {
            RGBSetValue(dest, source);
            SingleSetValue(dest + 3, source + 3);
        }

        [Test]
        public void SimpleShapeTest()
        {
            var msdf = new Msdf(MsdfMode.MultiAndTrue, 32, 32);

            bool skipColoring = false;
            //Assert.True(msdf.Shape.LoadFromDescriptionFile("test.shape", ref skipColoring), "Failed loading shape!");
            Assert.True(msdf.Shape.LoadFromFontFile("/usr/share/wine/fonts/arial.ttf", 0, (uint)'B'), "Failed loading shape!");

            Assert.True(msdf.Shape.Validate(), "Failed validating shape!");

            bool geometryPreprocess = false;

            if (geometryPreprocess)
                Assert.True(msdf.Shape.PreprocessGeometry(), "Shape preprocessing failed!");
            
            msdf.Shape.Normalize();
            
            msdf.AutoFrame();
            unsafe
            {
                msdf.ColorEdges(Msdf.GetColoringStrategy(MsdfColoringStrategy.Simple), 0, skipColoring, null, 3.0);
            }

            Assert.True(msdf.Generate(), "Generating failed!");

            Assert.True(msdf.ApplyOrientation(), "Apply orientation failed!");

            Assert.True(msdf.ApplyOutputDistanceShift(), "Apply output distance shift failed!");

            var msdfRes = msdf.Bitmap;
            using var bmp = new Bitmap(msdfRes.Width, msdfRes.Height, PixelFormatFromChannelCount(msdfRes.ChannelCount));
            if (msdfRes.ChannelCount == 1)
            {
                for (int i = 0; i < 256; i++)
                    bmp.Palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }

            var bmpData = bmp.LockBits(new Rectangle(new Point(), bmp.Size), ImageLockMode.WriteOnly, bmp.PixelFormat);
            unsafe
            {
                byte* dest = (byte*) bmpData.Scan0;
                float* source = (float*) msdfRes.PixelData;

                delegate*<byte*, float*, void> setValue = null;
                if (msdfRes.ChannelCount == 1)
                    setValue = &SingleSetValue;
                if (msdfRes.ChannelCount == 3)
                    setValue = &RGBSetValue;
                if (msdfRes.ChannelCount == 4)
                    setValue = &ARGBSetValue;

                for (int y = 0; y < msdfRes.Height; y++)
                {
                    for (int x = 0; x < msdfRes.Width; x++)
                    {
                        int indexDest = ((msdfRes.Height - y - 1) * msdfRes.Width) + x;
                        int indexSource = (y * msdfRes.Width) + x;
                        setValue(&dest[indexDest * msdfRes.ChannelCount], &source[indexSource * msdfRes.ChannelCount]);
                    }
                }

            }
            bmp.UnlockBits(bmpData);
            
            bmp.Save("SimpleShapeTest2.png", ImageFormat.Png);

            if (!msdf.Save("SimpleShapeTest.png", Format.Auto))
                Console.WriteLine("Msdf saving failed!");

            using var bmp2 = new Bitmap("SimpleShapeTest.png");
            using (Bitmap newBmp = new Bitmap(bmp2))
            using (Bitmap convertedBmp = newBmp.Clone(new Rectangle(0, 0, newBmp.Width, newBmp.Height), PixelFormat.Format32bppArgb))
                Assert.True(CompareBitmaps(bmp, convertedBmp), "Direct Result and indirect result differ!");
        }
    }
}