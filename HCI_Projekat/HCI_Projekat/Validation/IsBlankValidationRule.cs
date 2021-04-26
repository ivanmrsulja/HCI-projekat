using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HCI_Projekat.Validation
{
    class IsBlankValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var tekst = value as string;
                if (tekst.Trim() == "")
                {
                    return new ValidationResult(false, null);
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
