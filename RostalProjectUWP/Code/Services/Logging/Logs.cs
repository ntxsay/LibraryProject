﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RostalProjectUWP.Code.Services.Logging
{
    internal class Logs
    {
        public static string GetLog(Exception exception, MethodBase method)
        {
            try
            {
                return $"{method.ReflectedType.Name}.{method.Name} : {exception.Message}{(exception.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + exception.InnerException?.Message) }";
            }
            catch (Exception ex)
            {
                MethodBase m = MethodBase.GetCurrentMethod();
                Debug.WriteLine($"{m.ReflectedType.Name}.{m.Name} : {ex.Message}{(ex.InnerException?.Message == null ? string.Empty : "\nInner Exception : " + ex.InnerException?.Message) }");
                return null;
            }
        }
    }
}