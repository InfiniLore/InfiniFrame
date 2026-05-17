Get-ChildItem -Recurse -File -Include *.cpp,*.cxx,*.cc,*.c,*.hpp,*.hh,*.hxx,*.h,*.ixx,*.mm,*.m |
Where-Object {
    $_.FullName -notmatch '\\Dependencies\\' -and
        $_.FullName -notmatch '\\packages\\'
} |
ForEach-Object {
    Write-Host "Formatting $($_.FullName)"
    clang-format -i $_.FullName
}