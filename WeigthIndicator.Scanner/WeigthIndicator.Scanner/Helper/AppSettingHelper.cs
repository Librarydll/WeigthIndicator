using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace WeigthIndicator.Scanner.Helper
{
    public static class AppSettingHelper
    {
        static string pathToFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "appsetting.txt");

        public static async Task SaveApplicationProperty(string server)
        {
            using (FileStream fs = new FileStream(pathToFile, FileMode.OpenOrCreate, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs))
            {
               await  sw.WriteAsync(server);
            }
        }

        public static async Task<string> LoadApplicationPropertyAsync()
        {
            using (FileStream fs = new FileStream(pathToFile, FileMode.OpenOrCreate, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            {
                return await sr.ReadLineAsync();
            }
        }
        public static string LoadApplicationProperty()
        {
            try
            {
                using (FileStream fs = new FileStream(pathToFile, FileMode.OpenOrCreate, FileAccess.Read))
                using (StreamReader sr = new StreamReader(fs))
                {
                    return sr.ReadLine();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
