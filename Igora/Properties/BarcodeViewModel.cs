using Bytescout.BarCode;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Igora.Properties
{
    public class BarcodeViewModel : ObservableObject
    {
        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                if (value.Length > 12)
                    _value = value.Substring(0, 12);
                else _value = value;
            }
        }
        private SymbologyType _symbology;
        public SymbologyType Symbology
        {
            get { return _symbology; }
            set { _symbology = value; }
        }
        public BarcodeViewModel()
        {
            Value = "012345678900";
            Symbology = SymbologyType.EAN13;
        }
    }
}
