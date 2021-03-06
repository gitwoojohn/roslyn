// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;

namespace Microsoft.CodeAnalysis.CodeFixes.Suppression
{
    internal abstract partial class AbstractSuppressionCodeFixProvider : ISuppressionFixProvider
    {
        internal sealed class LocalSuppressMessageCodeAction : CodeAction
        {
            private readonly AbstractSuppressionCodeFixProvider _fixer;
            private readonly string _title;
            private readonly ISymbol _targetSymbol;
            private readonly SyntaxNode _targetNode;
            private readonly Document _document;
            private readonly Diagnostic _diagnostic;

            public LocalSuppressMessageCodeAction(AbstractSuppressionCodeFixProvider fixer, ISymbol targetSymbol, SyntaxNode targetNode, Document document, Diagnostic diagnostic)
            {
                _fixer = fixer;
                _targetSymbol = targetSymbol;
                _targetNode = targetNode;
                _document = document;
                _diagnostic = diagnostic;

                _title = FeaturesResources.SuppressWithLocalSuppressMessage;
            }

            protected async override Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
            {
                var newTargetNode = _fixer.AddLocalSuppressMessageAttribute(_targetNode, _targetSymbol, _diagnostic);
                var root = await _document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
                var newRoot = root.ReplaceNode(_targetNode, newTargetNode);
                return _document.WithSyntaxRoot(newRoot);
            }

            public override string Title
            {
                get
                {
                    return _title;
                }
            }

            internal SyntaxNode TargetNode_TestOnly
            {
                get
                {
                    return _targetNode;
                }
            }
        }
    }
}
