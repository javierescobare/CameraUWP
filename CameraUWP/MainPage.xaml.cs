using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.UI.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CameraUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        async void TakePic()
        {
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);

            StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                // User cancelled photo capture
                return;
            }

            MyProgressRing.IsActive = true;
            MyProgressRing.Visibility = Visibility.Visible;
            PageContent.Visibility = Visibility.Collapsed;

            var picturesLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            var destinationFolder = picturesLibrary.SaveFolder;

            var file = await photo.CopyAsync(
                destinationFolder, 
                "EmotionPhoto.jpg", 
                NameCollisionOption.GenerateUniqueName);
            await photo.DeleteAsync();

            PageContent.Visibility = Visibility.Visible;
            MyProgressRing.IsActive = false;
            MyProgressRing.Visibility = Visibility.Collapsed;

            Frame.Navigate(typeof(ResultPage), file);
        }

        private void PhotoButton_Click(object sender, RoutedEventArgs e)
        {
            TakePic();
        }
    }
}
