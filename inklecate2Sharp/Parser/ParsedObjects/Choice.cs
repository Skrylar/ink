﻿using System;

namespace Inklewriter.Parsed
{
	public class Choice : Parsed.Object
	{
		public string choiceText { get; protected set; }
		public Divert divert { get; }

		public Choice (string choiceText, Divert divert)
		{
			this.choiceText = choiceText;
			this.divert = divert;

			divert.parent = this;
		}

		public override Runtime.Object GenerateRuntimeObject ()
		{
			var runtimeChoice = new Runtime.Choice (choiceText);
			this.divert.GenerateRuntimeObject ();
			return runtimeChoice;
		}

        public override void ResolveReferences(Story context)
		{
			// Don't actually use the Parsed.Divert in the runtime, but use its path resolution
			// to set the pathOnChoice property of the Runtime.Choice.

			divert.ResolveReferences (context);

			var runtimeChoice = runtimeObject as Runtime.Choice;
			runtimeChoice.pathOnChoice = divert.runtimeDivert.targetPath;
		}
	}

}

