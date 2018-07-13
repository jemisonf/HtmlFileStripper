using HtmlStripper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace HtmlFileStripperTest
{
    [TestClass]
    public class HtmlFileStripperTest
    {
        private void writeStringToFile(string filename, string inputString, System.Text.Encoding encoding)
        {
            
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, false, encoding))
            {
                file.WriteLine(inputString);
            }
        }
        
        private void writeStringToFile(string filename, string inputString)
        {

            writeStringToFile(filename, inputString, System.Text.Encoding.UTF8);
        }

        [TestMethod]
        public void TestStripFile_Img()
        {
            string inputString = "<html><img></html>";
            string inputFileName =  $"{System.IO.Directory.GetCurrentDirectory()}\\testfile";
            string outputFileName = $"{System.IO.Directory.GetCurrentDirectory()}\\testfile_output";
            var tags = new List<string>
            {
                "img"
            };
            writeStringToFile(inputFileName, inputString);
            HtmlFileStripper stripper = new HtmlFileStripper(tags);
            stripper.StripFile(inputFileName, outputFileName, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(outputFileName));
            string outputString = File.ReadAllText(outputFileName);
            Assert.AreNotEqual(inputString, outputString);
            Assert.IsFalse(outputString.Contains("img"));
        }

        [TestMethod]
        public void TestStripFile_A()
        {
            string inputString = "<html><body>aaaaaa</body></html>";
            string inputFileName =  $"{System.IO.Directory.GetCurrentDirectory()}\\testfile";
            string outputFileName = $"{System.IO.Directory.GetCurrentDirectory()}\\testfile_output";
            var characters = new Dictionary<char, char>
            {
                { 'a', '\0' }
            };
            writeStringToFile(inputFileName, inputString);
            HtmlFileStripper stripper = new HtmlFileStripper(characters);
            stripper.StripFile(inputFileName, outputFileName, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(outputFileName));
            string outputString = File.ReadAllText(outputFileName);
            Assert.AreNotEqual(inputString, outputString);
            Assert.IsFalse(outputString.Contains("a"));
        }

        [TestMethod]
        public void TestStripFile_AImgLong()
        {
            string inputString = "<html><body><div><div><p>blahblahblah</p><p>no chars here</p></div><img><img></div><img><p>aaaaaAAA</p></body></html>";
            string inputFileName = $"{System.IO.Directory.GetCurrentDirectory()}\\testfile";
            string outputFileName = $"{System.IO.Directory.GetCurrentDirectory()}\\testfile_output";
            var characters = new Dictionary<char, char>
            {
                { 'a', '\0' }
            };
            var tags = new List<string>
            {
                "img"
            };
            writeStringToFile(inputFileName, inputString);
            var stripper = new HtmlFileStripper(characters, tags);
            stripper.StripFile(inputFileName, outputFileName, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(outputFileName));
            string outputString = File.ReadAllText(outputFileName);
            Assert.AreNotEqual(inputString, outputString);
            Assert.IsTrue(outputString.Trim() == "<html><body><div><div><p>blhblhblh</p><p>no chrs here</p></div></div><p>AAA</p></body></html>");
            Assert.IsFalse(outputString.Contains("a"));
            Assert.IsFalse(outputString.Contains("img"));
            Assert.IsTrue(outputString.Contains("html"));
            Assert.IsTrue(outputString.Contains("body"));
            Assert.IsTrue(outputString.Contains("div"));
            Assert.IsTrue(outputString.Contains("p"));
        }

        [TestMethod]
        public void TestStripFile_SameInputOutput()
        {
            string inputString = "<html><body><div><div><p>blahblahblah</p><p>no chars here</p></div><img><img></div><img><p>aaaaaAAA</p></body></html>";
            string inputFileName = $"{System.IO.Directory.GetCurrentDirectory()}\\testfile";
            string outputFileName = $"{System.IO.Directory.GetCurrentDirectory()}\\testfile";
            var characters = new Dictionary<char, char>
            {
                { 'a', '\0' }
            };
            var tags = new List<string>
            {
                "img"
            };
            writeStringToFile(inputFileName, inputString);
            var stripper = new HtmlFileStripper(characters, tags);
            stripper.StripFile(inputFileName, outputFileName, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(outputFileName));
            string outputString = File.ReadAllText(outputFileName);
            Assert.AreNotEqual(inputString, outputString);
            Assert.IsTrue(outputString.Trim() == "<html><body><div><div><p>blhblhblh</p><p>no chrs here</p></div></div><p>AAA</p></body></html>");
            Assert.IsFalse(outputString.Contains("a"));
            Assert.IsFalse(outputString.Contains("img"));
            Assert.IsTrue(outputString.Contains("html"));
            Assert.IsTrue(outputString.Contains("body"));
            Assert.IsTrue(outputString.Contains("div"));
            Assert.IsTrue(outputString.Contains("p"));
        }

        [TestMethod]
        public void TestStripFile_RunsWithoutCrashingWhenNoTagsPresent()
        {
            string inputString = "<html><br></html>";
            string inputFileName =  $"{System.IO.Directory.GetCurrentDirectory()}\\testfile";
            string outputFileName = $"{System.IO.Directory.GetCurrentDirectory()}\\testfile_output";
            var tags = new List<string>
            {
                "img"
            };
            writeStringToFile(inputFileName, inputString);
            HtmlFileStripper stripper = new HtmlFileStripper(tags);
            stripper.StripFile(inputFileName, outputFileName, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(outputFileName));
            string outputString = File.ReadAllText(outputFileName);
            Assert.AreNotEqual(inputString, outputString);
            Assert.IsFalse(outputString.Contains("img"));
        }

        [TestMethod]
        public void TestStripFile_UnicodeChars()
        {
            string inputString = "<html>\u00A0test\u00A0test\u00A0</html>";
            string inputFileName = $"{System.IO.Directory.GetCurrentDirectory()}\\testfile";
            string outputFileName = $"{System.IO.Directory.GetCurrentDirectory()}\\testfile_output";
            var characters = new Dictionary<char, char>
            {
                {  '\u00A0', ' ' },
                {  '\u2014', '-' },
            };
            writeStringToFile(inputFileName, inputString);
            var stripper = new HtmlFileStripper(characters);
            stripper.StripFile(inputFileName, outputFileName, System.Text.Encoding.UTF8);
            Assert.IsTrue(File.Exists(outputFileName));
            string outputString = File.ReadAllText(outputFileName);
            Assert.AreNotEqual(inputString, outputString);
            Assert.IsTrue(outputString.Trim() == "<html> test test </html>");
            Assert.IsFalse(outputString.Contains("\u00A0"));
        }
    }
}
