using System;
using System.Collections.Generic;
namespace GPRO.Core.Generic
{
	[Serializable]
	public class UploadConfig
	{
		public eUploadType Type
		{
			get;
			set;
		}
		public string UploadPath
		{
			get;
			set;
		}
		public string AllowExt
		{
			get;
			set;
		}
		public int MaxLength
		{
			get;
			set;
		}
		public string BindName
		{
			get;
			set;
		}
		public string Selector
		{
			get;
			set;
		}
		public int Limit
		{
			get;
			set;
		}
		public int Width
		{
			get;
			set;
		}
		public int Height
		{
			get;
			set;
		}
		public List<FileItem> Files
		{
			get;
			set;
		}
		public UploadConfig()
		{
			this.Limit = 1;
		}
		public UploadConfig(eUploadType type, string uploadPath = "", string allowExt = "", int maxLength = 0, string bindName = "ImagePath", string selector = "", int limit = 1, List<FileItem> files = null, int width = 260, int height = 180)
		{
			if (files == null)
			{
				files = new List<FileItem>();
			}
			this.Type = type;
			this.UploadPath = uploadPath;
			this.AllowExt = allowExt;
			this.MaxLength = maxLength;
			this.BindName = bindName;
			this.Selector = selector;
			this.Limit = limit;
			this.Files = files;
			this.Width = width;
			this.Height = height;
		}
	}
}
