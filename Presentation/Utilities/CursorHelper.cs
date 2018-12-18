using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32.SafeHandles;

namespace Presentation
{
    public class CursorHelper
    {
        private static class NativeMethods
        {
            public struct IconInfo
            {
                public bool fIcon;
                public int xHotspot;
                public int yHotspot;
                public IntPtr hbmMask;
                public IntPtr hbmColor;
            }

            [DllImport("user32.dll")]
            public static extern SafeIconHandle CreateIconIndirect(ref IconInfo icon);

            [DllImport("user32.dll")]
            public static extern bool DestroyIcon(IntPtr hIcon);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        private class SafeIconHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public SafeIconHandle()
                : base(true)
            {
            }

            override protected bool ReleaseHandle()
            {
                return NativeMethods.DestroyIcon(handle);
            }
        }

        private static Cursor InternalCreateCursor(System.Drawing.Bitmap bmp, int xHotspot = 0, int yHotspot = 0)
        {
            var iconInfo = new NativeMethods.IconInfo();
            var handler = bmp.GetHicon();
            NativeMethods.GetIconInfo(handler, ref iconInfo);
            NativeMethods.DestroyIcon(handler);

            iconInfo.xHotspot = xHotspot;
            iconInfo.yHotspot = yHotspot;
            iconInfo.fIcon = false;

            var cursorHandle = NativeMethods.CreateIconIndirect(ref iconInfo);
            return CursorInteropHelper.Create(cursorHandle);

        }

        public static Cursor CreateCursor(Image image)
        {
            image.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            image.Arrange(new Rect(new Point(), image.DesiredSize));

            var width = (int)image.DesiredSize.Width;
            var heigth = (int)image.DesiredSize.Height;

            GC.WaitForPendingFinalizers();

            RenderTargetBitmap rtb =
              new RenderTargetBitmap(
                width,
                heigth,
                100, 96, PixelFormats.Pbgra32);

            rtb.Render(image);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                using (var bmp = new System.Drawing.Bitmap(ms))
                {
                    return InternalCreateCursor(bmp, width / 2, heigth / 2);
                }
            }
        }
    }
}
