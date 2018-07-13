using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HtmlStripper
{
    public class HtmlFileStripper
    {
        // use '\0' as second attribute to remove a character
        public Dictionary<char, char> Characters { get; set; }
        public List<string> Tags { get; set; }

        public HtmlFileStripper()
        {
            Characters = new Dictionary<char, char>();
            Tags = new List<string>();
        }

        public HtmlFileStripper(Dictionary<char, char> newChararacters, List<string> newTags)
        {
            Characters = newChararacters;
            Tags = newTags;
        }

        public HtmlFileStripper(Dictionary<char, char> newCharacters)
        {
            Characters = newCharacters;
            Tags = new List<string>();
        }

        public HtmlFileStripper(List<string> newTags)
        {
            Tags = newTags;
            Characters = new Dictionary<char, char>();
        }

        private void StripTags(HtmlDocument inputDocument) {
            if (Tags.Count > 0)
            {
                foreach (var tag in Tags)
                {
                    HtmlNodeCollection tagNodes = inputDocument.DocumentNode.SelectNodes($"//{tag}");
                    if (tagNodes != null && tagNodes.Count > 0)
                    {
                        foreach (var node in tagNodes)
                        {
                            node.Remove();
                        }
                    }
                }
            }
        }

        private string stripInnerHtml(string innerHtml, char searchCharacter, char replaceCharacter, Encoding htmlEncoding)
        {
            char[] utf8Bytes = Encoding.UTF8.GetChars(Encoding.Convert(
                    htmlEncoding, 
                    Encoding.UTF8,
                    htmlEncoding.GetBytes(innerHtml)));
            if (replaceCharacter != '\0')
            {
                for (int i = 0; i < utf8Bytes.Length; i++)
                {
                    if (utf8Bytes[i] == searchCharacter)
                    {
                        utf8Bytes[i] = replaceCharacter;
                    }
                }
            }
            else
            {
                utf8Bytes = utf8Bytes.Where(b => b != searchCharacter).ToArray();
            }
            string utf8string = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(utf8Bytes));
            return utf8string;

        }

        private void StripCharacters(HtmlDocument inputDocument)
        {
            if (Characters.Count > 0)
            {
                HtmlNodeCollection nodes = inputDocument.DocumentNode.SelectNodes("//*");
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        foreach (var character in Characters.Keys)
                        {
                            node.InnerHtml = stripInnerHtml(node.InnerHtml, character, Characters[character], inputDocument.Encoding);
                        }
                    }
                }
            }
        }

        private void WriteOutput(HtmlDocument inputDocument, string outputFileName, Encoding encoding)
        {
            using (var writer = new System.IO.StreamWriter(outputFileName, false, encoding))
            {
                inputDocument.Save(writer);
            }
        }

        private HtmlDocument GetInputDocument(string inputFileName)
        {
            var inputDocument = new HtmlDocument();
            inputDocument.DetectEncodingAndLoad(inputFileName);
            return inputDocument;
        }

        /// <summary>
        /// Strips html tags from Tags property and characters from Characters property from the file at inputFileName and writes to the file at outputFileName
        /// </summary>
        /// <param name="inputFileName">An html file to be stripped</param>
        /// <param name="outputFileName">The output file for the stripped html</param>
        /// <param name="encoding">The encoding of the input and output files</param>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when input file does not exist</exception>
        /// <returns>True if operation was successful</returns>
        /// <remarks>If one of the elements in Characters has the same key and value, will loop infinitely.</remarks>
        public bool StripFile(string inputFileName, string outputFileName, Encoding encoding)
        {
            if (System.IO.File.Exists(inputFileName))
            {
                HtmlDocument inputDocument = GetInputDocument(inputFileName);
                StripTags(inputDocument);
                StripCharacters(inputDocument);
                WriteOutput(inputDocument, outputFileName, inputDocument.Encoding);
                return true;
            }
            else
            {
                System.IO.FileNotFoundException fileEx = new System.IO.FileNotFoundException("Input file does not exist", inputFileName);
                throw fileEx;
            }
        }
    }
}
