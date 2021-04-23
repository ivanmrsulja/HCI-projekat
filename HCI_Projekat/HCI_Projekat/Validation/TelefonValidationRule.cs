using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HCI_Projekat.Validation
{
    class TelefonValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var telefon = value as string;
                Regex regex = new Regex(@"[0][0-9]{9}");
                Match match = regex.Match(telefon);
                if (match.Success)
                    return new ValidationResult(true, null);
                else
                    return new ValidationResult(false, "Broj telefona nije u ispravnom formatu.");
            }
            catch
            {
                return new ValidationResult(false, "Unknown error occured.");
            }
        }
    }
}
