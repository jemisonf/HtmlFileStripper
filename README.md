# HtmlFileStripper
A .NET library for stripping tags and special characters from HTML files

Requires HTML Agility pack version 1.x.

Uses .NET standard 2.0.

## Using

To use, add as a dependancy and add the line `using HtmlStripper;` at the start of your code.

You can initialize a new `HtmlFileStripper` with:
```C#
var FileStripper = new HtmlFileStripper()
```

A FileStripper uses the `Characters` and `Tags` properties to parse files. 

`Characters` is a `Dictionary` of UTF8 characters (not guaranteed to work properly with non UTF8 characters); for each element, the key represents a character to search for and the value represents the character it should be replaced with. Use `\0` to remove a character instead of using a replacement.

`Tags` is a `List` of tag names to remove from the file.

When have set `Characters` and `Tags` through either the constructor or through setter methods, you can call `StripFile` to strip an actual file. If you want to overwrite an existing file, just use the same value for inputFile and outputFile.
