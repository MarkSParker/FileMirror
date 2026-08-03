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
                throw new ArgumentException();

            //  Check both are existing folders
            foreach (var folder in args)
            {
                if (!Directory.Exists(folder))
                    throw new ArgumentException($"Folder '{folder}' does not exist.");

                if (!Path.IsPathRooted(folder))
                    throw new ArgumentException($"Folder '{folder}' is not rooted (fully qualified).");
            }

            //  Folders must be different from each other (althoug mirror to self would be a no-op.)
            if (args[0] == args[1])
                throw new ArgumentException("Two different folders must be specified.");

            //  Folders must not be one inside the other else maddness


            //  Return the results of the parsing
            return (nolog, nocopy, args[0], args[1]);
        }


    }
}
