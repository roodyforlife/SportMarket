using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.ViewModels
{
    public class ConfigureTwoFactorAuthenticatorModel
    {
        public string InputCode { get; set; }
        public string SetupCode { get; set; }
        public string BarcodeImageUrl { get; set; }
    }
}
