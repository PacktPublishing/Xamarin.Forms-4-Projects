using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace GalleryApp
{
    public class FormsLocalStorage : ILocalStorage
    {
        public const string FavoritePhotosKey = "FavoritePhotos";

        public async Task<List<string>> Get()
        {
            if (Application.Current.Properties.ContainsKey(FavoritePhotosKey))
            {
                var filenames = (string)Application.Current.Properties[FavoritePhotosKey];

                return JsonConvert.DeserializeObject<List<string>>(filenames);
            }

            return new List<string>();
        }

        public async Task Store(string filename)
        {
            var filenames = await Get();

            filenames.Add(filename);

            var json = JsonConvert.SerializeObject(filenames);

            Application.Current.Properties[FavoritePhotosKey] = json;

            await Application.Current.SavePropertiesAsync();
        }
    }
}
