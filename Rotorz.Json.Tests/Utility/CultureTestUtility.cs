// Copyright (c) 2014 Rotorz Limited. All rights reserved.

using System;
using System.Globalization;
using System.Threading;

namespace Rotorz.Tests {

	/// <summary>
	/// Utility functionality to facilitate with testing cultures.
	/// </summary>
	public static class CultureTestUtility {

		/// <summary>
		/// Execute test behaviour using a specific culture.
		/// </summary>
		/// <param name="culture">Culture information.</param>
		/// <param name="test">Test action which is to be performed.</param>
		public static void ExecuteInCulture(CultureInfo culture, Action test) {
			var thread = Thread.CurrentThread;

			var restoreCulture = thread.CurrentCulture;
			var restoreUICulture = thread.CurrentUICulture;

			try {
				thread.CurrentCulture = culture;
				thread.CurrentUICulture = culture;

				test();
			}
			finally {
				thread.CurrentCulture = restoreCulture;
				thread.CurrentUICulture = restoreUICulture;
			}
		}

		/// <summary>
		/// Execute test behaviour using a specific culture.
		/// </summary>
		/// <param name="cultureName">Name of culture; for instance, "fr-FR".</param>
		/// <param name="test">Test action which is to be performed.</param>
		public static void ExecuteInCulture(string cultureName, Action test) {
			ExecuteInCulture(new CultureInfo(cultureName), test);
		}

	}

}
