//  FileMirror
//
//  Given two folders as arguments, copies all the missing files from A to B
//  and vice versa. Where there is a clash (same name, different size) the user
//  is given a warning. Backcopy is optional.
//
//  Missing folders are created as required.
//
//  Command format:
//
//      FileMirror/nocopy/nolog/bothways A:\a B:\b
//

//  Stats
using FileMirror;

var filesCopied = 0;
var filesAlreadyCopied = 0;

//  Flags
var nolog = false;                                                              // don't report file copies/folder creations
var nocopy = false;                                                             // don't create any files/folders; just say what would have been done
var bothways = false;

//  Folders to be mirrored. The order they are specified is irrelevant
var folderA = string.Empty;
var folderB = string.Empty;

try
{
    //  Parse the command line
    (nolog, nocopy, bothways, folderA, folderB) = Parser.ParseCommandLine(args);

    //  Mirror the folders
    var mirror = new Mirror(folderA, folderB);
    (filesCopied, filesAlreadyCopied) = mirror.MirrorTree(nolog, nocopy, bothways);

    //  Report
    Console.WriteLine("");
    Console.WriteLine(
        (nocopy
        ? "Files not copied:       "
        : "Files copied:           ")
        + filesCopied);
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
    Console.WriteLine("    FileMirror[/nolog][/nocopy][/bothways] <folderA> <folderB>");
    Console.WriteLine("");
    Console.WriteLine(@"    Eg:    filemirror/nolog c:\FolderA d:\SavedFiles\FolderB");
    Console.WriteLine("");
    Console.WriteLine("Missing files are replicated into both folder trees with subfolders");
    Console.WriteLine("being created as necessary. Both argument folders must already exist.");
    Console.WriteLine("");
    Console.WriteLine("If the same files already exist in both locations with different sizes");
    Console.WriteLine("they are reported as warnings.");
    Console.WriteLine("");
    Console.WriteLine("/nolog means don't report existing files, only files copied");
    Console.WriteLine("");
    Console.WriteLine("/nocopy means don't actually make copies; only report files which");
    Console.WriteLine("need to be copied.");
    Console.WriteLine("");
    Console.WriteLine("/bothways means copy A->B and B->A. By default only A->B is copied.");
    Console.WriteLine("");

    return;
}

