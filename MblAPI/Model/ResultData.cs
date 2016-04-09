using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MblAPI.Model
{
    [Serializable]
    public class ResultData
    {
        public bool IsOK { get; set; }

        public string ErrMsg { get; set; }

        public object Data { get; set; }
    }
}