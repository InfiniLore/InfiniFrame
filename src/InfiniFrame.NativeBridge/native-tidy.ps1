Get-ChildItem . -Recurse -File -Include *.cpp,*.cc,*.cxx,*.c,*.hpp,*.h |
Where-Object {
    $_.FullName -notmatch '\\(build|out|bin|obj|vcpkg)\\'
} |
ForEach-Object {
    Write-Host "Running clang-tidy on $($_.FullName)"
    clang-tidy $_.FullName -p build --fix
}