using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FileMirror
{
    internal class Mirror
    {
        private string folderA;
        private string folderB;
        private bool nolog = false;
        private bool nocopy = false;
        private int filesCopied = 0;
        private int filesAlreadyCopied = 0;
        private List<string>? createdAndCheckedDirs;

        /// <summary>
        /// Construictor
        /// </summary>
        /// <param name="folderA"></param>
        /// <param name="folderB"></param>
        public Mirror(string folderA, string folderB)
        {
            this.folderA = folderA;
            this.folderB = folderB;
        }

        /// <summary>
        /// Mirror a directory tree.
        /// </summary>
        /// <param name="nolog">Don't report files seen, only files copied</param>
        /// <param name="nocopy">Don't copy any files; just report</param>
        /// <returns>Number of files copied; number of files already copied</returns>
        public (int, int) MirrorTree(bool nolog = false, bool nocopy = false)
        {
            //  Stats
            filesCopied = 0;
            filesAlreadyCopied = 0;

            //  Record flags
            this.nolog = nolog;
            this.nocopy = nocopy;

            //  Normalize the folders by appending a path separator if reqd
            if (folderA[^1] != Path.DirectorySeparatorChar)
                folderA = folderA + Path.DirectorySeparatorChar;

            if (folderB[^1] != Path.DirectorySeparatorChar)
                folderB = folderB + Path.DirectorySeparatorChar;

            //  Scan both folders recursively; put results in a dictionary for easy lookup
            var filesA = ScanTree(folderA);
            var filesB = ScanTree(folderB);

            //  Populate A->B then B->A
            PopulateTree("A->B", filesA, filesB, folderB);
            PopulateTree("B->A", filesB, filesA, folderA);

            //  Done
            return (filesCopied, filesAlreadyCopied);
        }

        /// <summary>
        /// Build a dictionary of all files under a root
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <returns></returns>
        private static Dictionary<string, string> ScanTree(string rootFolder)
        {
            var filesDict = Directory
                .GetFiles(rootFolder, "*", SearchOption.AllDirectories)
                .ToDictionary(path => Path.GetRelativePath(rootFolder, path), path => path);

            return filesDict;
        }

        /// <summary>
        /// Copy all missing files from X to Y (which is either A to B or B to A.)
        /// </summary>
        /// <param name="mode">For reporting</param>
        /// <param name="filesX">Copy files from this tree</param>
        /// <param name="filesY">Copy files to this tree</param>
        /// <param name="folderY">Use this folder to calc abs path of destination file</param>
        private void PopulateTree(
            string mode, Dictionary<string, string> filesX, Dictionary<string, string> filesY, string folderY)
        {
            //  Init empty directory lookaside list
            createdAndCheckedDirs = new();

            //  For all files in dictionary X
            foreach (var key in filesX.Keys)
            {
                //  If the key exists in dictionary Y
                if (filesY.ContainsKey(key))
                {
                    // Check sizes; report error if different
                }
                //  Otherwise copy file across
                else
                {
                    //  Get from and to files
                    var fromFile = filesX[key];
                    var toFile = Path.Combine(folderY, key);

                    //  And copy it
                    CopyFile(mode, fromFile, toFile);

                }

            }

            return;
        }

        /// <summary>
        /// Copies a missing file from the source folder to the destination folder
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="fromFile"></param>
        /// <param name="toFile"></param>
        private void CopyFile(string mode, string fromFile, string toFile)
        {
            //  Report
            Console.WriteLine($"[{mode}] CopyingTo: {toFile}");

            //  May need to create the output folder. Dir is null if it's the root dir.
            var dir = Path.GetDirectoryName(toFile);
            if (dir != null)
                Directory.CreateDirectory(dir);

            //  Copy the file
            //File.Copy(fromFile, toFile);

            //  Maintain stats
            filesCopied++;

            //  Done
            return;
        }

        /// <summary>
        /// Implements
        /// </summary>
        /// <param name="dir"></param>
        private void CreateDirectory(string? dir)
        {
            if (dir != null)
            {
                if (!createdAndCheckedDirs!.Contains(dir))
                {
                    Directory.CreateDirectory(dir);
                    createdAndCheckedDirs.Add(dir);
                }
            }

        }
    }
}
