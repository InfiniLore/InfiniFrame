// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using InfiniTests;


// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/// <summary>
///     Every test in this assembly builds an ASP.NET Core host, and the Windows ARM64 lane runs several
///     test applications concurrently on a constrained runner. A test that normally completes in
///     milliseconds can therefore be starved for considerably longer than the ten second default, so the
///     assembly uses the same budget as the individual web-server tests.
/// </summary>
[assembly: DefaultTimeout(DefaultTimeoutAttribute.TimeoutValue + 50_000)]
