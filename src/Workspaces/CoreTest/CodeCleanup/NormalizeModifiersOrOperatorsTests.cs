﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeCleanup;
using Microsoft.CodeAnalysis.CodeCleanup.Providers;
using Microsoft.CodeAnalysis.Test.Utilities;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.UnitTests.CodeCleanup
{
    public class NormalizeModifiersOrOperatorsTests
    {
        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void PartialMethod()
        {
            var code = @"[|Class A
    Private Partial Sub()
    End Sub
End Class|]";

            var expected = @"Class A
    Partial Private Sub()
    End Sub
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void PartialClass()
        {
            var code = @"[|Public Partial Class A
End Class|]";

            var expected = @"Partial Public Class A
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void DefaultProperty()
        {
            var code = @"[|Class Class1
    Public Default Property prop1(i As Integer) As Integer
        Get
            Return i
        End Get
        Set(ByVal value As Integer)
        End Set
    End Property
End Class|]";

            var expected = @"Class Class1
    Default Public Property prop1(i As Integer) As Integer
        Get
            Return i
        End Get
        Set(ByVal value As Integer)
        End Set
    End Property
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Accessors()
        {
            var code = @"[|Public Module M
End Module

NotInheritable Friend Class C
    MustInherit Protected Friend Class N
        Overridable Public  Sub Test()
        End Sub

        MustOverride Protected  Sub Test2()

        Shared Private  Sub Test3()
        End Sub
    End Class

    Public Class O
        Inherits N

        Shadows Public Sub Test()
        End Sub

        Overrides Protected Sub Test2()
        End Sub
    End Class
End Class|]";

            var expected = @"Public Module M
End Module

Friend NotInheritable Class C
    Protected Friend MustInherit Class N
        Public Overridable Sub Test()
        End Sub

        Protected MustOverride Sub Test2()

        Private Shared Sub Test3()
        End Sub
    End Class

    Public Class O
        Inherits N

        Public Shadows Sub Test()
        End Sub

        Protected Overrides Sub Test2()
        End Sub
    End Class
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Structure()
        {
            var code = @"[|Public Partial Structure S
End Structure|]";

            var expected = @"Partial Public Structure S
End Structure";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Interface()
        {
            var code = @"[|Public Interface O
    Public Interface S
    End Interface
End Interface

Public Interface O2
    Inherits O

    Shadows Public Interface S
    End Interface
End Interface|]";

            var expected = @"Public Interface O
    Public Interface S
    End Interface
End Interface

Public Interface O2
    Inherits O

    Public Shadows Interface S
    End Interface
End Interface";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Class()
        {
            var code = @"[|MustInherit Public  Class C
End Class|]";

            var expected = @"Public MustInherit Class C
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Enum()
        {
            var code = @"[|Public Class O
    Public Enum S
        None
    End Enum
End Class

Public Class O2
    Inherits O

    Shadows Public  Enum S
        None
    End Enum
End Class|]";

            var expected = @"Public Class O
    Public Enum S
        None
    End Enum
End Class

Public Class O2
    Inherits O

    Public Shadows Enum S
        None
    End Enum
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Method()
        {
            var code = @"[|Public Class O
    Overridable Protected Function Test() As Integer
        Return 0
    End Function
End Class|]";

            var expected = @"Public Class O
    Protected Overridable Function Test() As Integer
        Return 0
    End Function
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Declare()
        {
            var code = @"[|Class C
    Overloads Public  Declare Function getUserName Lib ""advapi32.dll"" Alias ""GetUserNameA"" (ByVal lpBuffer As String, ByRef nSize As Integer) As Integer
End Class|]";

            var expected = @"Class C
    Public Overloads Declare Function getUserName Lib ""advapi32.dll"" Alias ""GetUserNameA"" (ByVal lpBuffer As String, ByRef nSize As Integer) As Integer
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Delegate()
        {
            var code = @"[|Public Class O
    Public Delegate Function S() As Integer
End Class

Public Class O2
    Inherits O

    Shadows Public  Delegate Function S() As Integer
End Class|]";

            var expected = @"Public Class O
    Public Delegate Function S() As Integer
End Class

Public Class O2
    Inherits O

    Public Shadows Delegate Function S() As Integer
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Event()
        {
            var code = @"[|Public Class O
    Shared Public  Event Test As System.EventHandler
End Class|]";

            var expected = @"Public Class O
    Public Shared Event Test As System.EventHandler
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Operator()
        {
            var code = @"[|Public Structure abc
    Shared Overloads Public  Operator And(ByVal x As abc, ByVal y As abc) As abc
    End Operator
End Structure|]";

            var expected = @"Public Structure abc
    Public Overloads Shared Operator And(ByVal x As abc, ByVal y As abc) As abc
    End Operator
End Structure";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Property()
        {
            var code = @"[|Class Class1
   Overridable  Public  Property prop1 As Integer
End Class|]";

            var expected = @"Class Class1
    Public Overridable Property prop1 As Integer
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Accessor()
        {
            var code = @"[|Class Class1
    Public Property prop1 As Integer
        Private Get
            Return 0
        End Get
        Set(value As Integer)

        End Set
    End Property
End Class|]";

            var expected = @"Class Class1
    Public Property prop1 As Integer
        Private Get
            Return 0
        End Get
        Set(value As Integer)

        End Set
    End Property
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void IncompleteMember()
        {
            var code = @"[|Class Program
    Shared Private Dim
End Class|]";

            var expected = @"Class Program
    Shared Private Dim
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Field()
        {
            var code = @"[|Class Program
    Shared ReadOnly Private Dim f = 1
End Class|]";

            var expected = @"Class Program
    Private Shared ReadOnly f = 1
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void NotOverridable_Overridable_Overrides()
        {
            var code = @"[|Public Class Program
    Class N
        Inherits Program

        Overrides Public   NotOverridable Sub test()
            MyBase.test()
        End Sub
    End Class

    Overridable Public  Sub test()
    End Sub
End Class|]";

            var expected = @"Public Class Program
    Class N
        Inherits Program

        Public NotOverridable Overrides Sub test()
            MyBase.test()
        End Sub
    End Class

    Public Overridable Sub test()
    End Sub
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void MustOverride_MustInherit()
        {
            var code = @"[|MustInherit Public Class Program
    MustOverride Public Sub test()
End Class|]";

            var expected = @"Public MustInherit Class Program
    Public MustOverride Sub test()
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Overloads()
        {
            var code = @"[|Public MustInherit Class Program
   Overloads Public  Sub test()
    End Sub

    Overloads Public  Sub test(i As Integer)
    End Sub
End Class|]";

            var expected = @"Public MustInherit Class Program
    Public Overloads Sub test()
    End Sub

    Public Overloads Sub test(i As Integer)
    End Sub
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void NotInheritable()
        {
            var code = @"[|NotInheritable Public Class Program
End Class|]";

            var expected = @"Public NotInheritable Class Program
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Shared_Shadow_ReadOnly_Const()
        {
            var code = @"[|Class C
    Class N
        Public  Sub Test()
        End Sub

        Const Private  Dim c As Integer = 2
        Shared ReadOnly Private Dim f = 1
    End Class

    Public Class O
        Inherits N

        Shadows Public Sub Test()
        End Sub
    End Class
End Class|]";

            var expected = @"Class C
    Class N
        Public Sub Test()
        End Sub

        Private Const c As Integer = 2
        Private Shared ReadOnly f = 1
    End Class

    Public Class O
        Inherits N

        Public Shadows Sub Test()
        End Sub
    End Class
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void WriteOnly()
        {
            var code = @"[|Class C
    WriteOnly Public  Property Test
        Set(value)
        End Set
    End Property
End Class|]";

            var expected = @"Class C
    Public WriteOnly Property Test
        Set(value)
        End Set
    End Property
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void WithEvent_Custom_Dim()
        {
            var code = @"[|Imports System

Public Class A
     Public Custom Event MyEvent As EventHandler
        AddHandler(value As EventHandler)
        End AddHandler

        RemoveHandler(value As EventHandler)
        End RemoveHandler

        RaiseEvent(sender As Object, e As EventArgs)
        End RaiseEvent
    End Event
End Class

Class B
    WithEvents Dim EventSource As A
    Public Sub EventHandler(s As Object, a As EventArgs) Handles EventSource.MyEvent
    End Sub
End Class|]";

            var expected = @"Imports System

Public Class A
    Public Custom Event MyEvent As EventHandler
        AddHandler(value As EventHandler)
        End AddHandler

        RemoveHandler(value As EventHandler)
        End RemoveHandler

        RaiseEvent(sender As Object, e As EventArgs)
        End RaiseEvent
    End Event
End Class

Class B
    Dim WithEvents EventSource As A
    Public Sub EventHandler(s As Object, a As EventArgs) Handles EventSource.MyEvent
    End Sub
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Widening_Narrowing()
        {
            var code = @"[|Public Structure digit
Widening  Shared  Public Operator CType(ByVal d As digit) As Byte
        Return 0
    End Operator
     Narrowing Public Shared  Operator CType(ByVal b As Byte) As digit
        Return Nothing
    End Operator
End Structure|]";

            var expected = @"Public Structure digit
    Public Shared Widening Operator CType(ByVal d As digit) As Byte
        Return 0
    End Operator
    Public Shared Narrowing Operator CType(ByVal b As Byte) As digit
        Return Nothing
    End Operator
End Structure";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Static_Const_Dim()
        {
            var code = @"[|Class A
    Sub Method()
        Dim Static a As Integer = 1
        Const a2 As Integer = 2
    End Sub
End Class|]";

            var expected = @"Class A
    Sub Method()
        Static Dim a As Integer = 1
        Const a2 As Integer = 2
    End Sub
End Class";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(544520, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void RemoveByVal1()
        {
            var code = @"[|Class A
    Sub Method(ByVal t As String)
    End Sub
End Class|]";

            var expected = @"Class A
    Sub Method(ByVal t As String)
    End Sub
End Class";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(544520, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void RemoveByVal2()
        {
            var code = @"[|Class A
    Sub Method(ByVal t As String, ByRef t1 As String)
    End Sub
End Class|]";

            var expected = @"Class A
    Sub Method(ByVal t As String, ByRef t1 As String)
    End Sub
End Class";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(544520, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void RemoveByVal_LineContinuation()
        {
            var code = @"[|Class A
    Sub Method( _
        ByVal _
              _
            t As String, ByRef t1 As String)
    End Sub
End Class|]";

            var expected = @"Class A
    Sub Method( _
        ByVal _
 _
            t As String, ByRef t1 As String)
    End Sub
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void RemoveDim()
        {
            var code = @"[|Class A
    Dim  Shared Private a As Integer = 1
End Class|]";

            var expected = @"Class A
    Private Shared a As Integer = 1
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void RemoveDim_LineContinuation()
        {
            var code = @"[|Class A
    Dim _
        Shared _
        Private _
            a As Integer = 1
End Class|]";

            var expected = @"Class A
    Private _
        Shared _
 _
            a As Integer = 1
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void LessThanGreaterThan()
        {
            var code = @"[|Class A
    Sub Test()
        If 1 >< 2 Then
        End If
    End Sub
End Class|]";

            var expected = @"Class A
    Sub Test()
        If 1 <> 2 Then
        End If
    End Sub
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void GreaterThanEquals()
        {
            var code = @"[|Class A
    Sub Test()
        If 1 => 2 Then
        End If
    End Sub
End Class|]";

            var expected = @"Class A
    Sub Test()
        If 1 >= 2 Then
        End If
    End Sub
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void LessThanEquals()
        {
            var code = @"[|Class A
    Sub Test()
        If 1 =< 2 Then
        End If
    End Sub
End Class|]";

            var expected = @"Class A
    Sub Test()
        If 1 <= 2 Then
        End If
    End Sub
End Class";

            Verify(code, expected);
        }

        [Fact]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void LessThanEquals_LineContinuation()
        {
            var code = @"[|Class A
    Sub Test()
        If 1 _ 
            = _ 
            < _
                2 Then
        End If
    End Sub
End Class|]";

            var expected = @"Class A
    Sub Test()
        If 1 _
            <= _
                2 Then
        End If
    End Sub
End Class";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(544300, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void NormalizedOperator_StructuredTrivia()
        {
            var code = @"[|#If VBC_VER => 9.0|]";

            var expected = @"#If VBC_VER >= 9.0";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(544520, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void DontRemoveByVal()
        {
            var code = @"[|Module Program
    Sub Main(
        ByVal _
        args _
        As String)
    End Sub
End Module|]";

            var expected = @"Module Program
    Sub Main(
        ByVal _
        args _
        As String)
    End Sub
End Module";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(544561, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void NormalizeOperator_Text()
        {
            var code = @"[|Module Program
    Sub Main()
        Dim z = 1
        Dim y = 2
        Dim x = z <   > y
    End Sub
End Module|]";

            var expected = @"Module Program
    Sub Main()
        Dim z = 1
        Dim y = 2
        Dim x = z <> y
    End Sub
End Module";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(544557, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void NormalizeOperator_OperatorStatement()
        {
            var code = @"[|Class S
    Shared Operator >< (s1 As S, s2 As   S) As S
End Class|]";

            var expected = @"Class S
    Shared Operator <>(s1 As S, s2 As S) As S
End Class";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(544574, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void Reorder_OperatorTokenAndModifiers()
        {
            var code = @"[|Class S
    Shared Operator Widening CType(aa As S) As Byte
End Class|]";

            var expected = @"Class S
    Shared Widening Operator CType(aa As S) As Byte
End Class";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(546521, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void SkippedTokenOperator()
        {
            var code = @"[|Module M
    Public Shared Narrowing Operator CTypeByVal s As Integer) As Test2
        Return New Test2()
    End Operator
End Module|]";

            var expected = @"Module M
    Public Shared Narrowing Operator CTypeByVal s As Integer) As Test2
        Return New Test2()
    End Operator
End Module";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(547255, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void ReorderAsyncModifier()
        {
            var code = @"[|Module M
    Public Async Function Foo() As Task(Of Integer)
        Return 0
    End Function

    Async Public Function Foo2() As Task(Of Integer)
        Return 0
    End Function

    Async Overridable Public Function Foo3() As Task(Of Integer)
        Return 0
    End Function
End Module|]";

            var expected = @"Module M
    Public Async Function Foo() As Task(Of Integer)
        Return 0
    End Function

    Public Async Function Foo2() As Task(Of Integer)
        Return 0
    End Function

    Public Overridable Async Function Foo3() As Task(Of Integer)
        Return 0
    End Function
End Module";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(547255, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void ReorderIteratorModifier()
        {
            var code = @"[|Module M
    Public Iterator Function Foo() As IEnumerable(Of Integer)
        Yield Return 0
    End Function

    Iterator Public Function Foo2() As IEnumerable(Of Integer)
        Yield Return 0
    End Function

    Iterator Overridable Public Function Foo3() As IEnumerable(Of Integer)
        Yield Return 0
    End Function
End Module|]";

            var expected = @"Module M
    Public Iterator Function Foo() As IEnumerable(Of Integer)
        Yield Return 0
    End Function

    Public Iterator Function Foo2() As IEnumerable(Of Integer)
        Yield Return 0
    End Function

    Public Overridable Iterator Function Foo3() As IEnumerable(Of Integer)
        Yield Return 0
    End Function
End Module";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(611766, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void ReorderDuplicateModifiers()
        {
            var code = @"[|Module M
    Public Public Function Foo() As Integer
        Return 0
    End Function

    Iterator Public Public Iterator Public Function Foo2() As IEnumerable(Of Integer)
        Yield Return 0
    End Function
End Module|]";

            var expected = @"Module M
    Public Function Foo() As Integer
        Return 0
    End Function

    Public Iterator Function Foo2() As IEnumerable(Of Integer)
        Yield Return 0
    End Function
End Module";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(530058, "DevDiv")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void TestBadOperatorToken()
        {
            var code = @"[|Module Test
Class c1 
Shared Operator ||(ByVal x As c1, ByVal y As c1) As Integer
End Operator
End Class
End Module|]";

            var expected = @"Module Test
    Class c1
        Shared Operator ||(ByVal x As c1, ByVal y As c1) As Integer
        End Operator
    End Class
End Module";

            Verify(code, expected);
        }

        [Fact]
        [WorkItem(1534, "https://github.com/dotnet/roslyn/issues/1534")]
        [Trait(Traits.Feature, Traits.Features.NormalizeModifiersOrOperators)]
        public void TestColonEqualsToken()
        {
            var code = @"[|Module Program
    Sub Main(args As String())
        Main(args   :     =    args)
    End Sub
End Module|]";

            var expected = @"Module Program
    Sub Main(args As String())
        Main(args:=args)
    End Sub
End Module";

            Verify(code, expected);
        }

        private void Verify(string codeWithMarker, string expectedResult)
        {
            var codeWithoutMarker = default(string);
            var textSpans = (IList<TextSpan>)new List<TextSpan>();
            MarkupTestFile.GetSpans(codeWithMarker, out codeWithoutMarker, out textSpans);

            var document = CreateDocument(codeWithoutMarker, LanguageNames.VisualBasic);
            var codeCleanups = CodeCleaner.GetDefaultProviders(document).Where(p => p.Name == PredefinedCodeCleanupProviderNames.NormalizeModifiersOrOperators || p.Name == PredefinedCodeCleanupProviderNames.Format);

            var cleanDocument = CodeCleaner.CleanupAsync(document, textSpans[0], codeCleanups).Result;

            Assert.Equal(expectedResult, cleanDocument.GetSyntaxRootAsync().Result.ToFullString());
        }

        private static Document CreateDocument(string code, string language)
        {
            var solution = new AdhocWorkspace().CurrentSolution;
            var projectId = ProjectId.CreateNewId();
            var project = solution.AddProject(projectId, "Project", "Project.dll", language).GetProject(projectId);

            return project.AddDocument("Document", SourceText.From(code));
        }
    }
}
