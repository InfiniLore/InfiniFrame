Get-ChildItem -Recurse -Include *.cpp,*.cxx,*.cc,*.c,*.hpp,*.hh,*.hxx,*.h,*.ixx,*.mm,*.m |
ForEach-Object {
    clang-format -i $_.FullName
}