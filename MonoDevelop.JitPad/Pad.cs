using System;
using System.Linq;
using JitPad;
using MonoDevelop.Components;
using MonoDevelop.Ide.Gui;

namespace MonoDevelop.JitPad
{
	class Pad : PadContent
	{
		public override Control Control => new XwtControl(new PadWidget());

		class PadWidget : Xwt.Widget
		{
			Xwt.Button btn;
			Xwt.RichTextView textView;
			Xwt.ComboBox combo;
			readonly DocumentJitter jitter = new DocumentJitter ();

			public PadWidget()
			{
				jitter = new DocumentJitter();

				var box = new Xwt.VBox();

				combo = new Xwt.ComboBox();
				combo.Items.Add(Jitter.Mono, "Mono JIT");
				combo.Items.Add(Jitter.MonoAOT, "Mono AOT");
				combo.SelectedIndex = 0;
				box.PackStart(combo);

				btn = new Xwt.Button("JIT");
				btn.Clicked += ButtonClicked;
				box.PackStart(btn);

				textView = new Xwt.RichTextView
				{
					MinHeight = 400,
					MinWidth = 400
				};
				box.PackStart(textView, true, true);

				Content = box;
			}

			async void ButtonClicked(object sender, EventArgs args)
			{
				var results = await jitter.Update((Jitter)combo.SelectedItem);
				var text = results.FirstOrDefault()?.Asm;
				if (text == null)
					textView.LoadText("Nothing JITted", Xwt.Formats.TextFormat.Plain);
				else
					textView.LoadText(text, Xwt.Formats.TextFormat.Plain);
			}

			protected override void Dispose(bool disposing)
			{
				btn.Clicked -= ButtonClicked;
				btn = null;
				jitter?.Dispose();
				base.Dispose(disposing);
			}
		}
	}
}
