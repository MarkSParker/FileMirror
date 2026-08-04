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
//      FileMirror/nocopy/nolog A:\a B:\b
//

//  Stats
using FileMirror;

var filesCopied = 0;
var filesAlreadyCopied = 0;

//  Flags (these are mutually exclusive)
var nolog = false;                                                              // don't report file copies/folder creations
var nocopy = false;                                                             // don't create any files/folders; just say what would have been done

//  Folders to be mirrored. The order they are specified is irrelevant
var folderA = string.Empty;
var folderB = string.Empty;

try
{

    //  Debug report command line
#if DEBUG
    Console.WriteLine("");
    Console.WriteLine($"Args length: {args.Length}");
    foreach (var arg in args)
        Console.WriteLine($"Arg: {arg}");
    Console.WriteLine("");
#endif

    //  Parse the command line
    (nolog, nocopy, folderA, folderB) = Parser.ParseCommandLine(args);

    //  Debug report result of parsing
#if DEBUG
    Console.WriteLine("");
    Console.WriteLine("folderA: " + folderA);
    Console.WriteLine("FolderB: " + folderB);
    Console.WriteLine("nolog:   " + nolog.ToString());
    Console.WriteLine("nocopy:  " + nocopy.ToString());
#endif

    //  We're good, mirror the folders
    var mirror = new Mirror(folderA, folderB);
    (filesCopied, filesAlreadyCopied) = mirror.MirrorTree(nolog, nocopy);

    //  Report
    Console.WriteLine("");
    Console.WriteLine("Files copied:           " + filesCopied);
    Console.WriteLine("Files already correct:  " + filesAlreadyCopied);
    Console.WriteLine("");
}
catch (ArgumentException ex)
{
    if (ex.Message != string.Empty)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
    }

    ReportSyntax();
}
catch (Exception ex)
{
    Console.Error.WriteLine("ERROR: " + ex.Message);
}

//  ReportSyntax
//
//  Advises on correct command syntax
static void ReportSyntax()
{
    Console.WriteLine("");
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

