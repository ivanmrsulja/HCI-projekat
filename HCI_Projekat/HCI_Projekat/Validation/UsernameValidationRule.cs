using HCI_Projekat.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace HCI_Projekat.Validation
{
    class UsernameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                var username = value as string;
                using (var db = new Context())
                {
                    string[] usernames = (from users in db.Korisnici where users.Username == username select users.Username).ToArray();
                    if (usernames.Length == 0 || username.Trim() == "")
                    {
                        return new ValidationResult(true, null);
                    }
                    else
                    {
                        return new ValidationResult(false, "Username je vec u upotrebi.");
                    }
                }

            }
            catch
            {
                return new ValidationResult(false, "Unknown error occured.");
            }
        }
    }
}
