using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CameraUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResultPage : Page
    {
        private readonly string SUBSCRIPTION_KEY = "c771c72794fc403e8e29e063db18c913";
        private EmotionServiceClient emotionClient;
        private StorageFile photo;

        public ResultPage()
        {
            this.InitializeComponent();
            emotionClient = new EmotionServiceClient(SUBSCRIPTION_KEY);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            photo = (StorageFile)e.Parameter;

            Frame rootFrame = Window.Current.Content as Frame;

            MyProgressRing.IsActive = true;
            MyProgressRing.Visibility = Visibility.Visible;
            PageContent.Visibility = Visibility.Collapsed;

            IRandomAccessStream accessStream = await photo.OpenAsync(FileAccessMode.Read);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(accessStream);
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();
            SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap,
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied);

            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

            Picture.Source = bitmapSource;

            var stream = await photo.OpenStreamForReadAsync();
            var result = await AnalyzeImage(stream);

            this.DataContext = (result.Any()) ? result.First().Scores : null;
            if (DataContext == null)
            {
                await new Windows.UI.Popups.MessageDialog("No pude detectar ninguna cara :(").ShowAsync();
                rootFrame.GoBack();
            }

            MyProgressRing.IsActive = false;
            MyProgressRing.Visibility = Visibility.Collapsed;
            PageContent.Visibility = Visibility.Visible;
        }

        public async Task<Emotion[]> AnalyzeImage(Stream imageStream)
        {
            try
            {
                return await emotionClient.RecognizeAsync(imageStream);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
