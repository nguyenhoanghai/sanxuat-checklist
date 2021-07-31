using System;
using System.Collections.Generic;
namespace GPRO.Core.Interface
{
	public interface IMenuCategory
	{
		string Category
		{
			get;
			set;
		}
		string Position
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
		string Icon
		{
			get;
			set;
		}
		bool IsViewIcon
		{
			get;
			set;
		}
		string Link
		{
			get;
			set;
		}
		List<IMenu> ListMenu
		{
			get;
			set;
		}
		int ModuleId
		{
			get;
			set;
		}
		string ModuleName
		{
			get;
			set;
		}
	}
}
