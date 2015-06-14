using System;
using System.Drawing;
using System.Drawing.Imaging;
using MMD_NCHLShader2Pre;

namespace MMD_NCHLShader2Pre
{
    public class NRMBuilder
    {
        public static bool BuildNRM(string TexPath,string TargetPath,bool Overwrite)
        {
            if (!Overwrite)
            {
                if (System.IO.File.Exists(TargetPath))
                {
                    return true;
                }
            }
            Image SImage=ImageLoader.Load(TexPath);
            if (SImage != null)
            {
                try
                {
                    NormalMapBuilder.NGener NG = new NormalMapBuilder.NGener();
                    NG.LoadPic(SImage);
                    Bitmap NBmp = NG.Map;
                    if (NBmp != null)
                    {
                        System.IO.File.Delete(TargetPath);
                        NBmp.Save(TargetPath, ImageFormat.Png);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch { return false; }
            }
            else
            {
                return false;
            }
        }
    }
}
/*
 
 Forked As:https://github.com/scskarsper/NormalMapCreator
 
 */
namespace NormalMapBuilder
{
    struct PixelData
    {
        public Byte b;
        public Byte g;
        public Byte r;
        public Byte a;
    }

    public class NGener
    {
        Bitmap m_bmp_image;
        Bitmap m_bmp_nmap;
        Bitmap m_bmp_nmap_blur;
        Bitmap m_bmp_channel;
        bool Genered = false;
        unsafe private void GreyscaleChannel(char channel)
        {
            int x = 0, y = 0, src_stride = 0, dst_stride = 0;
            PixelData* p_src = null, p_dst = null;

            System.Drawing.Imaging.BitmapData bd_src = m_bmp_nmap.LockBits(new Rectangle(0, 0, m_bmp_nmap_blur.Width, m_bmp_nmap_blur.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData bd_dst = m_bmp_channel.LockBits(new Rectangle(0, 0, m_bmp_channel.Width, m_bmp_channel.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            p_src = (PixelData*)bd_src.Scan0.ToPointer();
            p_dst = (PixelData*)bd_dst.Scan0.ToPointer();

            src_stride = bd_src.Stride / 4;
            dst_stride = bd_dst.Stride / 4;

            for (y = 0; y < bd_src.Height; y++)
            {
                for (x = 0; x < bd_src.Width; x++)
                {
                    switch (channel)
                    {
                        case 'R':
                            (p_dst + x)->a = 255;
                            (p_dst + x)->r = (Byte)((p_src + x)->r);
                            (p_dst + x)->g = (Byte)((p_src + x)->r); ;
                            (p_dst + x)->b = (Byte)((p_src + x)->r); ;
                            break;
                        case 'G':
                            (p_dst + x)->a = 255;
                            (p_dst + x)->r = (Byte)((p_src + x)->g);
                            (p_dst + x)->g = (Byte)((p_src + x)->g); ;
                            (p_dst + x)->b = (Byte)((p_src + x)->g); ;
                            break;
                        case 'B':
                            (p_dst + x)->a = 255;
                            (p_dst + x)->r = (Byte)((p_src + x)->b);
                            (p_dst + x)->g = (Byte)((p_src + x)->b); ;
                            (p_dst + x)->b = (Byte)((p_src + x)->b); ;
                            break;
                    }
                }
                p_src += src_stride;
                p_dst += dst_stride;
            }

            m_bmp_channel.UnlockBits(bd_dst);
            m_bmp_nmap.UnlockBits(bd_src);
        }

        unsafe private void GenerateNormalMap()
        {
            int x = 0, y = 0, kx = 0, ky = 0, src_stride = 0, dst_stride = 0, sumX = 128, sumY = 128, sumZ = 0;
            PixelData* p_src = null, p_dst = null;
            int[,] kernelX = new int[,] { { 1, 2, 1 }, { 0, 0, 0 }, { -1, -2, -1 } };
            int[,] kernelY = new int[,] { { 1, 0, -1 }, { 2, 0, -2 }, { 1, 0, -1 } };
            int[,] offsetX = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            int[,] offsetY = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

            System.Drawing.Imaging.BitmapData bd_src = m_bmp_image.LockBits(new Rectangle(0, 0, m_bmp_image.Width, m_bmp_image.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData bd_dst = m_bmp_nmap.LockBits(new Rectangle(0, 0, m_bmp_nmap.Width, m_bmp_nmap.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            p_src = (PixelData*)bd_src.Scan0.ToPointer();
            p_dst = (PixelData*)bd_dst.Scan0.ToPointer();

            src_stride = bd_src.Stride / 4;
            dst_stride = bd_dst.Stride / 4;

            for (y = 0; y < bd_src.Height; y++)
            {
                // Set Y offsets for image boundaries
                if (y == 0)
                {
                    offsetY[0, 0] = (bd_src.Height - 1) * src_stride;
                    offsetY[1, 0] = (bd_src.Height - 1) * src_stride;
                    offsetY[2, 0] = (bd_src.Height - 1) * src_stride;
                    offsetY[0, 2] = 0;
                    offsetY[1, 2] = 0;
                    offsetY[2, 2] = 0;
                }
                else if (y == bd_src.Height - 1)
                {
                    offsetY[0, 0] = 0;
                    offsetY[1, 0] = 0;
                    offsetY[2, 0] = 0;
                    offsetY[0, 2] = -((bd_src.Height - 1) * src_stride);
                    offsetY[1, 2] = -((bd_src.Height - 1) * src_stride);
                    offsetY[2, 2] = -((bd_src.Height - 1) * src_stride);
                }
                else if (y == 1 || y == bd_src.Height - 2)
                {
                    offsetY[0, 0] = 0;
                    offsetY[1, 0] = 0;
                    offsetY[2, 0] = 0;
                    offsetY[0, 2] = 0;
                    offsetY[1, 2] = 0;
                    offsetY[2, 2] = 0;
                }

                for (x = 0; x < bd_src.Width; x++)
                {
                    (p_dst + (y * dst_stride) + x)->a = 255;

                    // Set X offsets for image boundaries
                    if (x == 0)
                    {
                        offsetX[0, 0] = bd_src.Width - 1;
                        offsetX[0, 1] = bd_src.Width - 1;
                        offsetX[0, 2] = bd_src.Width - 1;
                        offsetX[2, 0] = 0;
                        offsetX[2, 1] = 0;
                        offsetX[2, 2] = 0;
                    }
                    else if (x == bd_src.Width - 1)
                    {
                        offsetX[0, 0] = 0;
                        offsetX[0, 1] = 0;
                        offsetX[0, 2] = 0;
                        offsetX[2, 0] = -bd_src.Width;
                        offsetX[2, 1] = -bd_src.Width;
                        offsetX[2, 2] = -bd_src.Width;
                    }
                    else if (x == 1 || x == bd_src.Width - 2)
                    {
                        offsetX[0, 0] = 0;
                        offsetX[0, 1] = 0;
                        offsetX[0, 2] = 0;
                        offsetX[2, 0] = 0;
                        offsetX[2, 1] = 0;
                        offsetX[2, 2] = 0;
                    }

                    sumX = 128;
                    for (kx = -1; kx <= 1; kx++)
                        for (ky = -1; ky <= 1; ky++)
                            sumX += kernelX[kx + 1, ky + 1] * (((p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->r + (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->g + (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->b) / 3);

                    sumY = 128;
                    for (kx = -1; kx <= 1; kx++)
                        for (ky = -1; ky <= 1; ky++)
                            sumY += kernelY[kx + 1, ky + 1] * (((p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->r + (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->g + (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->b) / 3);

                    // Assign clamped X sum to R channel
                    if (sumX < 0)
                        (p_dst + (y * dst_stride) + x)->r = 0;
                    else if (sumX > 255)
                        (p_dst + (y * dst_stride) + x)->r = 255;
                    else
                        (p_dst + (y * dst_stride) + x)->r = (Byte)sumX;

                    // Assign clamped Y sum to G channel
                    if (sumY < 0)
                        (p_dst + (y * dst_stride) + x)->g = 0;
                    else if (sumY > 255)
                        (p_dst + (y * dst_stride) + x)->g = 255;
                    else
                        (p_dst + (y * dst_stride) + x)->g = (Byte)sumY;

                    // Calculate and assign B channel data
                    sumZ = ((Math.Abs(sumX - 128) + Math.Abs(sumY - 128)) / 4);
                    if (sumZ < 0)
                        sumZ = 0;
                    if (sumZ > 64)
                        sumZ = 64;
                    (p_dst + (y * dst_stride) + x)->b = (Byte)(255 - (Byte)sumZ);
                }
            }

            m_bmp_image.UnlockBits(bd_src);
            m_bmp_nmap.UnlockBits(bd_dst);
        }

        unsafe private void BoxBlur(Bitmap bmpSrc, Bitmap bmpDst)
        {
            int x = 0, y = 0, kx = 0, ky = 0, src_stride = 0, dst_stride = 0;
            float sum = 0f;
            PixelData* p_src = null, p_dst = null;
            float[,] kernel = new float[,] { { 1f / 9f, 1f / 9f, 1f / 9f }, { 1f / 9f, 1f / 9f, 1f / 9f }, { 1f / 9f, 1f / 9f, 1f / 9f } };
            int[,] offsetX = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };
            int[,] offsetY = new int[,] { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } };

            System.Drawing.Imaging.BitmapData bd_src = bmpSrc.LockBits(new Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData bd_dst = bmpDst.LockBits(new Rectangle(0, 0, bmpDst.Width, bmpDst.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            p_src = (PixelData*)bd_src.Scan0.ToPointer();
            p_dst = (PixelData*)bd_dst.Scan0.ToPointer();

            src_stride = bd_src.Stride / 4;
            dst_stride = bd_dst.Stride / 4;

            for (y = 0; y < bd_src.Height; y++)
            {
                // Set Y offsets for image boundaries
                if (y == 0)
                {
                    offsetY[0, 0] = (bd_src.Height - 1) * src_stride;
                    offsetY[1, 0] = (bd_src.Height - 1) * src_stride;
                    offsetY[2, 0] = (bd_src.Height - 1) * src_stride;
                    offsetY[0, 2] = 0;
                    offsetY[1, 2] = 0;
                    offsetY[2, 2] = 0;
                }
                else if (y == bd_src.Height - 1)
                {
                    offsetY[0, 0] = 0;
                    offsetY[1, 0] = 0;
                    offsetY[2, 0] = 0;
                    offsetY[0, 2] = -((bd_src.Height - 1) * src_stride);
                    offsetY[1, 2] = -((bd_src.Height - 1) * src_stride);
                    offsetY[2, 2] = -((bd_src.Height - 1) * src_stride);
                }
                else if (y == 1 || y == bd_src.Height - 2)
                {
                    offsetY[0, 0] = 0;
                    offsetY[1, 0] = 0;
                    offsetY[2, 0] = 0;
                    offsetY[0, 2] = 0;
                    offsetY[1, 2] = 0;
                    offsetY[2, 2] = 0;
                }

                for (x = 0; x < bd_src.Width; x++)
                {
                    (p_dst + (y * dst_stride) + x)->a = 255;

                    // Set X offsets for image boundaries
                    if (x == 0)
                    {
                        offsetX[0, 0] = bd_src.Width - 1;
                        offsetX[0, 1] = bd_src.Width - 1;
                        offsetX[0, 2] = bd_src.Width - 1;
                        offsetX[2, 0] = 0;
                        offsetX[2, 1] = 0;
                        offsetX[2, 2] = 0;
                    }
                    else if (x == bd_src.Width - 1)
                    {
                        offsetX[0, 0] = 0;
                        offsetX[0, 1] = 0;
                        offsetX[0, 2] = 0;
                        offsetX[2, 0] = -bd_src.Width;
                        offsetX[2, 1] = -bd_src.Width;
                        offsetX[2, 2] = -bd_src.Width;
                    }
                    else if (x == 1 || x == bd_src.Width - 2)
                    {
                        offsetX[0, 0] = 0;
                        offsetX[0, 1] = 0;
                        offsetX[0, 2] = 0;
                        offsetX[2, 0] = 0;
                        offsetX[2, 1] = 0;
                        offsetX[2, 2] = 0;
                    }

                    // Calculate R sum
                    sum = 0f;
                    for (kx = -1; kx <= 1; kx++)
                        for (ky = -1; ky <= 1; ky++)
                            sum += kernel[kx + 1, ky + 1] * (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->r;
                    if (sum < 0)
                        sum = 0;
                    if (sum > 255)
                        sum = 255;
                    (p_dst + (y * dst_stride) + x)->r = (Byte)sum;

                    // Calculate G sum
                    sum = 0f;
                    for (kx = -1; kx <= 1; kx++)
                        for (ky = -1; ky <= 1; ky++)
                            sum += kernel[kx + 1, ky + 1] * (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->g;
                    if (sum < 0)
                        sum = 0;
                    if (sum > 255)
                        sum = 255;
                    (p_dst + (y * dst_stride) + x)->g = (Byte)sum;

                    // Calculate B sum
                    sum = 0f;
                    for (kx = -1; kx <= 1; kx++)
                        for (ky = -1; ky <= 1; ky++)
                            sum += kernel[kx + 1, ky + 1] * (p_src + (y * src_stride) + x + (ky * src_stride) + kx + offsetX[kx + 1, ky + 1] + offsetY[kx + 1, ky + 1])->b;
                    if (sum < 0)
                        sum = 0;
                    if (sum > 255)
                        sum = 255;
                    (p_dst + (y * dst_stride) + x)->b = (Byte)sum;
                }
            }

            bmpSrc.UnlockBits(bd_src);
            bmpDst.UnlockBits(bd_dst);
        }

        public void LoadPic(Image File)
        {
            try
            {
                m_bmp_image = (Bitmap)File;
                m_bmp_nmap = new Bitmap(m_bmp_image.Width, m_bmp_image.Height);
                m_bmp_nmap_blur = new Bitmap(m_bmp_image.Width, m_bmp_image.Height);
                m_bmp_channel = new Bitmap(m_bmp_image.Width, m_bmp_image.Height);

                GenerateNormalMap();
                BoxBlur(m_bmp_nmap, m_bmp_nmap_blur);
                BoxBlur(m_bmp_nmap_blur, m_bmp_nmap);
                Genered = true;
            }
            catch { ;}
        }
        public Bitmap Map
        {
            get
            {
                if (Genered)
                {
                    return m_bmp_nmap;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}