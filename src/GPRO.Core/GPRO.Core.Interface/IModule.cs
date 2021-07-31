using System;
namespace GPRO.Core.Interface
{
	public interface IModule
	{
		string SystemName
		{
			get;
			set;
		}
		string ModuleName
		{
			get;
			set;
		}
		bool IsSystem
		{
			get;
			set;
		}
		int OrderIndex
		{
			get;
			set;
		}
		string Description
		{
			get;
			set;
		}
		string ModuleUrl
		{
			get;
			set;
		}
	}
}
