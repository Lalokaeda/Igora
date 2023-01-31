using Bytescout.BarCode;
using Bytescout.BarCodeReader;
using Igora.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using SymbologyType = Bytescout.BarCode.SymbologyType;

namespace Igora
{
    public static class BarcodeReader
    {
        private static string _fileName;
        public static void Read() 
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Supported formats (*.bmp;*.gif;*.tif;*.png;*.jpg;*.pdf)|*.bmp;*.gif;*.tif;*.tiff;*.png;*.jpg;*.jpeg;*.pdf|All Files|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                _fileName = dlg.FileName;

                try
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(dlg.FileName, UriKind.Absolute);
                    bitmapImage.EndInit();
                }
                catch (Exception)
                {
                }
            }
        }
        public static List<BarcodeViewModel> Decode()
        {
            Reader reader = new Reader();
            reader.RegistrationName = "demo";
            reader.RegistrationKey = "demo";

            // Specify barcode types to find
            reader.BarcodeTypesToFind.EAN13 = true;
            // Select specific barcode types to speed up the decoding process and avoid false positives.

            // Show wait cursor
            Mouse.OverrideCursor = Cursors.Wait;

            /* -----------------------------------------------------------------------
            NOTE: We can read barcodes from specific page to increase performance.
            For sample please refer to "Decoding barcodes from PDF by pages" program.
            ----------------------------------------------------------------------- */

            try
            {
                // Search for barcodes
                reader.ReadFrom(_fileName);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }

            // Display found barcode inforamtion:
           List<BarcodeViewModel>barcodes=new List<BarcodeViewModel>();

            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < reader.FoundBarcodes.Length; i++)
            {
                FoundBarcode barcode = reader.FoundBarcodes[i];
                stringBuilder.AppendFormat("{0}) Type: {1}; Value: {2}.\r\n", i + 1, barcode.Type, barcode.Value);
                barcodes.Add(new BarcodeViewModel()
                { 
                    Symbology = SymbologyType.EAN13,
                    Value= barcode.Value
                });
            }
            reader.Dispose();
            return barcodes;
        }
 
    }
}
