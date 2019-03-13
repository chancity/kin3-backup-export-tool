using System;
using System.Drawing;
using System.IO;
using Kin.Backup.Extensions;
using Kin.Stellar.Sdk;
using Newtonsoft.Json;

namespace backup_example
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Bitmap bitmap = GetBitmap();
                Console.Write("Enter passphrase: ");
                var passphrase = Console.ReadLine();
                var keyPair = bitmap.ToKeyPair(passphrase);

                Console.WriteLine(JsonConvert.SerializeObject(keyPair, Formatting.Indented));

                Console.Write("Backup file name: ");
                var backupPath = Console.ReadLine();

                passphrase = GetNewPassphrase(passphrase);



                var image = keyPair.ToQrCode(passphrase);
                image.Save(backupPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("Press any key to exit....");
            Console.ReadLine();
        }

        public static Bitmap GetBitmap()
        {
            Console.Write("Enter file path: ");
            var filePath = Console.ReadLine();

            try
            {
                return GetBitmapFromFile(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"There was a problem reading file at '{filePath}'.  {e.Message}");
                return GetBitmap();
            }
        }
        public static string GetNewPassphrase(string oldPassphrase)
        {
            Console.Write("Enter new passphrase (if blank will use old): ");
            var newPassphrase = Console.ReadLine();

            if (!string.IsNullOrEmpty(newPassphrase))
            {
                Console.Write("Confirm passphrase: ");
                var confirmPassphrase = Console.ReadLine();

                if (newPassphrase.Equals(confirmPassphrase))
                {
                    return newPassphrase;
                }
                
                Console.WriteLine("Passphrases do not match! Try again");

                return GetNewPassphrase(oldPassphrase);
            }

            Console.WriteLine("Passphrase was blank, using old passphrase");
            return oldPassphrase;
        }
        public static Bitmap GetBitmapFromFile(string filePath)
        {
            try
            {
                return new Bitmap(Image.FromFile(filePath));
            }
            catch (Exception)
            {
                throw new FileNotFoundException($"Resource not found: {filePath}");
            }
        }
    }

    class GetKeypairResults
    {
        public string passphrase { get; }
        public KeyPair KeyPair { get; }
        public GetKeypairResults(string passphrase, KeyPair keyPair)
        {
            this.passphrase = passphrase;
            KeyPair = keyPair;
        }
    }
}
