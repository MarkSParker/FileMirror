namespace FileMirror
{
    internal static class Parser
    {
        public static (bool, bool, string, string) ParseCommandLine(string[] args)
        {
            var nolog = false;                                                  // don't report file copies/folder creations
            var nocopy = false;                                                 // don't create any files/folders; just say what would have been done

            //  Check there are args
            if (args.Length < 2)
            {
                throw new ArgumentException("Insufficient arguments on command");
            }

            //  Get flags
            if (args[0].StartsWith('/'))
            {
                switch (args[0])
                {
                    case "/nolog":
                        nolog = true;
                        break;

                    case "/nocopy":
                        nocopy = true;
                        break;

                    default:
                        throw new ArgumentException($"flag '{args[0]}' is not legal...");
                }

                // Discard the flag
                args = args[1..];
            }

            //  Ensure there are exactly two folders left on command line
            if (args.Length != 2)
                throw new ArgumentException("Two, and only two, folders must be given");

            //  Trim whitespace from the folders
            args[0] = args[0].Trim();
            args[1] = args[1].Trim();

            //  Normalize the folders by appending a path separator if reqd
            if (args[0][^1] != Path.DirectorySeparatorChar)
                args[0] = args[0] + Path.DirectorySeparatorChar;

            if (args[1][^1] != Path.DirectorySeparatorChar)
                args[1] = args[1] + Path.DirectorySeparatorChar;

            //  Debug report
#if DEBUG
            Console.WriteLine("");
            Console.WriteLine("Normalized folders:");
            Console.WriteLine("A: " + args[0]);
            Console.WriteLine("B: " + args[1]);
            Console.WriteLine("");
#endif

            //  Check both are existing, rooted folders
            foreach (var folder in args)
            {
                //  Check not null
                if (string.IsNullOrWhiteSpace(folder))
                    throw new ArgumentException($"Folder not given.");

                //  Must have min length of three, eg C:\
                if (folder.Length < 3)
                    throw new ArgumentException($"Folder '{folder}' is too short to be a fully qualified folder.");

                //  UNC not supported
                if (folder.StartsWith(@"\\"))
                    throw new ArgumentException($"Folder '{folder}' UNC paths are not supported.");

                //  Must not be raw device, eg C:
                if (folder[folder.Length - 1] == ':')
                    throw new ArgumentException($"Folder '{folder}' must not be a raw device.");

                //  Check folder exists
                if (!Directory.Exists(folder))
                    throw new ArgumentException($"Folder '{folder}' does not exist.");

                //  Check rooted
                if (!Path.IsPathRooted(folder))
                    throw new ArgumentException($"Folder '{folder}' is not rooted (fully qualified).");
            }

            //  Folders must be different from each other (although mirror to self would be a no-op.)
            if (args[0] == args[1])
                throw new ArgumentException("Two different folders must be specified.");

            //  Folders must not be one inside the other else madness ensues
            if (args[0].StartsWith(args[1], StringComparison.InvariantCultureIgnoreCase)
                || args[1].StartsWith(args[0], StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("One folder must not be a subfolder of the other.");
            }

            //  Return the results of the parsing
            return (nolog, nocopy, args[0], args[1]);
        }


    }
}
