using System;
using System.IO;
using System.Net.Cache;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Agent.CachedImage
{

    //Source: https://github.com/floydpink/CachedImage

    public class Image : System.Windows.Controls.Image
    {
        static Image()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Image),
                new FrameworkPropertyMetadata(typeof(Image)));
        }

        public string ImageUrl
        {
            get { return (string)GetValue(ImageUrlProperty); }
            set { SetValue(ImageUrlProperty, value); }
        }

        public BitmapCreateOptions CreateOptions
        {
            get { return ((BitmapCreateOptions)(base.GetValue(Image.CreateOptionsProperty))); }
            set { base.SetValue(Image.CreateOptionsProperty, value); }
        }

        private static async void ImageUrlPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var url = e.NewValue as string;

            if (string.IsNullOrEmpty(url))
                return;

            var cachedImage = (Image)obj;
            var bitmapImage = new BitmapImage();
            string path = null;

            switch (FileCache.AppCacheMode)
            {
                case FileCache.CacheMode.WinINet:
                    bitmapImage.BeginInit();
                    bitmapImage.CreateOptions = cachedImage.CreateOptions;
                    bitmapImage.UriSource = new Uri(url);
                    // Enable IE-like cache policy.
                    bitmapImage.UriCachePolicy = new RequestCachePolicy(RequestCacheLevel.Default);
                    bitmapImage.EndInit();
                    cachedImage.Source = bitmapImage;
                    break;

                case FileCache.CacheMode.Dedicated:
                    try
                    {
                        var memoryStream = await FileCache.HitAsync(url);
                        if (memoryStream.stream == null)
                            return;

                        path = memoryStream.file;
                        bitmapImage.BeginInit();
                        bitmapImage.CreateOptions = cachedImage.CreateOptions;
                        bitmapImage.StreamSource = memoryStream.stream;
                        bitmapImage.EndInit();
                        cachedImage.Source = bitmapImage;
                    }
                    catch (Exception)
                    {
                        if (File.Exists(path) && path != null)
                            File.Delete(path);

                        bitmapImage = new BitmapImage();

                        var memoryStream = await FileCache.HitAsync(url);
                        if (memoryStream.stream == null)
                            return;

                        bitmapImage.BeginInit();
                        bitmapImage.CreateOptions = cachedImage.CreateOptions;
                        bitmapImage.StreamSource = memoryStream.stream;
                        bitmapImage.EndInit();
                        cachedImage.Source = bitmapImage;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static readonly DependencyProperty ImageUrlProperty = DependencyProperty.Register("ImageUrl",
            typeof(string), typeof(Image), new PropertyMetadata("", ImageUrlPropertyChanged));

        public static readonly DependencyProperty CreateOptionsProperty = DependencyProperty.Register("CreateOptions",
            typeof(BitmapCreateOptions), typeof(Image));
    }
}
