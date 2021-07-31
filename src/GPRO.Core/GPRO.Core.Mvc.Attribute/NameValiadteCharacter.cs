using System;
using System.ComponentModel.DataAnnotations;
namespace GPRO.Core.Mvc.Attribute
{
	public class NameValiadteCharacter : ValidationAttribute
	{
		private bool _isAllowNull = true;
		public NameValiadteCharacter()
		{
		}
		public NameValiadteCharacter(bool isAllowNull = true)
		{
			this._isAllowNull = isAllowNull;
		}
		public override bool IsValid(object value)
		{
			string text = "!@#$%^&*()+=-[]\\';,./{}|\":<>?";
			char[] array = text.ToCharArray();
			bool flag;
			bool result;
			if (value != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					string value2 = array[i].ToString();
					if (value.ToString().IndexOf(value2) != -1)
					{
						flag = false;
						result = flag;
						return result;
					}
				}
			}
			flag = true;
			result = flag;
			return result;
		}
	}
}
