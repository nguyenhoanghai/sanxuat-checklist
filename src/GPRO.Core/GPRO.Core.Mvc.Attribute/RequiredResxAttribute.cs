using System;
using System.ComponentModel.DataAnnotations;
namespace GPRO.Core.Mvc.Attribute
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class RequiredResxAttribute : RequiredAttribute
	{
		public string ErrorMessageResourceNameResx
		{
			get;
			set;
		}
		public Type VNType
		{
			get;
			set;
		}
		public Type USType
		{
			get;
			set;
		}
		public override bool IsValid(object value)
		{
			base.ErrorMessage = string.Empty;
			ResxType resx = ResxManager.GetResx(ResxManager.CurrentLanguage);
			if (resx.UICulture == ResxManager.Vi.UICulture)
			{
				base.ErrorMessageResourceType = this.VNType;
			}
			else
			{
				base.ErrorMessageResourceType = this.USType;
			}
			base.ErrorMessageResourceName = this.ErrorMessageResourceNameResx;
			return base.IsValid(value);
		}
	}
}
