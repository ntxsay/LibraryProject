
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LibraryProjectUWP;
using LibraryProjectUWP.Code.Services.Db;
using LibraryProjectUWP.ViewModels;

namespace UTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestCreateLibrary()
        {
            var result =  DbServices.Library.CreateAsync(new BibliothequeVM()
            {
                Name = $"TestCreateLibrary_{DateTime.Now}",
                Description = $"Test description _{DateTime.Now}"
            });

            var r = result.GetAwaiter().GetResult();
            Assert.IsTrue(r.IsSuccess, r.Message);
        }
    }
}
