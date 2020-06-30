using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdStore.WebApp.Tests.Config
{
    public static class TestExtensions
    {
        public static decimal ApenasNumeros(this string value)
        {
            return Convert.ToDecimal(new string(value.Where(char.IsDigit).ToArray()));
        }
    }
}
