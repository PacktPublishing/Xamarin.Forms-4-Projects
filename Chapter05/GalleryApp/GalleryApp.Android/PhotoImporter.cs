using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Android;
using Android.Content.PM;
using Android.Provider;
using GalleryApp.Models;

namespace GalleryApp.Droid
{
    public class PhotoImporter : IPhotoImporter
    {
        private bool hasCheckedPermission;
        private string[] result;

        public async Task<ObservableCollection<Photo>> Get(int start, int count, Quality quality = Quality.Low)
        {
            if (result == null)
            {
                var succeded = await Import();

                if (!succeded)
                {
                    return new ObservableCollection<Photo>();
                }
            }

            if (result.Length == 0)
            {
                return new ObservableCollection<Photo>();
            }

            Index startIndex = start;
            Index endIndex = start + count;

            if (endIndex.Value >= result.Length)
            {
                endIndex = result.Length - 1;
            }

            if (startIndex.Value > endIndex.Value)
            {
                return new ObservableCollection<Photo>();
            }

            var photos = new ObservableCollection<Photo>();

            foreach (var path in result[startIndex..endIndex])
            {
                var filename = Path.GetFileName(path);

                var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

                var memoryStream = new MemoryStream();

                stream.CopyTo(memoryStream);

                var photo = new Photo()
                {
                    Bytes = memoryStream.ToArray(),
                    Filename = filename
                };

                photos.Add(photo);
            }

            return photos;
        }

        public async Task<ObservableCollection<Photo>> Get(List<string> filenames, Quality quality = Quality.Low)
        {
            if (result == null)
            {
                var succeded = await Import();

                if (!succeded)
                {
                    return new ObservableCollection<Photo>();
                }
            }

            if (result.Length == 0)
            {
                return new ObservableCollection<Photo>();
            }

            var photos = new ObservableCollection<Photo>();

            foreach (var path in result)
            {
                var filename = Path.GetFileName(path);

                if (!filenames.Contains(filename))
                {
                    continue;
                }

                var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

                var memoryStream = new MemoryStream();

                stream.CopyTo(memoryStream);

                var photo = new Photo()
                {
                    Bytes = memoryStream.ToArray(),
                    Filename = filename
                };

                photos.Add(photo);
            }

            return photos;
        }

        private async Task<bool> Import()
        {
            string[] permissions = { Manifest.Permission.ReadExternalStorage };

            if (MainActivity.Current.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted)
            {
                ContinueWithPermission(true);

                return true;
            }

            MainActivity.Current.RequestPermissions(permissions, 33);

            while (hasCheckedPermission)
            {
                await Task.Delay(100);
            }

            return MainActivity.Current.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted;
        }

        public bool ContinueWithPermission(bool granted)
        {
            if (!granted)
            {
                return false;
            }

            Android.Net.Uri imageUri = MediaStore.Images.Media.ExternalContentUri;
            var cursor = MainActivity.Current.ContentResolver.Query(imageUri, null, MediaStore.Images.ImageColumns.MimeType + "=? or " + MediaStore.Images.ImageColumns.MimeType + "=?", new string[] { "image/jpeg", "image/png" }, MediaStore.Images.ImageColumns.DateModified);

            var paths = new List<string>();

            while (cursor.MoveToNext())
            {
                string path = cursor.GetString(cursor.GetColumnIndex(MediaStore.Images.ImageColumns.Data));

                paths.Add(path);
            }

            result = paths.ToArray();

            hasCheckedPermission = true;

            return true;
        }
    }
}
