using System;
namespace GPRO.Core.Interface
{
	public interface IMenu
	{
		string MenuName
		{
			get;
			set;
		}
		int OrderIndex
		{
			get;
			set;
		}
		string Link
		{
			get;
			set;
		}
		bool IsShow
		{
			get;
			set;
		}
		bool IsViewIcon
		{
			get;
			set;
		}
		string Icon
		{
			get;
			set;
		}
		string Description
		{
			get;
			set;
		}
	}
}
