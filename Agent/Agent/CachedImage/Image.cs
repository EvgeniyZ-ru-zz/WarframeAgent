using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Cache;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Agent.CachedImage
{

    //Source: https://github.com/floydpink/CachedImage

    public class Image : System.Windows.Controls.Image
    {
        static bool isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
        static Image()
        {
            var dd = new Uri(@"pack://application:,,,/"
                             + Assembly.GetExecutingAssembly().GetName().Name
                             + ";component/"
                             + "Icons/Freq.png", UriKind.Absolute); //pack://application:,,,/Agent;component/Resources/Images/NoImg.jpg

            //Debugger.Break();

            DefaultStyleKeyProperty.OverrideMetadata(typeof(Image),
                new FrameworkPropertyMetadata(typeof(Image)));

            if (isInDesignMode) // не выполнять в дизайнере
                return;
            var files = Directory.GetFiles(FileCache.AppCacheDirectory);
            foreach (var file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.LastAccessTime < DateTime.Now.AddDays(-5))
                    fi.Delete();
            }
        }

        public string ImageUrl
        {
            get => (string)GetValue(ImageUrlProperty);
            set => SetValue(ImageUrlProperty, value);
        }

        public BitmapCreateOptions CreateOptions
        {
            get => (BitmapCreateOptions)GetValue(CreateOptionsProperty);
            set => SetValue(CreateOptionsProperty, value);
        }

        private static async void ImageUrlPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var url = e.NewValue as string;

            if (isInDesignMode) return;

            var cachedImage = (Image)obj;
            var bitmapImage = new BitmapImage();
            string path = null;

            if (!string.IsNullOrEmpty(url))
            {
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
                            var (file, stream) = await FileCache.HitAsync(url);
                            if (stream == null)
                                return;

                            path = file;
                            bitmapImage.BeginInit();
                            bitmapImage.CreateOptions = cachedImage.CreateOptions;
                            bitmapImage.StreamSource = stream;
                            bitmapImage.EndInit();
                            cachedImage.Source = bitmapImage;
                        }
                        catch (Exception)
                        {
                            if (File.Exists(path) && path != null)
                                File.Delete(path);

                            bitmapImage = new BitmapImage();

                            var (_, stream) = await FileCache.HitAsync(url);
                            if (stream == null)
                                return;

                            bitmapImage.BeginInit();
                            bitmapImage.CreateOptions = cachedImage.CreateOptions;
                            bitmapImage.StreamSource = stream;
                            bitmapImage.EndInit();
                            cachedImage.Source = bitmapImage;
                        }

                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                var img = new BitmapImage(new Uri(@"pack://application:,,,/"
                                                  + Assembly.GetExecutingAssembly().GetName().Name
                                                  + ";component/"
                                                  + "Resources/Images/NoImg.jpg", UriKind.Absolute));
                cachedImage.Source = img;
            }
        }

        public static readonly DependencyProperty ImageUrlProperty = DependencyProperty.Register("ImageUrl",
            typeof(string), typeof(Image), new PropertyMetadata("", ImageUrlPropertyChanged));

        public static readonly DependencyProperty CreateOptionsProperty = DependencyProperty.Register("CreateOptions",
            typeof(BitmapCreateOptions), typeof(Image));
    }
}
