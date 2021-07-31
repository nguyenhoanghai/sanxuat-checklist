using System;
using System.Collections.Generic;
namespace GPRO.Core
{
	public class Constant
	{
		public class Message
		{
			public const string UnAuthenticated = "Bạn chưa đăng nhập hoặc phiên làm việc của bạn đã kết thúc";
			public const string UnAuthorized = "Bạn không có quyền truy cập dữ liệu";
		}
		public class ModuleGlobal
		{
			public static readonly Constant.Module SM = new Constant.Module
			{
				ModuleId = Constant.eModule.Sm,
				Name = "GPRO.SM"
			};
			public static readonly Constant.Module Accounts = new Constant.Module
			{
				ModuleId = Constant.eModule.Accounts,
				Name = "GPRO.ACCOUNT"
			};
			public static IEnumerable<Constant.Module> GetEnumerable()
			{
				return new Constant.Module[]
				{
					Constant.ModuleGlobal.SM,
					Constant.ModuleGlobal.Accounts
				};
			}
		}
		public enum eModule
		{
			Sm,
			Accounts
		}
		public class Module
		{
			public Constant.eModule ModuleId
			{
				get;
				set;
			}
			public string Name
			{
				get;
				set;
			}
		}
		public const string APP_AUTHENTICATION = "APP_AUTHENTICATION";
		public const string APP_MENU_DISPLAY = "APP_MENU_DISPLAY";
		public const string APP_PERMISSION_FEATURE = "APP_PERMISSION_FEATURE";
		public const string SS_AUTHENTICATION = "SS_AUTHENTICATION";
		public const string GETENCRYPTOR = "GETENCRYPTOR";
		public const string IMEMBERSHIP_SERVICE = "IMEMBERSHIP_SERVICE";
		public const string UPDATEFILEEMPLOYEE = "HasTimeKeeping";
		public const string SS_LOGIN_COUNT = "SS_LOGIN_COUNT";
		public const string SS_REQUIRE_CHANGE_PASSWORD = "SS_REQUIRE_CHANGE_PASSWORD";
		public const string SS_LANGUAGE = "SS_LANGUAGE";
		public const string SS_DEFAULT_LANGUAGE = "vi-VN";
	}
}
