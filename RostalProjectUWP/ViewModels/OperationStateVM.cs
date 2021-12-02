using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RostalProjectUWP.ViewModels
{
    public class OperationStateVM
    {
        public long Id { get; set; }
        public Guid Guid { get; set; }
        public bool IsSuccess { get; set; }
        public string Title { get; set; }
        public string Glyph { get; set; } = "\uE9CE"; // UnKnow
        public string Message { get; set; }
        public object Result { get; set; }
    }

    public class GlobalOperationStateVM
    {
        public IEnumerable<OperationStateVM> OperationsState { get; set; }
        public bool IsGlobalSuccess { get; set; }

    }
}
