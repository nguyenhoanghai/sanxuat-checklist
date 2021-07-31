using System;
namespace GPRO.Core.Interface
{
	public interface IPermissionService
	{
		string PermissionId
		{
			get;
			set;
		}
		int FeatureId
		{
			get;
			set;
		}
		string FeatureName
		{
			get;
			set;
		}
		int PermissionTypeId
		{
			get;
			set;
		}
		string PermissionName
		{
			get;
			set;
		}
		string Url
		{
			get;
			set;
		}
	}
}
