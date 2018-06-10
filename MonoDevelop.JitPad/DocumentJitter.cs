using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JitPad;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using MonoDevelop.Ide;

namespace MonoDevelop.JitPad
{
	class DocumentJitter : IDisposable
	{
		static readonly IEnumerable<JitResult> empty = Enumerable.Empty<JitResult>();
		static readonly SymbolDisplayFormat format = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

		Ide.Gui.Document doc;
		public DocumentJitter()
		{
			IdeApp.Workbench.ActiveDocumentChanged += Workbench_ActiveDocumentChanged;

			doc = IdeApp.Workbench.ActiveDocument;
			//Update();
		}

		void Workbench_ActiveDocumentChanged(object sender, EventArgs e)
		{
			doc = IdeApp.Workbench.ActiveDocument;
			//Update();
		}

		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		public async Task<IEnumerable<JitResult>> Update(Jitter jitter)
		{
			if (doc == null)
				return empty;

			var offset = doc.Editor.CaretOffset;

			var proj = doc.Project;
			if (proj == null)
				return empty;

			var roslynDoc = doc.AnalysisDocument;
			if (roslynDoc == null)
				return empty;

			cancellationTokenSource.Cancel();
			cancellationTokenSource = new CancellationTokenSource();
			var token = cancellationTokenSource.Token;
			return await Task.Run(async () =>
			{
				var sm = await roslynDoc.GetSemanticModelAsync(token);

				// member
				var memberSymbol = sm.GetEnclosingSymbol(offset, token);
				if (memberSymbol == null)
					return empty;
					
				string[] parameterTypeNames = Array.Empty<string> ();
				if (memberSymbol is IMethodSymbol methodSymbol)
				{
					parameterTypeNames = methodSymbol.Parameters.Select(x => x.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)).ToArray();
				}
				else if (!(memberSymbol is IPropertySymbol))
				{
					return empty;
				}

				// type
				var typeName = memberSymbol.ContainingType.ToDisplayString(format);

				await Core.Runtime.RunInMainThread(() => IdeApp.ProjectOperations.Build(proj, token).Task);

				var result = new CompileResult(roslynDoc.Project.OutputFilePath);

				return jitter.Jit(result, new MethodDescription (typeName, memberSymbol.MetadataName, parameterTypeNames));
			}, token);
		}

		public void Dispose()
		{
			doc = null;
			IdeApp.Workbench.ActiveDocumentChanged -= Workbench_ActiveDocumentChanged;
		}

	}
}
