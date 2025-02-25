// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.Interop;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using static Microsoft.Interop.Analyzers.CustomMarshallerAttributeAnalyzer;

using VerifyCS = LibraryImportGenerator.UnitTests.Verifiers.CSharpCodeFixVerifier<
    Microsoft.Interop.Analyzers.CustomMarshallerAttributeAnalyzer,
    Microsoft.Interop.Analyzers.CustomMarshallerAttributeFixer>;

namespace LibraryImportGenerator.UnitTests
{
    [ActiveIssue("https://github.com/dotnet/runtime/issues/60650", TestRuntimes.Mono)]
    public class CustomMarshallerAttributeAnalyzerTests_StatefulLinearCollectionShapeValidation
    {
        [Fact]
        public async Task ModeThatUsesManagedToUnmanagedShape_Missing_AllMethods_ReportsDiagnostic()
        {
            string source = """
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedIn, typeof({|#0:MarshallerType<>|}))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedOut, typeof({|#1:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source,
                VerifyCS.Diagnostic(StatefulMarshallerRequiresFreeRule).WithLocation(0).WithArguments("MarshallerType<T>"),
                VerifyCS.Diagnostic(StatefulMarshallerRequiresFromManagedRule).WithLocation(0).WithArguments("MarshallerType<T>", MarshalMode.ManagedToUnmanagedIn, "ManagedType"),
                VerifyCS.Diagnostic(StatefulMarshallerRequiresFromManagedRule).WithLocation(1).WithArguments("MarshallerType<T>", MarshalMode.UnmanagedToManagedOut, "ManagedType"),
                VerifyCS.Diagnostic(StatefulMarshallerRequiresToUnmanagedRule).WithLocation(0).WithArguments("MarshallerType<T>", MarshalMode.ManagedToUnmanagedIn, "ManagedType"),
                VerifyCS.Diagnostic(StatefulMarshallerRequiresToUnmanagedRule).WithLocation(1).WithArguments("MarshallerType<T>", MarshalMode.UnmanagedToManagedOut, "ManagedType"),
                VerifyCS.Diagnostic(LinearCollectionInRequiresCollectionMethodsRule).WithLocation(0).WithArguments("MarshallerType<T>", MarshalMode.ManagedToUnmanagedIn, "ManagedType"),
                VerifyCS.Diagnostic(LinearCollectionInRequiresCollectionMethodsRule).WithLocation(1).WithArguments("MarshallerType<T>", MarshalMode.UnmanagedToManagedOut, "ManagedType"),
                VerifyCS.Diagnostic(StatefulMarshallerRequiresFreeRule).WithLocation(1).WithArguments("MarshallerType<T>"));
        }

        [Fact]
        public async Task ModeThatUsesManagedToUnmanagedShape_Missing_ContainerMethods_ReportsDiagnostic()
        {
            string source = """
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedIn, typeof({|#0:MarshallerType<>|}))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedOut, typeof({|#1:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromManaged(ManagedType m) {}
                    public nint ToUnmanaged() => default;
                    public void Free() {}
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source,
                VerifyCS.Diagnostic(LinearCollectionInRequiresCollectionMethodsRule).WithLocation(0).WithArguments("MarshallerType<T>", MarshalMode.ManagedToUnmanagedIn, "ManagedType"),
                VerifyCS.Diagnostic(LinearCollectionInRequiresCollectionMethodsRule).WithLocation(1).WithArguments("MarshallerType<T>", MarshalMode.UnmanagedToManagedOut, "ManagedType"));
        }

        [Fact]
        public async Task ModeThatUsesManagedToUnmanagedShape_Missing_GetManagedValuesSource_ReportsDiagnostic()
        {
            string source = """
                using System;
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedIn, typeof({|#0:MarshallerType<>|}))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedOut, typeof({|#1:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromManaged(ManagedType m) {}
                    public nint ToUnmanaged() => default;
                    public void Free() {}
                    public Span<T> GetUnmanagedValuesDestination() => default;
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source,
                VerifyCS.Diagnostic(LinearCollectionInRequiresCollectionMethodsRule).WithLocation(0).WithArguments("MarshallerType<T>", MarshalMode.ManagedToUnmanagedIn, "ManagedType"),
                VerifyCS.Diagnostic(LinearCollectionInRequiresCollectionMethodsRule).WithLocation(1).WithArguments("MarshallerType<T>", MarshalMode.UnmanagedToManagedOut, "ManagedType"));
        }

        [Fact]
        public async Task ModeThatUsesManagedToUnmanagedShape_Missing_GetUnmanagedValuesDestination_ReportsDiagnostic()
        {
            string source = """
                using System;
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedIn, typeof({|#0:MarshallerType<>|}))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedOut, typeof({|#1:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromManaged(ManagedType m) {}
                    public nint ToUnmanaged() => default;
                    public void Free() {}
                    public ReadOnlySpan<byte> GetManagedValuesSource() => default;
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source,
                VerifyCS.Diagnostic(LinearCollectionInRequiresCollectionMethodsRule).WithLocation(1).WithArguments("MarshallerType<T>", MarshalMode.UnmanagedToManagedOut, "ManagedType"),
                VerifyCS.Diagnostic(LinearCollectionInRequiresCollectionMethodsRule).WithLocation(0).WithArguments("MarshallerType<T>", MarshalMode.ManagedToUnmanagedIn, "ManagedType"));
        }

        [Fact]
        public async Task ModeThatUsesManagedToUnmanagedShape_DoesNotReportDiagnostic()
        {
            string source = """
                using System;
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedIn, typeof(MarshallerType<>))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedOut, typeof(MarshallerType<>))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromManaged(ManagedType m) {}
                    public nint ToUnmanaged() => default;
                    public void Free() {}
                    public ReadOnlySpan<byte> GetManagedValuesSource() => default;
                    public Span<T> GetUnmanagedValuesDestination() => default;
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source);
        }


        [Fact]
        public async Task ModeThatUsesManagedToUnmanagedShape_InvalidCollectionElementType_DoesNotReportDiagnostic()
        {
            string source = """
                using System;
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedIn, typeof({|#0:MarshallerType<>|}))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedOut, typeof({|#1:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromManaged(ManagedType m) {}
                    public nint ToUnmanaged() => default;
                    public void Free() {}
                    public ReadOnlySpan<byte> GetManagedValuesSource() => default;
                    public Span<byte> GetUnmanagedValuesDestination() => default;
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source,
                VerifyCS.Diagnostic(ReturnTypeMustBeExpectedTypeRule).WithLocation(0).WithArguments("MarshallerType<T>.GetUnmanagedValuesDestination()", "System.Span<T>"),
                VerifyCS.Diagnostic(ReturnTypeMustBeExpectedTypeRule).WithLocation(1).WithArguments("MarshallerType<T>.GetUnmanagedValuesDestination()", "System.Span<T>"));
        }

        [Fact]
        public async Task ModeThatUsesUnmanagedToManagedShape_Missing_AllMethods_ReportsDiagnostic()
        {
            string source = """
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedOut, typeof({|#0:MarshallerType<>|}))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedIn, typeof({|#1:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source,
                VerifyCS.Diagnostic(StatefulMarshallerRequiresFreeRule).WithLocation(0).WithArguments("MarshallerType<T>"),
                VerifyCS.Diagnostic(StatefulMarshallerRequiresToManagedRule).WithLocation(0).WithArguments("MarshallerType<T>", MarshalMode.ManagedToUnmanagedOut, "ManagedType"),
                VerifyCS.Diagnostic(StatefulMarshallerRequiresToManagedRule).WithLocation(1).WithArguments("MarshallerType<T>", MarshalMode.UnmanagedToManagedIn, "ManagedType"),
                VerifyCS.Diagnostic(StatefulMarshallerRequiresFromUnmanagedRule).WithLocation(0).WithArguments("MarshallerType<T>", MarshalMode.ManagedToUnmanagedOut, "ManagedType"),
                VerifyCS.Diagnostic(StatefulMarshallerRequiresFromUnmanagedRule).WithLocation(1).WithArguments("MarshallerType<T>", MarshalMode.UnmanagedToManagedIn, "ManagedType"),
                VerifyCS.Diagnostic(LinearCollectionOutRequiresCollectionMethodsRule).WithLocation(0).WithArguments("MarshallerType<T>", MarshalMode.ManagedToUnmanagedOut, "ManagedType"),
                VerifyCS.Diagnostic(LinearCollectionOutRequiresCollectionMethodsRule).WithLocation(1).WithArguments("MarshallerType<T>", MarshalMode.UnmanagedToManagedIn, "ManagedType"),
                VerifyCS.Diagnostic(StatefulMarshallerRequiresFreeRule).WithLocation(1).WithArguments("MarshallerType<T>"));
        }

        [Fact]
        public async Task ModeThatUsesUnmanagedToManagedShape_Missing_ContainerMethods_ReportsDiagnostic()
        {
            string source = """
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedOut, typeof({|#0:MarshallerType<>|}))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedIn, typeof({|#1:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromUnmanaged(int f) {}
                    public ManagedType ToManaged() => default;
                    public void Free() {}
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source,
                VerifyCS.Diagnostic(LinearCollectionOutRequiresCollectionMethodsRule).WithLocation(0).WithArguments("MarshallerType<T>", MarshalMode.ManagedToUnmanagedOut, "ManagedType"),
                VerifyCS.Diagnostic(LinearCollectionOutRequiresCollectionMethodsRule).WithLocation(1).WithArguments("MarshallerType<T>", MarshalMode.UnmanagedToManagedIn, "ManagedType"));
        }

        [Fact]
        public async Task ModeThatUsesUnmanagedToManagedShape_Missing_GetUnmanagedValuesSource_ReportsDiagnostic()
        {
            string source = """
                using System;
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedOut, typeof({|#0:MarshallerType<>|}))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedIn, typeof({|#1:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromUnmanaged(int f) {}
                    public ManagedType ToManaged() => default;
                    public void Free() {}
                    public Span<byte> GetManagedValuesDestination(int numElements) => default;
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source,
                VerifyCS.Diagnostic(LinearCollectionOutRequiresCollectionMethodsRule).WithLocation(0).WithArguments("MarshallerType<T>", MarshalMode.ManagedToUnmanagedOut, "ManagedType"),
                VerifyCS.Diagnostic(LinearCollectionOutRequiresCollectionMethodsRule).WithLocation(1).WithArguments("MarshallerType<T>", MarshalMode.UnmanagedToManagedIn, "ManagedType"));
        }

        [Fact]
        public async Task ModeThatUsesUnmanagedToManagedShape_Missing_GetManagedValuesDestination_ReportsDiagnostic()
        {
            string source = """
                using System;
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedOut, typeof({|#0:MarshallerType<>|}))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedIn, typeof({|#1:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromUnmanaged(int f) {}
                    public ManagedType ToManaged() => default;
                    public void Free() {}
                    public ReadOnlySpan<T> GetUnmanagedValuesSource(int numElements) => default;
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source,
                VerifyCS.Diagnostic(LinearCollectionOutRequiresCollectionMethodsRule).WithLocation(1).WithArguments("MarshallerType<T>", MarshalMode.UnmanagedToManagedIn, "ManagedType"),
                VerifyCS.Diagnostic(LinearCollectionOutRequiresCollectionMethodsRule).WithLocation(0).WithArguments("MarshallerType<T>", MarshalMode.ManagedToUnmanagedOut, "ManagedType"));
        }

        [Fact]
        public async Task ModeThatUsesUnmanagedToManagedShape_DoesNotReportDiagnostic()
        {
            string source = """
                using System;
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedOut, typeof(MarshallerType<>))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedIn, typeof(MarshallerType<>))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromUnmanaged(int f) {}
                    public ManagedType ToManaged() => default;
                    public void Free() {}
                    public Span<byte> GetManagedValuesDestination(int numElements) => default;
                    public ReadOnlySpan<T> GetUnmanagedValuesSource(int numElements) => default;
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source);
        }

        [Fact]
        public async Task ModeThatUsesUnmanagedToManagedShape_InvalidCollectionElementType_ReportsDiagnostic()
        {
            string source = """
                using System;
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedOut, typeof({|#0:MarshallerType<>|}))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedIn, typeof({|#1:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromUnmanaged(int f) {}
                    public ManagedType ToManaged() => default;
                    public void Free() {}
                    public Span<byte> GetManagedValuesDestination(int numElements) => default;
                    public ReadOnlySpan<int> GetUnmanagedValuesSource(int numElements) => default;
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source,
                VerifyCS.Diagnostic(ReturnTypeMustBeExpectedTypeRule).WithLocation(0).WithArguments("MarshallerType<T>.GetUnmanagedValuesSource(int)", "System.ReadOnlySpan<T>"),
                VerifyCS.Diagnostic(ReturnTypeMustBeExpectedTypeRule).WithLocation(1).WithArguments("MarshallerType<T>.GetUnmanagedValuesSource(int)", "System.ReadOnlySpan<T>"));
        }

        [Fact]
        public async Task CallerAllocatedBuffer_NoBufferSize_ReportsDiagnostic()
        {
            string source = """
                using System;
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedIn, typeof({|#0:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromManaged(ManagedType m, Span<byte> buffer) {}
                    public nint ToUnmanaged() => default;
                    public void Free() {}
                    public ReadOnlySpan<byte> GetManagedValuesSource() => default;
                    public Span<T> GetUnmanagedValuesDestination() => default;
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(
                source,
                VerifyCS.Diagnostic(CallerAllocFromManagedMustHaveBufferSizeRule).WithLocation(0).WithArguments("MarshallerType<T>", "byte"));
        }

        [Fact]
        public async Task ModeThatUsesBidirectionalShape_DoesNotReportDiagnostic()
        {
            string source = """
                using System;
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedRef, typeof({|#0:MarshallerType<>|}))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedRef, typeof({|#1:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromManaged(ManagedType m) {}
                    public nint ToUnmanaged() => default;
                    public ReadOnlySpan<byte> GetManagedValuesSource() => default;
                    public Span<T> GetUnmanagedValuesDestination() => default;
                    public void FromUnmanaged(nint i) {}
                    public ManagedType ToManaged() => default;
                    public Span<byte> GetManagedValuesDestination(int numElements) => default;
                    public ReadOnlySpan<T> GetUnmanagedValuesSource(int numElements) => default;
                    public void Free() {}
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(source);
        }

        [Fact]
        public async Task ModeThatUsesBidirectionalShape_MismatchedManagedElementTypes_ReportsDiagnostic()
        {
            string source = """
                using System;
                using System.Runtime.InteropServices.Marshalling;
                
                class ManagedType {}
                
                [CustomMarshaller(typeof(ManagedType), MarshalMode.ManagedToUnmanagedRef, typeof({|#0:MarshallerType<>|}))]
                [CustomMarshaller(typeof(ManagedType), MarshalMode.UnmanagedToManagedRef, typeof({|#1:MarshallerType<>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T>
                {
                    public void FromManaged(ManagedType m) {}
                    public nint ToUnmanaged() => default;
                    public ReadOnlySpan<byte> GetManagedValuesSource() => default;
                    public Span<T> GetUnmanagedValuesDestination() => default;
                    public void FromUnmanaged(nint i) {}
                    public ManagedType ToManaged() => default;
                    public Span<int> GetManagedValuesDestination(int numElements) => default;
                    public ReadOnlySpan<T> GetUnmanagedValuesSource(int numElements) => default;
                    public void Free() {}
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(source,
                VerifyCS.Diagnostic(ElementTypesOfReturnTypesMustMatchRule).WithLocation(0).WithArguments("MarshallerType<T>.GetManagedValuesSource()", "MarshallerType<T>.GetManagedValuesDestination(int)"),
                VerifyCS.Diagnostic(ElementTypesOfReturnTypesMustMatchRule).WithLocation(1).WithArguments("MarshallerType<T>.GetManagedValuesSource()", "MarshallerType<T>.GetManagedValuesDestination(int)"));
        }

        [Fact]
        public async Task ModeThatUsesBidirectionalShape_ArrayTarget_DoesNotReportDiagnostic()
        {
            string source = """
                using System;
                using System.Runtime.InteropServices.Marshalling;
                
                [CustomMarshaller(typeof(CustomMarshallerAttribute.GenericPlaceholder[]), MarshalMode.ManagedToUnmanagedRef, typeof({|#0:MarshallerType<,>|}))]
                [CustomMarshaller(typeof(CustomMarshallerAttribute.GenericPlaceholder[]), MarshalMode.UnmanagedToManagedRef, typeof({|#1:MarshallerType<,>|}))]
                [ContiguousCollectionMarshaller]
                struct MarshallerType<T, TNative>
                {
                    public void FromManaged(T[] m) {}
                    public nint ToUnmanaged() => default;
                    public ReadOnlySpan<T> GetManagedValuesSource() => default;
                    public Span<TNative> GetUnmanagedValuesDestination() => default;
                    public void FromUnmanaged(nint i) {}
                    public T[] ToManaged() => default;
                    public Span<T> GetManagedValuesDestination(int numElements) => default;
                    public ReadOnlySpan<TNative> GetUnmanagedValuesSource(int numElements) => default;
                    public void Free() {}
                }
                """;

            await VerifyCS.VerifyAnalyzerAsync(source);
        }
    }
}
