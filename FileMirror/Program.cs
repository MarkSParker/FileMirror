//  FileMirror
//
//  Given two folders as arguments, copies all the missing files from A to B
//  and vice versa. Where there is a clash (same name, different size) the user
//  is given a warning.
//
//  Missing folders are created as required.
//
//  Command format:
//
//      FileMirror/reportonly/noreport A:\a B:\b
//

//  Stats
var filesCopied = 0;
var filesAllReadyCopied = 0;

//  Flags (these are mutually exclusive)
var nolog = false;                                                              // don't report file copies/folder creations
var nocopy = false;                                                             // don't create any files/folders; just say what would have been done

//  Folders to be mirrored. The order they are specified is irrelevant
var folderA = string.Empty;
var folderB = string.Empty;

try
{

    //  Del me
    Console.WriteLine($"Args length: {args.Length}");
    foreach (var arg in args)
        Console.WriteLine($"Arg: {arg}");

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


    //  We're good, Mirror the folders





}
catch (ArgumentException ex)
{
    if (ex.Message != string.Empty)
    {
        Console.WriteLine(ex.Message);
    }

    ReportSyntax();
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
}

//  ReportSyntax
//
//  Advises on correct command syntax
static void ReportSyntax()
{
    Console.WriteLine("Command format is:-");
    Console.WriteLine("");
    Console.WriteLine("    FileMirror[/nologging][/nocopy] <folderA> <folderB>");
    Console.WriteLine("");
    Console.WriteLine(@"    Eg:    filemirror/nolog c:\FilesX d:\SavedFiles\FilesX");
    Console.WriteLine("");
    Console.WriteLine("Missing files are replicated into both folder trees with subfolders");
    Console.WriteLine("being created as necessary. Both argument folders must already exist.");
    Console.WriteLine("");
    Console.WriteLine("If the same files already exist in both locations with different sizes");
    Console.WriteLine("they are reported as warnings.");
    Console.WriteLine("");

    return;
}

