using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WinHue3.Validations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class MinimumCountAttribute : ValidationAttribute
    {
        private readonly int _min;

        public MinimumCountAttribute(int min)
        {
            _min = min;
        }

        public int Minimum => _min;

        public override bool IsValid(object dic)
        {
            var property = typeof(ICollection).GetProperty("Count");
            var count = (int) property.GetValue(dic, null);
            return count >= _min;
        }
        
    }
}
