﻿; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
AC1001  | Usage    | Error    | Don't modify this parameter          
AC1002  | Usage    | Error    | Don't modify this member             
AC1003  | Usage    | Error    | Don't invoke this method             
AC1004  | Usage    | Error    | Don't invoke this parameter's method 
AC1005  | Usage    | Error    | Don't invoke this member's method    
AC1006 | Usage | Warning | Please don't invoke this method, because it is not a Const method
AC1007 | Usage | Error | Please don't invoke this method, because it is not a Const method
AC1008 | Usage | Error | You can't access data outside of the parameters
AC1101 | Usage | Error    | Add 'partial' keyword to the property
AC1102 | Usage | Error | Don't add body to the property
AC1103 | Usage | Error | You can only add the accessor get or set
AC1104 | Usage | Error | Please add partial method for getting this property
AC1105 | Usage | Error | Don't call this property in this method
AC1106 | Usage | Error | Don't add the static to this property when you add the attribute Prop.
AC1107 | Usage | Error | You can add partial method for setting this property
AC1108 | Usage | Warning | Wrong Comparer Type
AC2001  | Tool     | Warning  | Where is its name?                   
