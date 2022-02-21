using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryProjectUWP.ViewModels.General
{
    public class WorkerState
    {
        public string State { get; set; }
        public object Model { get; set; }
    }

    public class WorkerState<T> where T : struct
    {
        public string State { get; set; }
        public long Id { get; set; }
        public object Model { get; set; }
        public T Result { get; set; }
        public IEnumerable<T> ResultList { get; set; }
    }

    public class WorkerState<T, U> where T : U
    {
        public string State { get; set; }
        public long Id { get; set; }
        public object Model { get; set; }
        public T Result { get; set; }
        public IEnumerable<T> ResultList { get; set; }
    }
}
