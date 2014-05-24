using Split_It_.Model;
using Split_It_.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Split_It_.Converter
{
    /// <summary>
/// Caches the image that gets downloaded as part of Image control Source property.
/// </summary>
    public class CacheImageFileConverter : IValueConverter
    {
        public static string DEFAULT_PROFILE_IMAGE_URL = @"https://dx0qysuen8cbs.cloudfront.net/assets/fat_rabbit/avatars/100-5eb999e2b4b24b823a9d82c29d42e9b2.png";
        private IsolatedStorageFile _storage;
        private const string imageStorageFolder = "TempImages";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                _storage = IsolatedStorageFile.GetUserStoreForApplication();
            }
            Picture picture = value as Picture;
            
            if (picture == null || picture.medium.Equals(DEFAULT_PROFILE_IMAGE_URL))
            {
                return LoadDefaultIfPassed(null, (parameter ?? string.Empty).ToString());
            }


            string path = picture.medium;
            Uri imageFileUri = new Uri(path);
            
            if (imageFileUri.Scheme == "http" || imageFileUri.Scheme == "https")
            {
                if (!Util.checkNetworkConnection())
                {
                    if (_storage.FileExists(GetFileNameInIsolatedStorage(imageFileUri)))
                    {
                        return ExtractFromLocalStorage(imageFileUri);
                    }
                    else
                    {
                        return LoadDefaultIfPassed(imageFileUri, (parameter ?? string.Empty).ToString());
                    }
                }
                else
                {
                    return DownloadFromWeb(imageFileUri);
                }
            }
            else
            {
                BitmapImage bm = new BitmapImage(imageFileUri);
                return bm;
            }
        }

        private static object LoadDefaultIfPassed(Uri imageFileUri, string defaultImagePath)
        {
            string defaultImageUri = (defaultImagePath ?? string.Empty).ToString();
            if (!string.IsNullOrEmpty(defaultImageUri))
            {
                BitmapImage bm = new BitmapImage(new Uri(defaultImageUri, UriKind.Relative)); //Load default Image
                return bm;
            }
            else
            {
                BitmapImage bm = new BitmapImage(imageFileUri);
                return bm;
            }
        }

        private object DownloadFromWeb(Uri imageFileUri)
        {
            WebClient m_webClient = new WebClient(); //Load from internet
            BitmapImage bm = new BitmapImage();

            m_webClient.OpenReadCompleted += (o, e) =>
            {
                if (e.Error != null || e.Cancelled) return;
                WriteToIsolatedStorage(IsolatedStorageFile.GetUserStoreForApplication(), e.Result, GetFileNameInIsolatedStorage(imageFileUri));
                bm.SetSource(e.Result);
                e.Result.Close();
            };
            m_webClient.OpenReadAsync(imageFileUri);
            return bm;
        }

        private object ExtractFromLocalStorage(Uri imageFileUri)
        {
            string isolatedStoragePath = GetFileNameInIsolatedStorage(imageFileUri); //Load from local storage
            using (var sourceFile = _storage.OpenFile(isolatedStoragePath, FileMode.Open, FileAccess.Read))
            {
                BitmapImage bm = new BitmapImage();
                bm.SetSource(sourceFile);
                return bm;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private void WriteToIsolatedStorage(IsolatedStorageFile storage, System.IO.Stream inputStream, string fileName)
        {
            IsolatedStorageFileStream outputStream = null;
            try
            {
                if (!storage.DirectoryExists(imageStorageFolder))
                {
                    storage.CreateDirectory(imageStorageFolder);
                }
                if (storage.FileExists(fileName))
                {
                    storage.DeleteFile(fileName);
                }
                outputStream = storage.CreateFile(fileName);
                byte[] buffer = new byte[32768];
                int read;
                while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    outputStream.Write(buffer, 0, read);
                }
                outputStream.Close();
            }
            catch
            {
                //We cannot do anything here.
                if (outputStream != null) outputStream.Close();
            }
        }

        /// <summary>
        /// Gets the file name in isolated storage for the Uri specified. This name should be used to search in the isolated storage.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public string GetFileNameInIsolatedStorage(Uri uri)
        {
            return imageStorageFolder + "\\" + uri.AbsoluteUri.GetHashCode() + ".img";
        }
    }
}
