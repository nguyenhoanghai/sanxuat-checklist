using GPRO.Core.Mvc;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
namespace GPRO.Core.Generic
{
	public class JsonDataResult
	{
		public string Result
		{
			get;
			set;
		} 
		public dynamic Records
		{
			 get;
			 set;
		}
		 
		public dynamic Data
		{
			 get;
			 set;
		}
		public long TotalRecordCount
		{
			get;
			set;
		}
		public List<Error> ErrorMessages
		{
			get;
			set;
		}
		public int StatusCode
		{
			get;
			set;
		}
		public string Message
		{
			get;
			set;
		}
		public JsonDataResult()
		{
			this.ErrorMessages = new List<Error>();
			this.StatusCode = 200;
		}
	}
}
