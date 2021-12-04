using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RostalProjectUWP.Code.Services.ES
{
    public class EsOperationState
    {
        public bool IsSuccess { get; set; }
        public StorageFile File { get; set; }
        public StorageFolder Folder { get; set; }
        public string Message { get; set; }
    }
}
