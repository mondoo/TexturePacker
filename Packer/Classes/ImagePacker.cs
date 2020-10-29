using System.IO;
using System.Windows.Media.Imaging;
using ImageMagick;
using Image = System.Windows.Controls.Image;
using System.Collections.Generic;
using System;

namespace Packer.Classes
{
    class ImagePacker
    {
        private MagickImage workingImage;

        private int width = 1024;
        private int height = 1024;
        private int padding = 10;

        private List<string> sourceImagePaths = new List<string>();

        public void SetSize(int size)
        {
            width = size;
            height = size;
        }

        public void SetPadding(int inPadding)
        {
            padding = inPadding;
        }

        public void AddImagePath(string path)
        {
            sourceImagePaths.Add(path);
        }

        private int CalculateDivisions()
        {
            double sourceImagePathsCount = (double)sourceImagePaths.Count / (double)3;
            sourceImagePathsCount = Math.Ceiling(sourceImagePathsCount);

            int divisor = 1;
            if (sourceImagePathsCount > 1)
            {
                int powerOfTwo = (int)sourceImagePathsCount - 1;
                while ((int)(powerOfTwo & powerOfTwo - 1) != 0)
                {
                    powerOfTwo = (int)(powerOfTwo & powerOfTwo - 1);
                }
                divisor = powerOfTwo << 1;
                if (divisor > 2)
                {
                    divisor /= 2;
                }
            }

            return divisor;
        }

        private void CreateBackground()
        {
            workingImage = new MagickImage(new MagickColor("#000000"), width, height);
            workingImage.Quality = 100;
        }

        public void Reset()
        {
            CreateBackground();
            sourceImagePaths.Clear();
        }

        public void CreateImage()
        {
            CreateBackground();

            int divisions = CalculateDivisions();
            int i = 0;
            int widthSize = width / divisions;
            int heightSize = height / divisions;

            int x = 0;
            int y = 0;

            sourceImagePaths.ForEach(imagePath =>
            {
                if (i >= 3)
                {
                    x++;
                    i = 0;
                }

                if (x >= divisions)
                {
                    x = 0;
                    y++;
                }

                Channels channel;
                switch (i)
                {
                    case 0:
                        channel = Channels.Red;
                        break;
                    case 1:
                        channel = Channels.Green;
                        break;
                    case 2:
                        channel = Channels.Blue;
                        break;
                    default:
                        channel = Channels.Red;
                        break;
                }

                ApplyImage(widthSize, heightSize, channel, imagePath, x, y);

                i++;
            });
        }

        public void PreviewOutput(Image imageRef)
        {
            CreateImage();

            imageRef.Source = MagickImageToBitmapImage();
        }

        private BitmapImage MagickImageToBitmapImage()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapImage bitmapImage = new BitmapImage();

                workingImage.Format = MagickFormat.Png;
                workingImage.Write(stream);

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = stream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }

        private void ApplyImage(int resizeWidth, int resizeHeight, Channels channel, string path, int x, int y)
        {
            int imgPadding = padding / 2;

            MagickImage imageToApply = new MagickImage(path);
            imageToApply.Transparent(new MagickColor("#000000"));
            imageToApply.FilterType = FilterType.Quadratic;
            imageToApply.Quality = 100;
            imageToApply.Format = MagickFormat.Png;
            imageToApply.ColorAlpha(new MagickColor("#000000"));
            MagickGeometry size = new MagickGeometry(resizeWidth - padding, resizeHeight - padding);
            size.IgnoreAspectRatio = true;
            imageToApply.Resize(size);

            workingImage.CopyPixels(imageToApply, size, (x * resizeWidth) + imgPadding, (y * resizeHeight) + imgPadding, channel);
        }

        public void SaveFile(string filePath)
        {
            if (sourceImagePaths.Count > 0)
            {
                CreateImage();
                workingImage.Write(filePath);
            }
        }
    }
}
