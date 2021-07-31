using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace GPRO.Core.Mvc.Attribute
{
	public class EmailValidationAttribute : ValidationAttribute
	{
		private bool _isAllowNull = true;
		public EmailValidationAttribute()
		{
		}
		public EmailValidationAttribute(bool isAllowNull = true)
		{
			this._isAllowNull = isAllowNull;
		}
		public override bool IsValid(object value)
		{
			bool result;
			if (!this._isAllowNull)
			{
				Regex regex = new Regex("^[a-zA-Z][\\w\\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\\w\\.-]*[a-zA-Z0-9]\\.[a-zA-Z][a-zA-Z\\.]*[a-zA-Z]$");
				result = regex.IsMatch(value.ToString());
			}
			else
			{
				result = true;
			}
			return result;
		}
	}
}
