﻿using System;
using System.Collections.Generic;

namespace Inklewriter.Parsed
{
	public class Story : FlowBase
    {
		public Story (List<Parsed.Object> toplevelObjects) : base(null, toplevelObjects)
		{

		}

		public Runtime.Story ExportRuntime()
		{
			// Get default implementation of runtimeObject, which calls ContainerBase's generation method
			var rootContainer = runtimeObject as Runtime.Container;

			// Replace runtimeObject with Story object instead of the Runtime.Container generated by Parsed.ContainerBase
			var runtimeStory = new Runtime.Story (rootContainer);
			runtimeObject = runtimeStory;

			// Now that the story has been fulled parsed into a hierarchy,
			// and the derived runtime hierarchy has been built, we can
			// resolve (translate) links/paths.
			// e.g. " -> knotName --> stitchName" into an INKPath (knotName.stitchName)
			// We don't make any assumptions that the INKPath follows the same
			// conventions as the script format, so we resolve to actual objects before
			// translating into an INKPath. (This also allows us to choose whether
			// we want the paths to be absolute)
			ResolveReferences (this);

			// Don't successfully return the object if there was an error
			if (_criticalError) {
				return null;
			}

			return runtimeStory;
		}

		public override Parsed.Object ResolvePath(Path path)
		{
			string knotName = path.knotName;
			if (knotName == null) {
				knotName = path.ambiguousName;
			}

			// Try to find the knot with the given name,
			// and if necessary dig in and find the stitch within
			if (knotName != null) {
				foreach (Parsed.Object contentObj in content) {
					Knot knot = contentObj as Knot;
					if (knot != null && knot.name == knotName) {
						if (path.stitchName != null) {
							return knot.ResolvePath (path);
						} else {
							return knot;
						}
					}
				}
			}

			return null;
		}

		public override void Error(string message, Parsed.Object source)
		{
			Console.WriteLine ("ERROR: "+message);
			_criticalError = true;
		}

		private bool _criticalError;
	}
}

