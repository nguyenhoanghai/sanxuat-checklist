using GPRO.Core.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanXuatCheckList.Business.Model
{
   public class ResponseBase
    {
        public bool IsSuccess { get; set; }
        public List<Error> Errors { get; set; }
        public dynamic Data { get; set; }
        public dynamic Records { get; set; }
        public ResponseBase()
        {
            Errors = new List<Error>();
        }
    }
}
