﻿namespace Aegis.Controllers
{
	using System.Diagnostics;

	using Aegis.Models;

	using Microsoft.AspNetCore.Mvc;

	/// <summary>
	/// Home Controller
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
	public class HomeController : Controller
	{
		/// <summary>
		/// Index.
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
			=> this.View();

		/// <summary>
		/// Error.
		/// </summary>
		/// <returns></returns>
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
			=> this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}