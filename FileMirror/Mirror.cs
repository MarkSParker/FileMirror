namespace FileMirror
{
    /// <summary>
    /// Supplies the functionality to 'mirror' one folder tree to another.
    /// </summary>
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
        /// Constructor
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
            //  Scan the tree
            var filesDict = Directory
                .GetFiles(rootFolder, "*", SearchOption.AllDirectories)
                .ToDictionary(path => Path.GetRelativePath(rootFolder, path), path => path);

#if DEBUG
            //  Debug report results of scan
            Console.WriteLine("");
            Console.WriteLine("Result of scannng: " + rootFolder);
            foreach (var key in filesDict.Keys)
                Console.WriteLine(filesDict[key]);
            Console.WriteLine("");
#endif

            return filesDict;
        }

        /// <summary>
        /// Copy all missing files from X to Y (which is either A to B or B to A.)
        /// </summary>
        /// <param name="mode">For reporting</param>
        /// <param name="filesFrom">Copy files from this tree</param>
        /// <param name="filesTo">Copy files to this tree</param>
        /// <param name="folderTo">Use this folder to calc abs path of destination file</param>
        private void PopulateTree(
            string mode, Dictionary<string, string> filesFrom, Dictionary<string, string> filesTo, string folderTo)
        {
            //  Init empty directory lookaside list
            createdAndCheckedDirs = new();

            //  For all files in src dictionary
            foreach (var key in filesFrom.Keys)
            {
                //  Get from and to files
                var fromFile = filesFrom[key];
                var toFile = Path.Combine(folderTo, key);

                //  If the key exists in dest dictionary
                if (filesTo.ContainsKey(key))
                {
                    var fiFrom = new FileInfo(fromFile);
                    var fiTo = new FileInfo(toFile);

                    if (fiFrom.Length != fiTo.Length)
                    {
                        Console.WriteLine(
                            $"WARNING: file '{key}' has different size in each location ({fiFrom.Length} bytes vs {fiTo.Length} bytes)");
                    }
                    else
                    {
                        if (!nolog)
                            Console.WriteLine($"[{mode}] Present  : {toFile}");
                        filesAlreadyCopied++;
                    }
                }
                else
                {
                    // Otherwise copy file across
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
            var action = nocopy ? "NotCopyTo" : "CopyingTo";
            Console.WriteLine($"[{mode}] {action}: {toFile}");

            //  May need to create the output folder. Dir is null if it's the root dir.
            var dir = Path.GetDirectoryName(toFile);
            CreateDirectory(dir);

            //  Copy the file
            if (!nocopy)
                File.Copy(fromFile, toFile);

            //  Maintain stats
            filesCopied++;

            //  Done
            return;
        }

        /// <summary>
        /// Ensures a directory exists and also puts it on a lookaside list so that
        /// each dir is only checked once.
        /// </summary>
        /// <param name="dir"></param>
        private void CreateDirectory(string? dir)
        {
            if (dir != null)
            {
                if (!createdAndCheckedDirs!.Contains(dir))
                {
                    if (!nocopy)
                    {
                        Directory.CreateDirectory(dir);
                    }

                    createdAndCheckedDirs.Add(dir);
                }
            }
        }
    }
}
