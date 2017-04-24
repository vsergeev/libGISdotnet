# Note: this project is functional, but no longer maintained.

## libgisdotnet

`libgisdotnet` is a C# port of the [libGIS](https://github.com/vsergeev/libGIS) library. Written by Jerry G. Scherer (@scherej1).

## File Structure

* `AtmelGeneric.cs` - Utility class to create, read, write, and print Atmel Generic binary records.
* `IntelHex.cs` -  Utility class to create, read, write, and print Intel HEX8 binary records.
* `SRecord.cs` - Utility class to create, read, write, and print Motorola S-Record binary records.
* `tests/`
    * `TestGIS_RecDump.cs` - Test program to print records in an Atmel Generic, Intel HEX, or Motorola S-Record file.
    * `TestGIS_Write.cs` - Test program to write and read back test records.

## Testing

Build the libgisdotnet test programs with `make`, and run `tests/TestGIS_Write.exe`:

```
$ make -f Makefile.mono
$ mono tests/TestGIS_Write.exe
```

## Building

Build libgisdotnet directly into your project by including one or more of the `AtmelGeneric.cs`, `IntelHex.cs`, and `SRecord.cs` files.

## License

libgisdotnet is MIT licensed. See the included [LICENSE](LICENSE) file.

