using UserMS.Core.DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UserMS.Logic.ServiceLayer.Helpers
{
    public class Validator
    {
        public static bool IsValid(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            if (input.Length >= 20)
            {
                return false;
            }

            var regex = new Regex(@"^[a-zA-Z]+$");

            return regex.IsMatch(input);
        }
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }
            if (email.Length >= 30)
            {
                return false;
            }
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            return emailRegex.IsMatch(email);
        }
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return false;
            }

            var passwordRegex = new Regex("^(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$");
            if (password.Length >= 20)
            {
                return false;
            }

            return passwordRegex.IsMatch(password);
        }
        public static bool IsValidUser(User user)
        {
            if (user == null)
            {
                return false;
            }
            if (Validator.IsValid(user.FirstName) && Validator.IsValid(user.LastName) && Validator.IsValid(user.Country) && Validator.IsValidEmail(user.Email) && Validator.IsValidPassword(user.Password))
            {
                return true;
            }
            return false;

        }
    }
}
