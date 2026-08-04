Given two folders as arguments, copies all the missing files from A to B
and vice versa. Where there is a clash (same name, different size) the user
is given a warning.

Missing folders are created as required.

Command format:

  FileMirror/nocopy/nolog A:\a B:\b

/nocopy means don't do any copying or folder creation; just say what would
have been done.

/nolog means don't report files existing in both places; just report files
which are copied.
