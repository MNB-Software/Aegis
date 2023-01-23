﻿namespace Aegis.Models.Auth
{
	using Aegis.Models.Shared;

	/// <summary>
	/// SignOut Command Result
	/// </summary>
	/// <seealso cref="Aegis.Models.Shared.BaseResult" />
	/// <seealso cref="System.IEquatable&lt;Aegis.Models.Shared.BaseResult&gt;" />
	/// <seealso cref="System.IEquatable&lt;Aegis.Models.Auth.SignOutCommandResult&gt;" />
	public sealed record SignOutCommandResult : BaseResult
	{
		/// <summary>
		/// Creates the success result.
		/// </summary>
		public static SignOutCommandResult Succeeded(string? returnUrl)
			=> new SignOutCommandResult { Success = true, ReturnUrl = returnUrl };

		/// <summary>
		/// Creates the failed result.
		/// </summary>
		public static SignOutCommandResult Failed()
			=> new SignOutCommandResult();

		/// <summary>
		/// Gets or sets the return URL.
		/// </summary>
		/// <value>
		/// The return URL.
		/// </value>
		public string? ReturnUrl { get; init; }

		/// <summary>
		/// Initializes a new instance of the <see cref="SignOutCommandResult"/> class.
		/// </summary>
		/// <param name="success">if set to <c>true</c> [succeeded].</param>
		public SignOutCommandResult() : base() { }
	}
}
