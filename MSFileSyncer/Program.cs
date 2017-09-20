using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSFileSyncer
{
    class Program
    {
        private const string relativeSyncFilePath = ".msfilesync\\order.sync";

        static void Main(string[] args)
        {
            //Laufwerksbuchstabe sollte als erstes Argument übergeben werden
            if (args.Length < 1) throw new ArgumentOutOfRangeException("drive", "No drive letter supplied.");
            var driveLetter = args[0];

            var syncFilePath = Path.Combine(driveLetter, relativeSyncFilePath);
            if (!File.Exists(syncFilePath)) return;
            var syncFileContent = File.ReadAllText(syncFilePath);
            
        }
    }
}
