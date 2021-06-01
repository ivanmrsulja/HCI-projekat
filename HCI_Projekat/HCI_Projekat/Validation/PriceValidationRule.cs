using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HCI_Projekat.Validation
{
    public class PriceValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var cena = value as string;
                try
                {
                    double a = Double.Parse(cena);
                    if(a < 0)
                    {
                        return new ValidationResult(false, "Cena mora biti pozitivan broj.");
                    }
                }
                catch
                {
                    return new ValidationResult(false, "Cena mora biti broj.");
                }
                return new ValidationResult(true, null);
            }
            catch
            {
                return new ValidationResult(false, "Unknown error occured.");
            }
        }
    }
}
