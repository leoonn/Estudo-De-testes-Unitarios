using System;
using System.Configuration;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyClasses;

namespace MyClassesTest
{
    [TestClass]
    public class FileProcessTest
    {
        private const string Bad_fileName = @"C:\Badfilename.bat";
        private string goodFileName;

        public TestContext TestContext { get; set; }
        #region Test initialize and cleanup
        [TestInitialize]
           public void TestInitialize()
        {
            if (TestContext.TestName  == "FileNameDoesExists")
            {
                SetGoodFileName();
                if (!string.IsNullOrEmpty(goodFileName))
            {
                
                TestContext.WriteLine($"Creating file {goodFileName}");
                File.AppendAllText(goodFileName, "Some Text");
            }
            }

        }
        [TestCleanup]
        public void TestCleanUp()
        {
            if (TestContext.TestName == "FileNameDoesExists")
            {
                if (!string.IsNullOrEmpty(goodFileName))
                {
                    TestContext.WriteLine($"Deleting File  {goodFileName}");
                    File.Delete(goodFileName);
                }
            }
        }

        #endregion
        //Exemples of Decorators
        [TestMethod]
        [Description("Check if a file does Exists ")]
        [Owner("Leonardo")]
        [Priority(1)]
        [TestCategory("NoException")]
        public void FileNameDoesExists()
        {
            FileProcess fp = new FileProcess();
            bool fromCall;
           
            TestContext.WriteLine($"Testing File {goodFileName}");
            fromCall = fp.FileExists(goodFileName);
            
            Assert.IsTrue(fromCall);
        }
        [TestMethod]
        [DataSource("System.Data.SqlClient", @"Data Source=DESKTOP-GG8D5OV\SQLEXPRESS;Initial Catalog=TesteUnitario;Integrated Security=True", 
            "FileProcessTest", DataAccessMethod.Sequential)]
        public void FileDoesExistsFromDb()
        {
            FileProcess fp = new FileProcess();
            string file;

            bool expectedvalue;
            bool causesException;
            bool fromCall;

            file =  TestContext.DataRow["FileName"].ToString();
            expectedvalue = Convert.ToBoolean(TestContext.DataRow["ExpectedValue"]);
            causesException = Convert.ToBoolean(TestContext.DataRow["CausesException"]);
            

            try
            {
                fromCall = fp.FileExists(file);
                Assert.AreEqual(expectedvalue, fromCall,
                    $"File:  {file} has failed. method FileDoesExistsFromDb");
            }
            catch (ArgumentException)
            {

                Assert.IsTrue(causesException);
            }

           
        }

        private const string FileName = @"FileToDeploy.txt";
        [TestMethod]
        [DeploymentItem(FileName)]
        public void FileNameDoesExistsUsingDeploymentItem()
        {
            FileProcess fp = new FileProcess();
            string filename;
            bool fromCall;

            filename = $@"{TestContext.DeploymentDirectory}\{FileName}";
            TestContext.WriteLine($"Checking File {filename}");
            fromCall = fp.FileExists(filename);

            Assert.IsTrue(fromCall);
        }

        public void SetGoodFileName()
        {
            goodFileName = ConfigurationManager.AppSettings["GoodFileName"];
            if(goodFileName.Contains("[AppPath]"))
            {
                goodFileName = goodFileName.Replace("[AppPath]",
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }
        }
        [TestMethod] //Exemple of a test using timeOut
        [Timeout(3100)]
        public void SimulateTimeOut()
        {
            System.Threading.Thread.Sleep(3000);
        }

        [TestMethod]
        [Description("Check if a file doesn't Exists ")]
        [Owner("Leonardo")]
        public void FileNameDoesNotExists()
        {
            FileProcess fp = new FileProcess();
            bool fromCall; //ARRANGE
            fromCall = fp.FileExists(Bad_fileName); //ACT
            Assert.IsFalse(fromCall); //ASSERT
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [Description("Check if a file will throw a exception ")]
        [Owner("Leonardo")]
        
        public void FileNameNullOrEmpty_throwsArgumentException()
        {
            FileProcess fp = new FileProcess();
            fp.FileExists("");
        }

        [TestMethod]
        [Description("Check if a file will throw a exception ")]
        [Owner("Leonardo")]
        public void FileNameNullOrEmpty_throwsArgumentException_UsingTry_Catch()
        {
            FileProcess fp = new FileProcess();

            try
            {
                fp.FileExists("");
            }
            catch (ArgumentException)
            {
                //test has been a success
                return;
            }
            

            Assert.Fail("Fail Exception");
        }

    }
}
